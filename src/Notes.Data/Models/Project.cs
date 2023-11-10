using System.ComponentModel.DataAnnotations;

namespace Notes.Data.Models;

public class Project
{
    public Project()
    {
        ProjectId = Guid.NewGuid().ToString();
    }

    [Key]
    public string ProjectId { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;

    public string? Description { get; set; } = null;

    public string Icon { get; set; } = "journal";
}