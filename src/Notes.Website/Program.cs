using System.Configuration;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Notes.Business;
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Use(IndexPageMiddleware.Compile(builder.Environment.WebRootFileProvider));

            var pathGroup = app.MapGroup("/api/{**path}")
                .RequireAuthorization()
                .ExcludeFromDescription();

            pathGroup.MapGet("", 
                async (string path, IContentService svc) => await svc.GetAsync(path)
            );
            pathGroup.MapPut("",
                async (string path, [FromBody]string data, IContentService svc) => await svc.PutAsync(path, data)
            );

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
                options.AddSecurityDefinition("Bearer", new()
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    BearerFormat = "Bearer <token>",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        // scopes
                        new string[] { }
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

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = Encoding.UTF8.GetBytes(config.Signing.JWTSecret);
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = NotesConstants.JWT_ISSUER,
                        ValidAudience = config.Urls,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
        }
    }
}