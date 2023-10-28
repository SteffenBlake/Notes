using Notes.Business.Services.Abstractions;

namespace Notes.Business.Services
{
    public class NoteService : INoteService
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
