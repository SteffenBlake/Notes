using Notes.Business.Models.Projects;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services;

///<inheritdoc />
public class ProjectService : IProjectService
{
    private IHttpContextService HttpContext { get; }

    public ProjectService(IHttpContextService httpContext)
    {
        HttpContext = httpContext;
    }

    ///<inheritdoc />
    public bool TryIndex(NotesDbContext db, out ProjectIndexModel? indexModel)
    {
        var projects = db.Projects
            .Where(p => p.UserId == HttpContext.UserId)
            .Select(ProjectReadModel.ToModel(db))
            .ToList();

        indexModel = new ProjectIndexModel
        {
            Data = projects
        };

        return true;
    }

    ///<inheritdoc />
    public bool TryGet(NotesDbContext db, string projectName, out ProjectReadModel? readModel)
    {
        readModel = db.Projects
            .Where(p => 
                p.Name == projectName &&
                p.UserId == HttpContext.UserId
            )
            .Select(ProjectReadModel.ToModel(db))
            .SingleOrDefault();

        return readModel != null;
    }

    ///<inheritdoc />
    public bool TryPut(NotesDbContext db, string projectName, ProjectWriteModel writeModel, out ProjectReadModel? readModel)
    {
        var project = db.Projects.SingleOrDefault(p => 
            p.Name == projectName &&
            p.UserId == HttpContext.UserId
        );
        project ??= db.Projects.Add(
            new Project
            {
                Name = projectName,
                UserId = HttpContext.UserId
            }).Entity;

        writeModel.Write(db, project);
        db.SaveChanges();

        return TryGet(db, projectName, out readModel);
    }

    ///<inheritdoc />
    public bool TryDelete(NotesDbContext db, string projectName)
    {
        var project = db.Projects.SingleOrDefault(p => 
            p.Name == projectName &&
            p.UserId == HttpContext.UserId
        );
        if (project == null)
        {
            return false;
        }

        if (db.Notes.Any(n => n.ProjectId == project.ProjectId))
        {
            throw new InvalidOperationException("Cannot delete a project with associated notes");
        }

        db.Remove(project);
        db.SaveChanges();
        return true;
    }
}