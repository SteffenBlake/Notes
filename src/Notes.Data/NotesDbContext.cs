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

    public DbSet<Project> Projects { get; set; } = default!;
    public DbSet<Note> Notes { get; set; } = default!;
    public DbSet<NoteLink> NoteLinks { get; set; } = default!;
}
