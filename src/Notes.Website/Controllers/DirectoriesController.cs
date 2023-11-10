using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Business.Services.Abstractions;
using Notes.Data;

namespace Notes.Website.Controllers;

/// <summary>
/// API section for handling of retrieval of directory information
/// for a specific user. Data is handled in a generic format decoupled from
/// Home/Projects/Notes specifically, and treats them all as generic "Directory" objects
/// </summary>
/// <remarks>
/// For API section with respect to specifically handling of Projects/Notes individually:
/// <seealso cref="ProjectsController"/>
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DirectoriesController : ControllerBase
{
    private NotesDbContext DB { get; }

    private IDirectoryService DirectoryService { get; }

    /// <inheritdoc />
    public DirectoriesController(NotesDbContext db, IDirectoryService directoryService)
    {
        DB = db;
        DirectoryService = directoryService;
    }

    /// <summary>
    /// Fetches the list of items that the user has modified in their history
    /// in order of most recent to least
    /// </summary>
    [HttpGet("recent")]
    public IActionResult Recent([FromQuery]int skip = 0, [FromQuery]int take = 5)
    {
        if (!DirectoryService.TryGetRecent(DB, out var readModel, skip, take))
        {
            return NotFound();
        }

        return Ok(readModel);
    }

    /// <summary>
    /// Fetches the high level overview of directories relevant for a given specified directory.
    /// This includes the Home dir, all root Projects, 
    /// the list of files 1 layer into the opened project (if directory id is or is in a project)
    /// and all directories from Home drilling directly down to <see cref="directoryId"/> depth first
    /// </summary>
    [HttpGet("{directoryId}/overview")]
    public IActionResult Overview(string directoryId)
    {
        if (!DirectoryService.TryGetOverview(DB, directoryId, out var readModel))
        {
            return NotFound();
        }

        return Ok(readModel);

    }

    /// <summary>
    /// Gets all immediate descendants (1 layer of depth)
    /// for a specific directory id, whether it is a project or a note
    /// </summary>
    [HttpGet("{directoryId}/descendants")]
    public IActionResult Descendants(string directoryId)
    {
        if (!DirectoryService.TryGetDescendants(DB, directoryId, out var readModel))
        {
            return NotFound();
        }

        return Ok(readModel);
    }
}
