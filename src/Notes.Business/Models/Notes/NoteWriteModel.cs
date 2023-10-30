using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Models.Notes;

public class NoteWriteModel
{
    public string? ContentRaw { get; set; }

    public string? HtmlContent { get; set; }

    public void Write(NotesDbContext db, Note note)
    {
        note.ContentRaw = ContentRaw;
        note.HtmlContent = HtmlContent;
    }
}