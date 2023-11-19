using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Models.Projects;
public class ProjectWriteModel
{
    public string? Description { get; set; }

    public string Icon { get; set; } = "journal";

    public Task WriteAsync(NotesDbContext db, string? userId, Project project)
    {
        project.Description = Description;
        project.Icon = Icon;

        return Task.CompletedTask;
    }
}