using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Business.Services.Abstractions;
using Notes.Data;

namespace Notes.Website.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private NotesDbContext DB { get; }
    private IProjectService ProjectService { get; }
    private INoteService NoteService { get; }

    public ProjectsController(NotesDbContext db, IProjectService projectService, INoteService noteService)
    {
        DB = db;
        ProjectService = projectService;
        NoteService = noteService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return Ok();
    }

    [HttpPost("")]
    public IActionResult CreateProject()
    {
        return Ok();
    }

    [HttpGet("{projectId}")]
    public IActionResult Read(string projectId)
    {
        return Ok();
    }

    [HttpPut("{projectId}")]
    public IActionResult Update(string projectId)
    {
        return Ok();
    }

    [HttpDelete("{projectId}")]
    public IActionResult Delete(string projectId)
    {
        return Ok();
    }

    [HttpGet("{projectId}/notes")]
    public IActionResult IndexNotes(string projectId)
    {
        return Ok();
    }

    [HttpGet("{projectId}/notes/{**notePath}")]
    public IActionResult ReadNote(string projectId, string notePath)
    {
        if (!NoteService.TryGet(DB, projectId, notePath, out var readModel))
        {
            return NotFound();
        }

        return Ok(readModel);
    }

    [HttpPut("{projectId}/notes/{**notePath}")]
    public IActionResult CrupdateNote(string projectId, string notePath)
    {
        return Ok();
    }

    [HttpDelete("{projectId}/notes/{**notePath}")]
    public IActionResult DeleteNote(string projectId, string notePath)
    {
        return Ok();
    }
}