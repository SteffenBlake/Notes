using Notes.Business.Models.Projects;
using Notes.Business.Services;
using Notes.Business.Tests.Mocks;
using Notes.Data.Models;

namespace Notes.Business.Tests.Tests;
internal class ProjectServiceTests : TestBase<ProjectService, MockHttpContextService, MockEditHistoryService>
{
    private const string ExistingUserId = nameof(ExistingUserId);

    private readonly Project Project_NoNotes = new()
    {
        Name = nameof(Project_NoNotes),
        ProjectId = nameof(Project_NoNotes),
        UserId = ExistingUserId
    };
    private readonly Project Project_WithNotes = new()
    {
        Name = nameof(Project_WithNotes),
        ProjectId = nameof(Project_WithNotes),
        UserId = ExistingUserId
    };
    private readonly Project Project_NotOwned = new()
    {
        Name = nameof(Project_NotOwned),
        ProjectId = nameof(Project_NotOwned),
        UserId = "SomeOtherUser"
    };

    private readonly Note Some_Note = new()
    {
        Name = nameof(Some_Note),
        NoteId = nameof(Some_Note),
        ProjectId = nameof(Project_WithNotes)
    };

    protected override void Build(IServiceProvider services)
    {
        base.Build(services);

        ServiceA!.UserId = ExistingUserId;

        Db!.Projects.Add(Project_NoNotes);
        Db.Projects.Add(Project_WithNotes);
        Db.Projects.Add(Project_NotOwned);
        Db.Notes.Add(Some_Note);

        Db.SaveChanges();
    }

    [Test]
    public async Task Index_Works()
    {
        var (result, _, statusCode, data) = await PrimaryService!.TryIndexAsync(Db!);

        Assert.That(statusCode, Is.EqualTo(200));
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Data, Has.Count.EqualTo(2));
        Assert.That(data.Data, Has.One.Matches<ProjectReadModel>(m => m.Name == nameof(Project_NoNotes)));
        Assert.That(data.Data, Has.One.Matches<ProjectReadModel>(m => m.Name == nameof(Project_WithNotes)));
    }

    [Test]
    public async Task Put_NewProject_GetsAdded()
    {
        var writeModel = new ProjectWriteModel();
        var (result, _, _, readModel) = await PrimaryService!.TryPutAsync(Db!, nameof(Put_NewProject_GetsAdded), writeModel);
        var added = Db!.Projects.SingleOrDefault(p => p.Name == nameof(Put_NewProject_GetsAdded));

        Assert.That(result, Is.True);
        Assert.That(readModel!.Name, Is.EqualTo(nameof(Put_NewProject_GetsAdded)));
        Assert.That(readModel.CanDelete, Is.True);
        Assert.That(added, Is.Not.Null);
        Assert.That(added!.UserId, Is.EqualTo(ExistingUserId));

        Assert.That(ServiceB!.ProjectEvents, Is.EqualTo(1));
    }

    [Test]
    public async Task Put_ExistingProject_GetsUpdated()
    {
        var writeModel = new ProjectWriteModel();
        var (result, _, _, readModel) = await PrimaryService!.TryPutAsync(Db!, nameof(Project_WithNotes), writeModel);
        var added = Db!.Projects.SingleOrDefault(p => p.Name == nameof(Project_WithNotes));

        Assert.That(result, Is.True);
        Assert.That(readModel!.Name, Is.EqualTo(nameof(Project_WithNotes)));
        Assert.That(added, Is.Not.Null);

        Assert.That(ServiceB!.ProjectEvents, Is.EqualTo(1));
    }

    [Test]
    public async Task Get_UnownedProject_Fails()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetAsync(Db!, nameof(Project_NotOwned));

        Assert.That(result, Is.False);
        Assert.That(readModel, Is.Null);
    }

    [Test]
    public async Task Get_NonExistingProject_Fails()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetAsync(Db!, "DoesntExist");

        Assert.That(result, Is.False);
        Assert.That(readModel, Is.Null);
    }


    public static object[] Get_OwnedProject_CorrectData_Cases =
    {
        new object?[] { nameof(Project_NoNotes), true },
        new object?[] { nameof(Project_WithNotes), false },
    };

    [Test]
    [TestCaseSource(nameof(Get_OwnedProject_CorrectData_Cases))]
    public async Task Get_OwnedProject_CorrectData(string projectName, bool expectedCanDelete)
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetAsync(Db!, projectName);

        Assert.That(result, Is.True);
        Assert.That(readModel, Is.Not.Null);

        Assert.That(readModel!.ProjectId, Is.EqualTo(projectName));
        Assert.That(readModel!.Name, Is.EqualTo(projectName));
        Assert.That(readModel!.CanDelete, Is.EqualTo(expectedCanDelete));
    }

    [Test]
    public async Task Delete_UnownedProject_Fails()
    {
        var (result, _, _, _) = await PrimaryService!.TryDeleteAsync(Db!, nameof(Project_NotOwned));
        var existing = Db!.Projects.SingleOrDefault(p => p.ProjectId == nameof(Project_NotOwned));

        Assert.That(result, Is.False);
        Assert.That(existing, Is.Not.Null);
    }

    [Test]
    public async Task Delete_NonExistingProject_Fails()
    {
        var (result, _, _, _) = await PrimaryService!.TryDeleteAsync(Db!, "DoesntExist");
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task Delete_HasNotes_Throws()
    {
        var (result, _, _, _) = await PrimaryService!.TryDeleteAsync(Db!, nameof(Project_WithNotes));

        Assert.That(result, Is.False);

        var existing = Db!.Projects.SingleOrDefault(p => p.ProjectId == nameof(Project_WithNotes));
        Assert.That(existing, Is.Not.Null);
    }

    [Test]
    public async Task Delete_EmptyProject_Succeeds()
    {
        var (result, _, _, _) = await PrimaryService!.TryDeleteAsync(Db!, nameof(Project_NoNotes));
        var existing = Db!.Projects.SingleOrDefault(p => p.ProjectId == nameof(Project_NoNotes));

        Assert.That(result, Is.True);
        Assert.That(existing, Is.Null);
    }
}
