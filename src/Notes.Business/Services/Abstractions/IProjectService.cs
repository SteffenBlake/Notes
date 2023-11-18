using Notes.Business.Models.Projects;
using Notes.Data;

namespace Notes.Business.Services.Abstractions;

public interface IProjectService
{
    /// <summary>
    /// Fetches the list of all Projects for a User
    /// </summary>
    Task<bool> TryIndexAsync(NotesDbContext db, out Task<ResultErrors> errorsTask, out Task<ProjectIndexModel?> indexModelTask);

    /// <summary>
    /// Reads out the data for a specific Project by name
    /// </summary>
    bool TryGet(in NotesDbContext db, string projectName, out ProjectReadModel? readModel);

    /// <summary>
    /// Adds or Updates a project to/in the system, by project name
    /// </summary>
    bool TryPut(in NotesDbContext db, string projectName, in ProjectWriteModel writeModel, out ProjectReadModel? readModel);
    
    /// <summary>
    /// Deletes a project. Throws an exception if the project has any notes associated with it.
    /// </summary>
    bool TryDelete(in NotesDbContext db, string projectName);
}