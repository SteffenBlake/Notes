using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Business.Models.Notes;
using Notes.Business.Models.Projects;
using Notes.Business.Services.Abstractions;
using Notes.Data;

namespace Notes.Website.Controllers;

/// <summary>
/// Api group for handling of ICRUD for Projects and their notes
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController(NotesDbContext db, IProjectService ProjectService, INoteService NoteService) 
    : NotesControllerBase(db)
{
    /// <summary>
    /// Fetches the list of all Projects for a User
    /// </summary>
    [HttpGet("")]
    public async Task<IActionResult> IndexProjects()
    {
        return await HandleTryResultAsync(ProjectService.TryIndexAsync);
    }

    /// <summary>
    /// Reads out the data for a specific Project by name
    /// </summary>
    [HttpGet("{projectName}")]
    public async Task<IActionResult> ReadProject(
        [FromRoute] string projectName
    )
    {
        return await HandleTryResultAsync((db) => ProjectService.TryGetAsync(db, projectName));
    }

    /// <summary>
    /// Adds or Updates a project to/in the system, by project name
    /// </summary>
    [HttpPut("{projectName}")]
    public async Task<IActionResult> PutProject(
        [FromRoute]string projectName,
        [FromBody]ProjectWriteModel writeModel
    )
    {
        return await HandleTryResultAsync((db) => ProjectService.TryPutAsync(db, projectName, writeModel));
    }

    /// <summary>
    /// Deletes a project. Throws an exception if the project has any notes associated with it.
    /// </summary>
    [HttpDelete("{projectName}")]
    public async Task<IActionResult> DeleteProject(
        [FromRoute] string projectName
    )
    {
        return await HandleTryResultAsync((db) => ProjectService.TryDeleteAsync(db, projectName));
    }

    /// <summary>
    /// Indexes all the notes for a given project by project name
    /// </summary>
    [HttpGet("{projectName}/notes")]
    public async Task<IActionResult> IndexNotes(
        [FromRoute] string projectName
    )
    {
        return await HandleTryResultAsync((db) => NoteService.TryIndexAsync(db, projectName));
    }

    /// <summary>
    /// Reads a specific note's data from a project, by project name and unique path for that note
    /// </summary>
    [HttpGet("{projectName}/notes/{**notePath}")]
    public async Task<IActionResult> ReadNote(
        [FromRoute] string projectName, 
        [FromRoute] string notePath
    )
    {
        return await HandleTryResultAsync((db) => NoteService.TryGetAsync(db, projectName, notePath));
    }

    /// <summary>
    /// Adds or Updates a note for a project, by project name and unique note path
    /// </summary>
    [HttpPut("{projectName}/notes/{**notePath}")]
    public async Task<IActionResult> PutNote(
        [FromRoute]string projectName,
        [FromRoute] string notePath, 
        [FromBody]NoteWriteModel writeModel
    )
    {
        return await HandleTryResultAsync((db) => NoteService.TryPutAsync(db, projectName, notePath, writeModel));
    }

    /// <summary>
    /// Deletes a specific note for a project by project name and unique note path.
    /// If the note has children beneath it's path, it's contents will just be cleared instead
    /// </summary>
    [HttpDelete("{projectName}/notes/{**notePath}")]
    public async Task<IActionResult> DeleteNote(
        [FromRoute] string projectName, 
        [FromRoute] string notePath
    )
    {
        return await HandleTryResultAsync((db) => NoteService.TryDeleteAsync(db, projectName, notePath));
    }
}