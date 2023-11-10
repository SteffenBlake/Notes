using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services.Abstractions;

public interface IEditHistoryService
{
    IQueryable<EditHistory> GetHistory(NotesDbContext db, int skip = 0, int take = 5);

    void AddWebsiteEvent(NotesDbContext db);

    void AddProjectEvent(NotesDbContext db, string projectId);

    void AddNoteEvent(NotesDbContext db, string noteId);
}