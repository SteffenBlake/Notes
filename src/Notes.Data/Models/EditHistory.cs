using System.ComponentModel.DataAnnotations;
using Notes.Data.Models.Identity;

namespace Notes.Data.Models;

public class EditHistory
{
    public EditHistory()
    {
        EditHistoryId = Guid.NewGuid().ToString();
    }

    [Key]
    public string EditHistoryId { get; set; }

    [Required]
    public DateTimeOffset Timestamp { get; set; } = default!;

    public NotesUser EdittedBy { get; set; } = default!;
    [Required]
    public string EdittedById { get; set; } = default!;

    public Note? Note { get; set; }
    public string? NoteId { get; set; }

    public Project? Project { get; set; }
    public string? ProjectId { get; set; }
}