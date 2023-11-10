using Notes.Data;
using Notes.Data.Models;
using Notes.Data.Models.Identity;

namespace Notes.Business.Models.Notes;

public class NoteWriteModel
{
    public string? ContentRaw { get; set; }

    public string? HtmlContent { get; set; }

    public string? Icon { get; set; }

    public void Write(NotesDbContext db, NotesUser user, Note note)
    {
        note.ContentRaw = ContentRaw;
        note.HtmlContent = HtmlContent;
        note.Icon = Icon ?? (string.IsNullOrEmpty(ContentRaw) ? "file-earmark" : "file-earmark-text");
    }
}