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
    public void AddWebsiteEvent(NotesDbContext db) => WebsiteEvents++;

    public int ProjectEvents { get; private set; }
    public void AddProjectEvent(NotesDbContext db, string projectId) => ProjectEvents++;

    public int NoteEvents { get; private set; }
    public void AddNoteEvent(NotesDbContext db, string noteId) => NoteEvents++;
}
