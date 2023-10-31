using System.Linq.Expressions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Models.Projects;

public class ProjectReadModel : ProjectWriteModel
{
    public required string ProjectId { get; set; }

    public required string Name { get; set; }

    public required bool CanDelete { get; set; }

    public static Expression<Func<Project, ProjectReadModel>> ToModel(NotesDbContext db)
    {
        return project => new()
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            CanDelete = !db.Notes.Any(n => n.ProjectId == project.ProjectId)
        };
    }
}