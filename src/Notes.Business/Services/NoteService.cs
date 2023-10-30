using Notes.Business.Models.Notes;
using Notes.Business.Services.Abstractions;
using Notes.Business.Services.Models.Notes;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services;

public class NoteService : INoteService
{
    public bool TryIndex(NotesDbContext db, string projectName, out NoteIndexModel? indexModel)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }

        var project = db.Projects.SingleOrDefault(p => p.Name == projectName);
        if (project == null)
        {
            indexModel = null;
            return false;
        }

        var data = db.Notes.Where(n => n.Project.Name == projectName)
            .Select(NoteReadModel.FromModel(db))
            .ToList();

        indexModel = new NoteIndexModel
        {
            Data = data
        };

        return true;
    }

    public bool TryPut(NotesDbContext db, string projectName, string path, NoteWriteModel writeModel, out NoteReadModel? readModel)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var projectId = db.Projects.Where(p => p.Name == projectName)
            .Select(p => p.ProjectId)
            .SingleOrDefault();
        if (string.IsNullOrEmpty(projectId))
        {
            readModel = null;
            return false;
        }

        if (
            !TryGetInternal(db, projectName, path, out var heirarchy, out var segments) ||
            heirarchy.FirstOrDefault() == null
        )
        {
            for (var i = 0; i < heirarchy.Length; i++)
            {
                heirarchy[i] ??= db.Notes.Add(new Note
                {
                    Name = segments[i],
                    ProjectId = projectId
                }).Entity;

                if (i == 0)
                {
                    continue;
                }
                heirarchy[i - 1]!.ParentNoteId ??= heirarchy[i]!.NoteId;
            }
        }

        writeModel.Write(db, heirarchy[0]!);

        db.SaveChanges();

        return TryGet(db, projectName, path, out readModel);
    }

    public bool TryGet(NotesDbContext db, string projectName, string path, out NoteReadModel? readModel)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        Note? note;
        if (
            !TryGetInternal(db, projectName, path, out var heirarchy, out _) ||
            (note = heirarchy.FirstOrDefault()) == null
        )
        {
            readModel = null;
            return false;
        }

        readModel = NoteReadModel.FromModel(db).Compile()(note);

        return true;
    }
    
    public bool TryDelete(NotesDbContext db, string projectName, string path)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        Note? note;
        if (
            !TryGetInternal(db, projectName, path, out var heirarchy, out _) ||
            (note = heirarchy.FirstOrDefault()) == null
        )
        {
            return false;
        }

        if (db.Notes.Any(n => n.ParentNoteId == note.NoteId))
        {
            note.ContentRaw = null;
            note.HtmlContent = null;
        }
        else
        {
            db.Remove(note);
        }

        db.SaveChanges();
        return true;
    }

    private bool TryGetInternal(NotesDbContext db, string projectName, string path, out Note?[] heirarchy, out string[] segments)
    {
        // this seems to be the cleanest way to achieve our desired result
        // of splitting up a uri path into segments
        var segmentsInternal = new Uri("http://a.b/" + path)
            .Segments
            .Select(s => s.Replace('/', ' ').Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .Reverse()
            .ToList();

        segments = segmentsInternal.ToArray();

        heirarchy = new Note[segmentsInternal.Count];

        var baseQuery = db.Notes.Where(n =>
            n.Project.Name == projectName &&
            segmentsInternal.Contains(n.Name)
        ).ToDictionary(n => n.NoteId, n => n);

        // Get the matching root note
        var note = baseQuery.SingleOrDefault(n => n.Value.ParentNoteId == null && n.Value.Name == segmentsInternal[^1]).Value;
        if (note == null)
        {
            return false;
        }

        heirarchy[^1] = note;

        for (var i = 2; i <= segmentsInternal.Count; i++)
        {
            var noteId = note.NoteId;
            note = baseQuery.SingleOrDefault(n => n.Value.ParentNoteId == noteId && n.Value.Name == segmentsInternal[^i]).Value;
            if (note == null)
            {
                return false;
            }

            heirarchy[^i] = note;
        }

        return true;
    }
}