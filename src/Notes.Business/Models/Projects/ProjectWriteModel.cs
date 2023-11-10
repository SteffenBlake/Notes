using Notes.Data;
using Notes.Data.Models;
using Notes.Data.Models.Identity;

namespace Notes.Business.Models.Projects;
public class ProjectWriteModel
{
    public string? Description { get; set; }

    public string Icon { get; set; } = "journal";

    public void Write(NotesDbContext db, string? userId, Project project)
    {
        project.Description = Description;
        project.Icon = Icon;
    }
}