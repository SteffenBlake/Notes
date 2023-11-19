using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Tests.Mocks;
public class MockEditHistoryService : IEditHistoryService
{
    public IQueryable<EditHistory> GetHistory(NotesDbContext db, int skip = 0, int take = 5)
    {
        throw new NotImplementedException();
    }

    public int WebsiteEvents { get; private set; }
    public Task AddWebsiteEventAsync(NotesDbContext db)
    {
        WebsiteEvents++;
        return Task.CompletedTask;
    }

    public int ProjectEvents { get; private set; }
    public Task AddProjectEventAsync(NotesDbContext db, string projectId)
    {
        ProjectEvents++;
        return Task.CompletedTask;
    }

    public int NoteEvents { get; private set; }
    public Task AddNoteEventAsync(NotesDbContext db, string noteId)
    {
        NoteEvents++;
        return Task.CompletedTask;
    }
}
