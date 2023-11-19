using Notes.Business.Models.Projects;
using Notes.Data;

namespace Notes.Business.Services.Abstractions;

public interface IProjectService
{
    /// <summary>
    /// Fetches the list of all Projects for a User
    /// </summary>
    Task<TryResult<ProjectIndexModel>> TryIndexAsync(NotesDbContext db);

    /// <summary>
    /// Reads out the data for a specific Project by name
    /// </summary>
    Task<TryResult<ProjectReadModel>> TryGetAsync(NotesDbContext db, string projectName);

    /// <summary>
    /// Adds or Updates a project to/in the system, by project name
    /// </summary>
    Task<TryResult<ProjectReadModel>> TryPutAsync(NotesDbContext db, string projectName, ProjectWriteModel writeModel);
    
    /// <summary>
    /// Deletes a project. Throws an exception if the project has any notes associated with it.
    /// </summary>
    Task<TryResult<object>> TryDeleteAsync(NotesDbContext db, string projectName);
}