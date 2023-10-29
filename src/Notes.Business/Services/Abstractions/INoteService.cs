using Notes.Business.Models.Notes;
using Notes.Business.Services.Models.Notes;
using Notes.Data;

namespace Notes.Business.Services.Abstractions;

public interface INoteService
{
    bool TryGet(NotesDbContext db, string projectName, string path, out NoteReadModel? readModel);
    bool TryPut(NotesDbContext db, string projectId, string path, NoteWriteModel writeModel, out NoteReadModel? readModel);
}