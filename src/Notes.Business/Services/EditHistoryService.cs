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

    public IQueryable<EditHistory> GetHistory(in NotesDbContext db, int skip = 0, int take = 5)
    {
        return db.EditHistory
            .Include(h => h.Note)
            .Include(h => h.Project)
            .Where(h => h.EdittedById == HttpContext.UserId!)
            .OrderByDescending(h => h.Timestamp)
            .Skip(skip)
            .Take(take);
    }

    public void AddWebsiteEvent(in NotesDbContext db)
    {
        db.EditHistory.Add(new()
        {
            EdittedById = HttpContext.UserId!,
            Timestamp = DateTimeOffset.Now,
        });

        db.SaveChanges();
    }

    public void AddProjectEvent(in NotesDbContext db, string projectId)
    {
        db.EditHistory.Add(new()
        {
            EdittedById = HttpContext.UserId!,
            Timestamp = DateTimeOffset.Now,
            ProjectId = projectId
        });

        db.SaveChanges();
    }

    public void AddNoteEvent(in NotesDbContext db, string noteId)
    {
        db.EditHistory.Add(new()
        {
            EdittedById = HttpContext.UserId!,
            Timestamp = DateTimeOffset.Now,
            NoteId = noteId
        });

        db.SaveChanges();
    }
}