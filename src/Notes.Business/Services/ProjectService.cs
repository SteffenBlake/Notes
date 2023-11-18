using Microsoft.EntityFrameworkCore;
using Notes.Business.Extensions;
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

    public Task<bool> TryIndexAsync(
        NotesDbContext db, 
        out Task<ResultErrors> errorsTask, 
        out Task<ProjectIndexModel?> indexModelTask)
    {
        (var successTask, errorsTask, indexModelTask) = TryIndexAsyncInternal(db);
        return successTask;
    }

    private async Task<TryResult<ProjectIndexModel>> TryIndexAsyncInternal(NotesDbContext db)
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
    public bool TryGet(in NotesDbContext db, string projectName, out ProjectReadModel? readModel)
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
    public bool TryPut(in NotesDbContext db, string projectName, in ProjectWriteModel writeModel, out ProjectReadModel? readModel)
    {
        var project = db.Projects.SingleOrDefault(p => 
            p.Name == projectName &&
            p.UserId == HttpContext.UserId
        );
        project ??= db.Projects.Add(
            new Project
            {
                Name = projectName,
                UserId = HttpContext.UserId!
            }).Entity;

        writeModel.Write(db, HttpContext.UserId, project);

        db.SaveChanges();

        EditHistory.AddProjectEvent(db, project.ProjectId);

        return TryGet(db, projectName, out readModel);
    }

    ///<inheritdoc />
    public bool TryDelete(in NotesDbContext db, string projectName)
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