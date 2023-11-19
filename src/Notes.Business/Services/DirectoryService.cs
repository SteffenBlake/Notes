using Microsoft.EntityFrameworkCore;
using Notes.Business.Models.Directories;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using System.Linq;

namespace Notes.Business.Services;

///<inheritdoc />
public class DirectoryService : IDirectoryService
{
    private IHttpContextService HttpContext { get; }

    private IEditHistoryService EditHistory { get; }

    public DirectoryService(IHttpContextService httpContext, IEditHistoryService editHistory)
    {
        HttpContext = httpContext;
        EditHistory = editHistory;
    }

    ///<inheritdoc />
    public async Task<TryResult<DirectoryRecentReadModel>> TryGetRecentAsync(NotesDbContext db, int skip = 0, int take = 5)
    {
        var recent = await EditHistory.GetHistory(db, skip, take)
            .Select(DirectoryReadModel.Read(db))
            .ToListAsync();

        var readModel = new DirectoryRecentReadModel()
        {
            Recent = recent
        };

        return TryResult<DirectoryRecentReadModel>.Succeed(readModel);
    }

    ///<inheritdoc />
    public async Task<TryResult<DirectoryOverviewReadModel>> TryGetOverviewAsync(NotesDbContext db, string directoryId)
    {
        var readModel = new DirectoryOverviewReadModel();

        var projectIds = db.EditHistory
            .Include(h => h.Note)
            .ThenInclude(n => n!.Project)
            .Where(h =>
                h.Note != null &&
                (
                    h.Note.ProjectId == directoryId ||
                    h.NoteId == directoryId
                )
            )
            .Select(h => h.Note!.ProjectId);

        var allHistory = await db.EditHistory.Where(h =>
                // Projects
                h.ProjectId != null ||
                // All notes inside that project (we will filter off the extras after)
                h.NoteId == directoryId ||
                (
                    h.Note != null &&
                    projectIds.Contains(h.Note.ProjectId)
                )
            )
            .Select(DirectoryReadModel.Read(db))
            .ToListAsync();

        var all = allHistory
            .GroupBy(h => h.Id)
            .Select(g => g.First())
            .ToDictionary(m => m.Id, m => m);

        if (directoryId != "0" && !all.ContainsKey(directoryId))
        {
            return TryResult<DirectoryOverviewReadModel>.NotFound();
        }

        var directoryScan = directoryId;
        while (directoryScan != "0")
        {
            var primaryDir = all[directoryScan];
            all.Remove(directoryScan);
            primaryDir.Selected = primaryDir.Id == directoryId;
            readModel.Primaries[directoryScan] = primaryDir;
            directoryScan = primaryDir.ParentId;
        }

        foreach (var (_, remaining) in all.OrderBy(kvp => kvp.Value.Name))
        {
            if (remaining.ParentId == "0")
            {
                readModel.Projects.Add(remaining);
            }
            else if (readModel.Primaries.ContainsKey(remaining.ParentId))
            {
                readModel.Notes.Add(remaining);
            }
        }

        return TryResult<DirectoryOverviewReadModel>.Succeed(readModel);
    }

    ///<inheritdoc />
    public async Task<TryResult<DirectoryDescendantsReadModel>> TryGetDescendantsAsync(NotesDbContext db, string directoryId)
    {
        var history = await db.EditHistory
            .Include(h => h.Note)
            .Include(h => h.Project)
            .Where(h =>
                h.Note != null &&
                (
                    (h.Note.ProjectId == directoryId && h.Note.ParentNoteId == null) ||
                    h.Note.ParentNoteId == directoryId
                )
            )
            .Select(DirectoryReadModel.Read(db))
            .ToListAsync();

        var descendants = history
            .GroupBy(h => h.Id)
            .Select(h => h.First())
            .ToList();

        if (!descendants.Any())
        {
            return TryResult<DirectoryDescendantsReadModel>.NotFound();
        }

        var readModel = new DirectoryDescendantsReadModel()
        {
            Descendants = descendants
        };

        return TryResult<DirectoryDescendantsReadModel>.Succeed(readModel);
    }
}