using System.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Notes.Business;
using Notes.Business.Configurations;
using Notes.Business.Extensions;
using Notes.Business.Services.Abstractions;
using Notes.Data;
using Notes.Data.Models.Identity;

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

            app.UseAuthentication();
            app.UseAuthorization();

            var cspBuilder = new StringBuilder();
            cspBuilder.Append("default-src 'none';");
            cspBuilder.Append("img-src 'self';");
            cspBuilder.Append("font-src 'self';");
            cspBuilder.Append($"script-src-elem {string.Join(' ',config.Signing.JsHashes)};");
            cspBuilder.Append($"style-src-elem {string.Join(' ',config.Signing.CssHashes)};");
            var cspHeader = cspBuilder.ToString();

            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    var requestPath = context.Request.Path.Value;

                    if (requestPath == "/index.html")
                    {
                        context.Response.Headers.Add("Content-Security-Policy", cspHeader);
                    }

                    return Task.FromResult(0);
                });

                await next();
            });

            app.UseStaticFiles();

            app.MapControllers();
            app.MapFallbackToFile("index.html");

            await app.RunAsync();
        }
        
        private static NotesConfig EnsureConfig(WebApplicationBuilder builder)
        {
            var config = builder.Configuration.Get<NotesConfig>() ??
                         throw new ConfigurationErrorsException(
                             "Invalid Configuration, please validate configuration integrity"
                         );
            config.Validate();

            var dir = builder.Environment.WebRootFileProvider.GetDirectoryContents("");
            foreach (var file in dir)
            {
                if (file.Name.EndsWith(".js"))
                {
                    config.Signing.JsHashes.Add(ComputeFileHash(file));
                }
                else if (file.Name.EndsWith(".css"))
                {
                    config.Signing.CssHashes.Add(ComputeFileHash(file));
                }
            }

            builder.Services.AddSingleton(config);
            return config;
        }

        private static string ComputeFileHash(IFileInfo file)
        {
            using var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            using var sha384 = SHA384.Create();

            var text = reader.ReadToEnd();
            var utf = Encoding.UTF8.GetBytes(text);
            var hash = sha384.ComputeHash(utf);
            return "'sha384-" + Convert.ToBase64String(hash) + "'";
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