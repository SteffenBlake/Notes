using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.Data.Models.Identity;

namespace Notes.Data
{
    public class NotesDbContext : IdentityDbContext<NotesUser>
    {
        public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options) {}
    }
}
