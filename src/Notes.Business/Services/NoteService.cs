using Notes.Business.Models.Notes;
using Notes.Business.Services.Abstractions;
using Notes.Business.Services.Models.Notes;
using Notes.Data;

namespace Notes.Business.Services;

public class NoteService : INoteService
{
    public bool TryGet(NotesDbContext db, string projectName, string path, out NoteReadModel? readModel)
    {
        // this seems to be the cleanest way to achieve our desired result
        // of splitting up a uri path into segments
        var segments = new Uri("http://a.b/" + path)
            .Segments
            .Select(s => s.Replace('/', ' ').Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        var baseQuery = db.Notes.Where(n => 
            n.Project.Name == projectName && 
            segments.Contains(n.Name)
        ).ToDictionary(n => n.NoteId, n => n);

        if (!baseQuery.Any())
        {
            readModel = null;
            return false;
        }

        // Get the matching root note
        var note = baseQuery.SingleOrDefault(n => n.Value.ParentNoteId == null && n.Value.Name == segments[0]).Value;
        if (note == null)
        {
            readModel = null;
            return false;
        }

        for (var i = 1; i < segments.Count; i++)
        {
            note = baseQuery.SingleOrDefault(n => n.Value.ParentNoteId == note.NoteId && n.Value.Name == segments[i]).Value;
            if (note == null)
            {
                readModel = null;
                return false;
            }
        }

        readModel = new()
        {
            HtmlContent = note.HtmlContent,
            ContentRaw = note.ContentRaw,
        };

        return true;
    }

    public bool TryPut(NotesDbContext db, string projectId, string path, NoteWriteModel writeModel, out NoteReadModel? readModel)
    {
        readModel = null;
        return false;
    }
}