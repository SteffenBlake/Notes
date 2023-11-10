using System.Linq.Expressions;
using Notes.Data;
using Notes.Data.Models;

namespace Notes.Business.Models.Directories;

public class DirectoryRecentReadModel
{
    public List<DirectoryReadModel> Recent { get; set; } = new();
}

public class DirectoryOverviewReadModel
{
    public Dictionary<string, DirectoryReadModel> Primaries { get; set; } = new();
    public List<DirectoryReadModel> Projects { get; set; } = new();
    public List<DirectoryReadModel> Notes { get; set; } = new();
}

public class DirectoryDescendantsReadModel
{
    public List<DirectoryReadModel> Descendants { get; set; } = new();
}

public class DirectoryReadModel
{
    public required string Id { get; set; }

    public required string ParentId { get; set; }

    public required string Name { get; set; }

    public required bool HasContent { get; set; }

    public required bool HasChildren { get; set; }

    public required string Icon { get; set; }

    public required DateTimeOffset Timestamp { get; set; }

    public bool Selected { get; set; }

    public required string Route { get; set; }

    public static Expression<Func<EditHistory, DirectoryReadModel>> Read(NotesDbContext db)
    {
        return (h) => new DirectoryReadModel
        {
            Id = h.ProjectId ?? h.NoteId ?? "0",
            Name = 
                h.Project != null ? h.Project.Name :
                h.Note != null ? h.Note.Name :
                "Home",
            HasContent = h.Project == null && h.Note != null && !string.IsNullOrEmpty(h.Note.ContentRaw),
            Icon =
                h.Project != null ? h.Project.Icon :
                h.Note != null ? h.Note.Icon :
                "house",
            Timestamp = h.Timestamp,
            ParentId =
                h.Project != null ? "0" :
                h.Note != null ? h.Note.ParentNoteId ?? h.Note.ProjectId :
                "",
            HasChildren = h.ProjectId != null ? db.Notes.Any(n => n.ProjectId == h.ProjectId) :
                h.NoteId == null || 
                db.Notes.Any(n => n.ParentNoteId == h.NoteId),
            Selected = false,
            Route = h.Project != null ? "/" + h.Project.Name :
                h.Note != null ? h.Note.Route : "/"
        };
    }
}