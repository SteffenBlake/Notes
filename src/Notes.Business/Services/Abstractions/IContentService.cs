namespace Notes.Business.Services.Abstractions;

public interface IContentService
{
    Task<string> GetAsync(string path);
    Task PutAsync(string path, string data);
}