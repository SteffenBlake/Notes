using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Notes.Business.Services.Abstractions;

namespace Notes.Business.Services;

public class HttpContextService : IHttpContextService
{
    private IHttpContextAccessor HttpContextAccessor { get; }

    public HttpContextService(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    public string? UserId => HttpContextAccessor
        .HttpContext?.User
        .FindFirstValue(ClaimTypes.NameIdentifier);
}