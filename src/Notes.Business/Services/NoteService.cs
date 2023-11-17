using Notes.Business.Models.Notes;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services;

/// <inheritdoc />
public class NoteService : INoteService
{
    private IHttpContextService HttpContext { get; }

    private IEditHistoryService EditHistory { get; }

    public NoteService(IHttpContextService httpContext, IEditHistoryService editHistory)
    {
        HttpContext = httpContext;
        EditHistory = editHistory;
    }

    /// <inheritdoc />
    public bool TryIndex(in NotesDbContext db, string projectName, out NoteIndexModel? indexModel)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }

        var project = db.Projects.SingleOrDefault(p => 
            p.Name == projectName &&
            p.UserId == HttpContext.UserId
        );
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

    /// <inheritdoc />
    public bool TryPut(in NotesDbContext db, string projectName, string path, in NoteWriteModel writeModel, out NoteReadModel? readModel)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var user = db.Users.Find(HttpContext.UserId)!;

        var projectId = db.Projects.Where(p => 
                p.Name == projectName &&
                p.UserId == HttpContext.UserId
            )
            .Select(p => p.ProjectId)
            .SingleOrDefault();
        if (string.IsNullOrEmpty(projectId))
        {
            readModel = null;
            return false;
        }

        if (
            !TryGetInternal(db, projectName, path, out var heirarchy, out var segments) ||
            heirarchy.LastOrDefault() == null
        )
        {
            for (var i = 0; i < heirarchy.Length; i++)
            {
                var route = $"/{projectName}/{string.Join('/', segments[..(i+1)])}";
                heirarchy[i] ??= db.Notes.Add(new Note
                {
                    Name = segments[i],
                    ProjectId = projectId,
                    Route = route
                }).Entity;

                if (i == 0)
                {
                    continue;
                }
                heirarchy[i]!.ParentNoteId ??= heirarchy[i -1]!.NoteId;
            }
        }

        writeModel.Write(db, user, heirarchy[^1]!);

        db.SaveChanges();

        EditHistory.AddNoteEvent(db, heirarchy[0]!.NoteId);

        return TryGet(db, projectName, path, out readModel);
    }

    /// <inheritdoc />
    public bool TryGet(in NotesDbContext db, string projectName, string path, out NoteReadModel? readModel)
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
            (note = heirarchy.LastOrDefault()) == null
        )
        {
            readModel = null;
            return false;
        }

        readModel = NoteReadModel.FromModel(db).Compile()(note);

        return true;
    }
    
    /// <inheritdoc />
    public bool TryDelete(in NotesDbContext db, string projectName, string path)
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
            (note = heirarchy.LastOrDefault()) == null
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

    private bool TryGetInternal(in NotesDbContext db, string projectName, string path, out Note?[] heirarchy, out string[] segments)
    {
        // this seems to be the cleanest way to achieve our desired result
        // of splitting up a uri path into segments
        var segmentsInternal = new Uri("http://a.b/" + path)
            .Segments
            .Select(s => s.Replace('/', ' ').Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        segments = segmentsInternal.ToArray();

        heirarchy = new Note[segmentsInternal.Count];

        var baseQuery = db.Notes.Where(n =>
            n.Project.Name == projectName &&
            n.Project.UserId == HttpContext.UserId &&
            segmentsInternal.Contains(n.Name)
        ).ToDictionary(n => n.NoteId, n => n);

        // Get the matching root note
        var note = baseQuery.SingleOrDefault(n => n.Value.ParentNoteId == null && n.Value.Name == segmentsInternal[0]).Value;
        if (note == null)
        {
            return false;
        }

        heirarchy[0] = note;

        for (var i = 1; i < segmentsInternal.Count; i++)
        {
            var noteId = note.NoteId;
            note = baseQuery.SingleOrDefault(n => n.Value.ParentNoteId == noteId && n.Value.Name == segmentsInternal[i]).Value;
            if (note == null)
            {
                return false;
            }

            heirarchy[i] = note;
        }

        return true;
    }
}