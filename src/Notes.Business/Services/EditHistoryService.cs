using Microsoft.EntityFrameworkCore;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services;
public class EditHistoryService : IEditHistoryService
{
    private IHttpContextService HttpContext { get; }

    public EditHistoryService(IHttpContextService httpContext)
    {
        HttpContext = httpContext;
    }

    public IQueryable<EditHistory> GetHistory(NotesDbContext db, int skip = 0, int take = 5)
    {
        return db.EditHistory
            .Include(h => h.Note)
            .Include(h => h.Project)
            .Where(h => h.EdittedById == HttpContext.UserId!)
            .OrderByDescending(h => h.Timestamp)
            .Skip(skip)
            .Take(take);
    }

    public async Task AddWebsiteEventAsync(NotesDbContext db)
    {
        await db.EditHistory.AddAsync(new()
        {
            EdittedById = HttpContext.UserId!,
            Timestamp = DateTimeOffset.Now,
        });

        await db.SaveChangesAsync();
    }

    public async Task AddProjectEventAsync(NotesDbContext db, string projectId)
    {
        await db.EditHistory.AddAsync(new()
        {
            EdittedById = HttpContext.UserId!,
            Timestamp = DateTimeOffset.Now,
            ProjectId = projectId
        });

        await db.SaveChangesAsync();
    }

    public async Task AddNoteEventAsync(NotesDbContext db, string noteId)
    {
        await db.EditHistory.AddAsync(new()
        {
            EdittedById = HttpContext.UserId!,
            Timestamp = DateTimeOffset.Now,
            NoteId = noteId
        });

        await db.SaveChangesAsync();
    }
}