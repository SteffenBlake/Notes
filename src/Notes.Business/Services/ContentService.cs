using Notes.Business.Services.Abstractions;

namespace Notes.Business.Services
{
    public class ContentService : IContentService
    {
        public Task<string> GetAsync(string path)
        {
            return Task.FromResult("Hello World!");
        }

        public Task PutAsync(string path, string data)
        {
            return Task.CompletedTask;
        }
    }
}
