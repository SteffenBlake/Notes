﻿using Microsoft.Extensions.DependencyInjection;
using Notes.Business.Services;
using Notes.Business.Services.Abstractions;

namespace Notes.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddTransient<IDatabaseInitService, DatabaseInitService>();
        services.AddTransient<INoteService, NoteService>();
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<IDirectoryService, DirectoryService>();
        services.AddTransient<IEditHistoryService, EditHistoryService>();
        services.AddTransient<IHttpContextService, HttpContextService>();
    }
}