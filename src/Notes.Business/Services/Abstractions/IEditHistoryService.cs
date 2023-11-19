using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services.Abstractions;

public interface IEditHistoryService
{
    IQueryable<EditHistory> GetHistory(NotesDbContext db, int skip = 0, int take = 5);

    Task AddWebsiteEventAsync(NotesDbContext db);

    Task AddProjectEventAsync(NotesDbContext db, string projectId);

    Task AddNoteEventAsync(NotesDbContext db, string noteId);
}