using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Business.Extensions;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using System.Buffers;
using System.IO.Pipelines;
using System.IO.Pipes;

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
public class DirectoriesController(NotesDbContext db, IDirectoryService DirectoryService) 
    : NotesControllerBase(db)
{
    /// <summary>
    /// Fetches the list of items that the user has modified in their history
    /// in order of most recent to least
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> Recent([FromQuery]int skip = 0, [FromQuery]int take = 5)
    {
        return await HandleTryResultAsync((db) => DirectoryService.TryGetRecentAsync(db, skip, take));
    }

    /// <summary>
    /// Fetches the high level overview of directories relevant for a given specified directory.
    /// This includes the Home dir, all root Projects, 
    /// the list of files 1 layer into the opened project (if directory id is or is in a project)
    /// and all directories from Home drilling directly down to <see cref="directoryId"/> depth first
    /// </summary>
    [HttpGet("{directoryId}/overview")]
    public async Task<IActionResult> Overview(string directoryId)
    {
        return await HandleTryResultAsync((db) => DirectoryService.TryGetOverviewAsync(db, directoryId));
    }

    /// <summary>
    /// Gets all immediate descendants (1 layer of depth)
    /// for a specific directory id, whether it is a project or a note
    /// </summary>
    [HttpGet("{directoryId}/descendants")]
    public async Task<IActionResult> Descendants(string directoryId)
    {
        return await HandleTryResultAsync((db) => DirectoryService.TryGetDescendantsAsync(db, directoryId));
    }
}
