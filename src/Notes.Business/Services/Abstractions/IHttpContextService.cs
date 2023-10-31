namespace Notes.Business.Services.Abstractions;

public interface IHttpContextService
{
    string? UserId { get; }
}