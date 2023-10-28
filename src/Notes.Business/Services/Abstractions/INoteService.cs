namespace Notes.Business.Services.Abstractions;

public interface INoteService
{
    Task<string> GetAsync(string path);
    Task PutAsync(string path, string data);
}