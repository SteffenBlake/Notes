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
    public bool TryGetRecent(in NotesDbContext db, out DirectoryRecentReadModel readModel, int skip = 0, int take = 5)
    {
        var recent = EditHistory.GetHistory(db, skip, take)
            .Select(DirectoryReadModel.Read(db))
            .ToList();

        readModel = new()
        {
            Recent = recent
        };

        return true;
    }

    ///<inheritdoc />
    public bool TryGetOverview(in NotesDbContext db, string directoryId, out DirectoryOverviewReadModel readModel)
    {
        readModel = new();

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

        var all = db.EditHistory.Where(h =>
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
            .ToList()
            .GroupBy(h => h.Id)
            .Select(g => g.First())
            .ToDictionary(m => m.Id, m => m);

        if (directoryId != "0" && !all.ContainsKey(directoryId))
        {
            return false;
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

        return true;
    }

    ///<inheritdoc />
    public bool TryGetDescendants(in NotesDbContext db, string directoryId, out DirectoryDescendantsReadModel readModel)
    {
        var descendants = db.EditHistory
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
            .ToList()
            .GroupBy(h => h.Id)
            .Select(h => h.First())
            .ToList();

        readModel = new()
        {
            Descendants = descendants
        };

        return readModel.Descendants.Any();
    }
}