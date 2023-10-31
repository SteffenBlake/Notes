using Notes.Business.Services.Abstractions;

namespace Notes.Business.Tests.Mocks;
public class MockHttpContextService : IHttpContextService
{
    public string? UserId { get; set; }
}
