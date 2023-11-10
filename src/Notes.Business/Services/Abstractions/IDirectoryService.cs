using Notes.Business.Models.Directories;
using Notes.Data;

namespace Notes.Business.Services.Abstractions;

/// <summary>
/// Service for pulling of directory information in a generic manner, whether it is by project or by note
/// Primarily utilized via the Directory Browsing functionality of the frontend
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Fetches the list of items that the user has modified in their history, in order of most recent to least
    /// </summary>
    bool TryGetRecent(NotesDbContext db, out DirectoryRecentReadModel readModel, int skip = 0, int take = 5);

    /// <summary>
    /// Fetches the high level overview of directories relevant for a given specified directory.
    /// This includes the Home dir, all root Projects, 
    /// the list of files 1 layer into the opened project (if directory id is or is in a project)
    /// and all directories from Home drilling directly down to <see cref="directoryId"/> depth first
    /// </summary>
    bool TryGetOverview(NotesDbContext db, string directoryId, out DirectoryOverviewReadModel readModel);

    /// <summary>
    /// Gets all immediate descendants (1 layer of depth)
    /// for a specific directory id, whether it is a project or a note
    /// </summary>
    bool TryGetDescendants(NotesDbContext db, string directoryId, out DirectoryDescendantsReadModel readModel);
}