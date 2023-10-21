using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Notes.Business.Configurations;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models.Identity;

namespace Notes.Business.Services
{
    public class DatabaseInitService : IDatabaseInitService
    {
        private NotesConfig Config { get; }
        private NotesDbContext DB { get; }
        private UserManager<NotesUser> UserManager { get; }
        private RoleManager<IdentityRole> RoleManager { get; }

        public DatabaseInitService(
            NotesConfig config, 
            NotesDbContext db, 
            UserManager<NotesUser> userManager, 
            RoleManager<IdentityRole> roleManager
        )
        {
            Config = config;
            DB = db;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public async Task InitAsync()
        {
            await DB.Database.MigrateAsync();

            var adminRole = await EnsureRoleAsync("Administrator");
            var admin = await EnsureUserAsync(Config.Login.UserName, Config.Login.Email, Config.Login.Password);

            await EnsureRolesAsync(admin, "Administrator");
        }

        private async Task<IdentityRole> EnsureRoleAsync(string roleName)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                return role;
            }

            await RoleManager.CreateAsync(new IdentityRole
            {
                Name = roleName
            });

            role = await RoleManager.FindByNameAsync(roleName);
            return role!;
        }

        private async Task<NotesUser> EnsureUserAsync(string userName, string email, string password)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null)
            {
                return user;
            }

            var result = await UserManager.CreateAsync(new NotesUser
            {
                UserName = userName,
                Email = email
            }, password);

            if (!result.Succeeded)
            {
                throw new AggregateException(
                    result.Errors.Select(e => new ValidationException(e.Description))
                );
            }

            user = await UserManager.FindByNameAsync(userName);
            return user!;
        }

        private async Task EnsureRolesAsync(NotesUser user, params string[] roles)
        {
            var existingRoles = await UserManager.GetRolesAsync(user);
            var missing = roles.Except(existingRoles);
            await UserManager.AddToRolesAsync(user, missing);
        }
    }
}
