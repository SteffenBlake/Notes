using Notes.Business.Models.Notes;
using Notes.Data;

namespace Notes.Business.Services.Abstractions;

public interface INoteService
{
    /// <summary>
    /// Indexes all the notes for a given project by project name
    /// </summary>
    bool TryIndex(NotesDbContext db, string projectName, out NoteIndexModel? indexModel);

    /// <summary>
    /// Reads a specific note's data from a project, by project name and unique path for that note
    /// </summary>
    bool TryGet(NotesDbContext db, string projectName, string path, out NoteReadModel? readModel);

    /// <summary>
    /// Adds or Updates a note for a project, by project name and unique note path
    /// </summary>
    bool TryPut(NotesDbContext db, string projectName, string path, NoteWriteModel writeModel, out NoteReadModel? readModel);
    
    /// <summary>
    /// Deletes a specific note for a project by project name and unique note path.
    /// If the note has children beneath it's path, it's contents will just be cleared instead
    /// </summary>
    bool TryDelete(NotesDbContext db, string projectName, string path);
}