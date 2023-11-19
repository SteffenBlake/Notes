using Notes.Business.Models.Notes;
using Notes.Data;

namespace Notes.Business.Services.Abstractions;

public interface INoteService
{
    /// <summary>
    /// Indexes all the notes for a given project by project name
    /// </summary>
    Task<TryResult<NoteIndexModel>> TryIndexAsync(NotesDbContext db, string projectName);

    /// <summary>
    /// Reads a specific note's data from a project, by project name and unique path for that note
    /// </summary>
    Task<TryResult<NoteReadModel>> TryGetAsync(NotesDbContext db, string projectName, string path);

    /// <summary>
    /// Adds or Updates a note for a project, by project name and unique note path
    /// </summary>
    Task<TryResult<NoteReadModel>> TryPutAsync(NotesDbContext db, string projectName, string path, NoteWriteModel writeModel);

    /// <summary>
    /// Deletes a specific note for a project by project name and unique note path.
    /// If the note has children beneath it's path, it's contents will just be cleared instead
    /// </summary>
    Task<TryResult<object>> TryDeleteAsync(NotesDbContext db, string projectName, string path);
}