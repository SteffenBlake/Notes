using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Business.Models.Notes;
using Notes.Business.Services.Abstractions;
using Notes.Data;

namespace Notes.Website.Controllers;

/// <summary>
/// Api group for handling of ICRUD for Projects and their notes
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private NotesDbContext DB { get; }
    private IProjectService ProjectService { get; }
    private INoteService NoteService { get; }

    /// <summary>
    /// Dependency Injections
    /// </summary>
    public ProjectsController(NotesDbContext db, IProjectService projectService, INoteService noteService)
    {
        DB = db;
        ProjectService = projectService;
        NoteService = noteService;
    }

    /// <summary>
    /// Fetches the list of all Projects for a User
    /// </summary>
    [HttpGet("")]
    public IActionResult IndexProjects()
    {
        return Ok();
    }

    /// <summary>
    /// Reads out the data for a specific Project by name
    /// </summary>
    /// <param name="projectName"></param>
    [HttpGet("{projectName}")]
    public IActionResult ReadProject(string projectName)
    {
        return Ok();
    }

    /// <summary>
    /// Adds or Updates a project to/in the system, by project name
    /// </summary>
    [HttpPut("{projectName}")]
    public IActionResult PutProject(string projectName)
    {
        return Ok();
    }

    /// <summary>
    /// Deletes a project
    /// </summary>
    [HttpDelete("{projectName}")]
    public IActionResult DeleteProject(string projectName)
    {
        return Ok();
    }

    /// <summary>
    /// Indexes all the notes for a given project by project name
    /// </summary>
    [HttpGet("{projectName}/notes")]
    public IActionResult IndexNotes(string projectName)
    {
        if (!NoteService.TryIndex(DB, projectName, out var indexModel) || indexModel == null)
        {
            return NotFound();
        }

        return Ok(indexModel);
    }

    /// <summary>
    /// Reads a specific note's data from a project, by project name and unique path for that note
    /// </summary>
    [HttpGet("{projectName}/notes/{**notePath}")]
    public IActionResult ReadNote(string projectName, string notePath)
    {
        if (!NoteService.TryGet(DB, projectName, notePath, out var readModel))
        {
            return BadRequest();
        }

        if (readModel == null)
        {
            return NotFound();
        }

        return Ok(readModel);
    }

    /// <summary>
    /// Adds or Updates a note for a project, by project name and unique note path
    /// </summary>
    [HttpPut("{projectName}/notes/{**notePath}")]
    public IActionResult PutNote(
        [FromRoute]string projectName,
        [FromRoute] string notePath, 
        [FromBody]NoteWriteModel writeModel
    )
    {
        if (!NoteService.TryPut(DB, projectName, notePath, writeModel, out var readModel))
        {
            return BadRequest();
        }

        if (readModel == null)
        {
            return NotFound();
        }

        return Ok(readModel);
    }

    /// <summary>
    /// Deletes a specific note for a project by project name and unique note path.
    /// If the note has children beneath it's path, it's contents will just be cleared instead
    /// </summary>
    [HttpDelete("{projectName}/notes/{**notePath}")]
    public IActionResult DeleteNote(string projectName, string notePath)
    {
        if (!NoteService.TryDelete(DB, projectName, notePath))
        {
            return NotFound();
        }

        return Ok();
    }
}