using Microsoft.EntityFrameworkCore;
using Notes.Business.Models.Notes;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Services;

/// <inheritdoc />
public class NoteService : INoteService
{
    private IHttpContextService HttpContext { get; }

    private IEditHistoryService EditHistory { get; }

    public NoteService(IHttpContextService httpContext, IEditHistoryService editHistory)
    {
        HttpContext = httpContext;
        EditHistory = editHistory;
    }

    /// <inheritdoc />
    public async Task<TryResult<NoteIndexModel>> TryIndexAsync(NotesDbContext db, string projectName)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }

        var project = await db.Projects.SingleOrDefaultAsync(p => 
            p.Name == projectName &&
            p.UserId == HttpContext.UserId
        );
        if (project == null)
        {
            return TryResult<NoteIndexModel>.NotFound();
        }

        var data = await db.Notes.Where(n => n.Project.Name == projectName)
            .Select(NoteReadModel.FromModel(db))
            .ToListAsync();

        var indexModel = new NoteIndexModel
        {
            Data = data
        };

        return TryResult<NoteIndexModel>.Succeed(indexModel);
    }

    /// <inheritdoc />
    public async Task<TryResult<NoteReadModel>> TryGetAsync(NotesDbContext db, string projectName, string path)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var (success, heirarchy, segments) = await TryGetInternalAsync(db, projectName, path);

        Note ? note;
        if (!success || (note = heirarchy[^1]) == null)
        {
            return TryResult<NoteReadModel>.NotFound();
        }

        var readModel = NoteReadModel.FromModel(db).Compile()(note);
        return TryResult<NoteReadModel>.Succeed(readModel);
    }

    /// <inheritdoc />
    public async Task<TryResult<NoteReadModel>> TryPutAsync(NotesDbContext db, string projectName, string path, NoteWriteModel writeModel)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        var user = db.Users.Find(HttpContext.UserId)!;

        var projectId = await db.Projects.Where(p => 
                p.Name == projectName &&
                p.UserId == HttpContext.UserId
            )
            .Select(p => p.ProjectId)
            .SingleOrDefaultAsync();

        if (string.IsNullOrEmpty(projectId))
        {
            return TryResult<NoteReadModel>.NotFound();
        }

        var (success, heirarchy, segments) = await TryGetInternalAsync(db, projectName, path);

        if (!success || heirarchy.LastOrDefault() == null)
        {
            for (var i = 0; i < heirarchy.Length; i++)
            {
                var route = $"/{projectName}/{string.Join('/', segments[..(i+1)])}";
                heirarchy[i] ??= db.Notes.Add(new Note
                {
                    Name = segments[i],
                    ProjectId = projectId,
                    Route = route
                }).Entity;

                if (i == 0)
                {
                    continue;
                }
                heirarchy[i]!.ParentNoteId ??= heirarchy[i -1]!.NoteId;
            }
        }

        writeModel.Write(db, user, heirarchy[^1]!);

        db.SaveChanges();

        await EditHistory.AddNoteEventAsync(db, heirarchy[^1]!.NoteId);

        return await TryGetAsync(db, projectName, path);
    }
    
    /// <inheritdoc />
    public async Task<TryResult<object>> TryDeleteAsync(NotesDbContext db, string projectName, string path)
    {
        if (string.IsNullOrEmpty(projectName))
        {
            throw new ArgumentNullException(nameof(projectName));
        }
        if (string.IsNullOrEmpty(path))
        {
            throw new ArgumentNullException(nameof(path));
        }

        Note? note;
        var (success, heirarchy, _) = await TryGetInternalAsync(db, projectName, path);
        if (!success || (note = heirarchy[^1]) == null)
        {
            return TryResult<object>.Gone();
        }

        if (db.Notes.Any(n => n.ParentNoteId == note.NoteId))
        {
            note.ContentRaw = null;
            note.HtmlContent = null;
        }
        else
        {
            db.Remove(note);
        }

        db.SaveChanges();
        return TryResult<object>.Succeed(new { });
    }

    private readonly record struct TryGetResult(bool success, Note?[] heirarchy, string[] segments);
    private async Task<TryGetResult> TryGetInternalAsync(NotesDbContext db, string projectName, string path)
    {
        // this seems to be the cleanest way to achieve our desired result
        // of splitting up a uri path into segments
        var segmentsInternal = new Uri("http://a.b/" + path)
            .Segments
            .Select(s => s.Replace('/', ' ').Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        var segments = segmentsInternal.ToArray();

        var heirarchy = new Note[segmentsInternal.Count];

        var baseQuery = await db.Notes.Where(n =>
            n.Project.Name == projectName &&
            n.Project.UserId == HttpContext.UserId &&
            segmentsInternal.Contains(n.Name)
        ).ToDictionaryAsync(n => n.NoteId, n => n);

        if (!baseQuery.Any())
        {
            return new(false, heirarchy, segments);
        }

        // Get the matching root note
        var note = baseQuery.Single(n => n.Value.ParentNoteId == null && n.Value.Name == segmentsInternal[0]).Value;
        if (note == null)
        {
            return new (false, heirarchy, segments);
        }

        heirarchy[0] = note;

        for (var i = 1; i < segmentsInternal.Count; i++)
        {
            var noteId = note.NoteId;
            note = baseQuery.SingleOrDefault(n => n.Value.ParentNoteId == noteId && n.Value.Name == segmentsInternal[i]).Value;
            if (note == null)
            {
                return new (false, heirarchy, segments);
            }

            heirarchy[i] = note;
        }

        return new (true, heirarchy, segments);
    }
}