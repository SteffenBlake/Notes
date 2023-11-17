using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services.Abstractions;

public interface IEditHistoryService
{
    IQueryable<EditHistory> GetHistory(in NotesDbContext db, int skip = 0, int take = 5);

    void AddWebsiteEvent(in NotesDbContext db);

    void AddProjectEvent(in NotesDbContext db, string projectId);

    void AddNoteEvent(in NotesDbContext db, string noteId);
}