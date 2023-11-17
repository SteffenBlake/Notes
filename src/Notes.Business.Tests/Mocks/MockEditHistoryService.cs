using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Tests.Mocks;
public class MockEditHistoryService : IEditHistoryService
{
    public IQueryable<EditHistory> GetHistory(in NotesDbContext db, int skip = 0, int take = 5)
    {
        throw new NotImplementedException();
    }

    public int WebsiteEvents { get; private set; }
    public void AddWebsiteEvent(in NotesDbContext db) => WebsiteEvents++;

    public int ProjectEvents { get; private set; }
    public void AddProjectEvent(in NotesDbContext db, string projectId) => ProjectEvents++;

    public int NoteEvents { get; private set; }
    public void AddNoteEvent(in NotesDbContext db, string noteId) => NoteEvents++;
}
