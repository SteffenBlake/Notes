using System.Configuration;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notes.Business.Configurations;
using Notes.Business.Extensions;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models.Identity;
using Notes.Website.Middleware;

namespace Notes.Website
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = EnsureConfig(builder);

            builder.Services.AddDbContext<NotesDbContext>(options =>
            {
                options.UseNpgsql(config.Database.ConnectionString);
            });

            EnsureIdentity(builder, config);

            builder.Services.AddControllers();

            if (builder.Environment.IsDevelopment())
            {
                EnsureSwagger(builder);
            }

            builder.Services.AddBusinessServices();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbInitService = scope.ServiceProvider.GetRequiredService<IDatabaseInitService>();
                await dbInitService.InitAsync();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/Notes/swagger.json", "Notes");
                });
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            var contentRoot = builder.Environment.ContentRootFileProvider;
            app.MapGet("/login", StaticPageMiddleware.Compile(contentRoot, "login.html")).AllowAnonymous();
            app.MapGet("/denied", StaticPageMiddleware.Compile(contentRoot, "denied.html")).AllowAnonymous();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            
            var pathGroup = app.MapGroup("/api/{**path}")
                .RequireAuthorization()
                .ExcludeFromDescription()
                .RequireAuthorization();

            pathGroup.MapGet("", 
                async (string path, IContentService svc) => await svc.GetAsync(path)
            );
            pathGroup.MapPut("",
                async (string path, [FromBody]string data, IContentService svc) => await svc.PutAsync(path, data)
            );

            app.MapFallback(StaticPageMiddleware.Compile(contentRoot, "index.html")).RequireAuthorization();

            await app.RunAsync();
        }
        
        private static NotesConfig EnsureConfig(WebApplicationBuilder builder)
        {
            var config = builder.Configuration.Get<NotesConfig>() ??
                         throw new ConfigurationErrorsException(
                             "Invalid Configuration, please validate configuration integrity"
                         );
            config.Validate();

            builder.Services.AddSingleton(config);
            return config;
        }

        private static void EnsureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("Notes", new()
                {
                    Description = "Open API Docs for Notes v1",
                    Title = "Notes",
                    Version = "v1",
                    Contact = new()
                    {
                        Email = "steffen@technically.fun",
                        Name = "Steffen Blake",
                        Url = new Uri("https://github.com/SteffenBlake/Notes"),
                    },
                    License = new ()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://raw.githubusercontent.com/SteffenBlake/Notes/main/LICENSE")
                    }
                });
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        }

        private static void EnsureIdentity(WebApplicationBuilder builder, NotesConfig config)
        {
            builder.Services
                .AddDefaultIdentity<NotesUser>(options =>
                {
                    options.Lockout = config.Identity.Lockout;
                    options.Password = config.Identity.Password;
                    options.SignIn = config.Identity.SignIn;
                    options.User = config.Identity.User;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<NotesDbContext>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/api/logout";
                    options.AccessDeniedPath = "/denied";
                    options.ExpireTimeSpan = config.Login.Expiry;
                });
        }
    }
}