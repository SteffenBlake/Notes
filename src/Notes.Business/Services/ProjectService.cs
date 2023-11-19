using Microsoft.EntityFrameworkCore;
using Notes.Business.Models.Projects;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services;

///<inheritdoc />
public class ProjectService : IProjectService
{
    private IHttpContextService HttpContext { get; }

    private IEditHistoryService EditHistory { get; }

    public ProjectService(IHttpContextService httpContext, IEditHistoryService editHistory)
    {
        HttpContext = httpContext;
        EditHistory = editHistory;
    }

    public async Task<TryResult<ProjectIndexModel>> TryIndexAsync(NotesDbContext db)
    {
        var projects = await db.Projects
            .Where(p => p.UserId == HttpContext.UserId)
            .Select(ProjectReadModel.ToModel(db))
            .ToListAsync();

        var model = new ProjectIndexModel
        {
            Data = projects
        };
        return TryResult<ProjectIndexModel>.Succeed(model);
    }

    ///<inheritdoc />
    public async Task<TryResult<ProjectReadModel>> TryGetAsync(NotesDbContext db, string projectName)
    {
        var readModel = await db.Projects
            .Where(p => 
                p.Name == projectName &&
                p.UserId == HttpContext.UserId
            )
            .Select(ProjectReadModel.ToModel(db))
            .SingleOrDefaultAsync();

        return readModel == null ?
            TryResult<ProjectReadModel>.NotFound() :
            TryResult<ProjectReadModel>.Succeed(readModel);
    }

    ///<inheritdoc />
    public async Task<TryResult<ProjectReadModel>> TryPutAsync(NotesDbContext db, string projectName, ProjectWriteModel writeModel)
    {
        var project = await db.Projects.SingleOrDefaultAsync(p => 
            p.Name == projectName &&
            p.UserId == HttpContext.UserId
        );

        project ??= (await db.Projects.AddAsync(
            new Project
            {
                Name = projectName,
                UserId = HttpContext.UserId!
            })).Entity;

        await writeModel.WriteAsync(db, HttpContext.UserId, project);

        await db.SaveChangesAsync();

        await EditHistory.AddProjectEventAsync(db, project.ProjectId);

        return await TryGetAsync(db, projectName);
    }

    ///<inheritdoc />
    public async Task<TryResult<object>> TryDeleteAsync(NotesDbContext db, string projectName)
    {
        if (await db.Notes.AnyAsync(n => n.Project.Name == projectName))
        {
            return TryResult<object>.Conflict(nameof(projectName), "Cannot delete a project with associated notes");
        }

        var project = db.Projects.SingleOrDefault(p => 
            p.Name == projectName &&
            p.UserId == HttpContext.UserId
        );
        if (project == null)
        {
            return TryResult<object>.Gone();
        }

        db.Remove(project);
        await db.SaveChangesAsync();
        return TryResult<object>.Succeed(new { });
    }
}