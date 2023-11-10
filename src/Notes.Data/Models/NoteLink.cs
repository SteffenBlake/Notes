using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes.Data.Models;

public class NoteLink
{
    public NoteLink()
    {
        NoteLinkId = Guid.NewGuid().ToString();
    }

    [Key]
    public string NoteLinkId { get; set; }

    [ForeignKey(nameof(Note))]
    [Required]
    public string FromNoteId { get; set; } = null!;

    [ForeignKey(nameof(Note))]
    [Required]
    public string ToNoteId { get; set; } = null!;
}