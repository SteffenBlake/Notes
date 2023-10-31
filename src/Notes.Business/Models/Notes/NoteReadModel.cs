using System.Linq.Expressions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Models.Notes;

public class NoteReadModel : NoteWriteModel
{
    public required string? ParentId { get; set; }

    public required string Id { get; set; }

    public static Expression<Func<Note, NoteReadModel>> FromModel(NotesDbContext db)
    {
        return (note) => new()
        {
            Id = note.NoteId,
            ParentId = note.ParentNoteId,
            HtmlContent = note.HtmlContent,
            ContentRaw = note.ContentRaw,
        };
    }
}