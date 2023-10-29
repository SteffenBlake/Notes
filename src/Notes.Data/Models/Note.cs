using System.ComponentModel.DataAnnotations;

namespace Notes.Data.Models;

public class Note
{
    public Note()
    {
        NoteId = Guid.NewGuid().ToString();
    }

    [Key]
    public string NoteId { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string? ParentNoteId { get; set; }

    public Project Project { get; set; }
    [Required]
    public string ProjectId { get; set; } = null!;

    public string? HtmlContent { get; set; }

    public string? ContentRaw { get; set; }

}