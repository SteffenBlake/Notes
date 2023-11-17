using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.Data.Models;
using Notes.Data.Models.Identity;

namespace Notes.Data;

public class NotesDbContext : IdentityDbContext<NotesUser>
{
    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }

    public required DbSet<WebsiteConfiguration> WebsiteConfiguration { get; init; }
    public required DbSet<Project> Projects { get; init; }
    public required DbSet<Note> Notes { get; init; }
    public required DbSet<NoteLink> NoteLinks { get; init; }
    public required DbSet<EditHistory> EditHistory { get; init; }
}
