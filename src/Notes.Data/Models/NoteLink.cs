using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Notes.Data.Models;

[Keyless]
public class NoteLink
{
    [ForeignKey(nameof(Note))]
    [Required]
    public string FromNoteId { get; set; } = null!;

    [ForeignKey(nameof(Note))]
    [Required]
    public string ToNoteId { get; set; } = null!;
}