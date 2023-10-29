using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Notes.Business.Configurations;
using Notes.Business.Services;
using Notes.Data;

namespace Notes.Business.Tests;

public static class Engine
{
    public static IServiceProvider Compile(NotesConfig? config = null)
    {
        var services = new ServiceCollection();

        services.AddSingleton(config ?? new NotesConfig());

        var dbOptionsBuilder = new DbContextOptionsBuilder<NotesDbContext>()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString(), 
                options => options.EnableNullChecks()
        );
        services.AddSingleton(new NotesDbContext(dbOptionsBuilder.Options));

        services.AddSingleton<NoteService>();
        services.AddSingleton<ProjectService>();

        return services.BuildServiceProvider();
    }

    private static void RegisterMock<TService, TImplementation>(this IServiceCollection services) 
        where TImplementation : class, TService, new() 
        where TService : class
    {
        var service = new TImplementation();
        services.AddSingleton(service);
        services.AddSingleton<TService, TImplementation>(s => s.GetRequiredService<TImplementation>());
    }

}