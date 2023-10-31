using Notes.Business.Models.Projects;
using Notes.Data;

namespace Notes.Business.Services.Abstractions;

public interface IProjectService
{
    /// <summary>
    /// Fetches the list of all Projects for a User
    /// </summary>
    bool TryIndex(NotesDbContext db, out ProjectIndexModel? indexModel);

    /// <summary>
    /// Reads out the data for a specific Project by name
    /// </summary>
    bool TryGet(NotesDbContext db, string projectName, out ProjectReadModel? readModel);

    /// <summary>
    /// Adds or Updates a project to/in the system, by project name
    /// </summary>
    bool TryPut(NotesDbContext db, string projectName, ProjectWriteModel writeModel, out ProjectReadModel? readModel);
    
    /// <summary>
    /// Deletes a project. Throws an exception if the project has any notes associated with it.
    /// </summary>
    bool TryDelete(NotesDbContext db, string projectName);
}