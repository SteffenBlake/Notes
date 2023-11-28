using Notes.Business.Models.Directories;
using Notes.Business.Services;
using Notes.Business.Tests.Mocks;
using System.IO.Pipelines;

namespace Notes.Business.Tests.Tests;

public class DirectoryServiceTests : TestBase<DirectoryService, MockHttpContextService, MockEditHistoryService>
{
    private const string CurrentUser = nameof(CurrentUser);
    private const string NotCurrentUser = nameof(NotCurrentUser);

    private const string Project_HasNoNotes = nameof(Project_HasNoNotes);
    private const string Project_HasSomeNotes = nameof(Project_HasSomeNotes);

    private const string Note_HasNoChildren = nameof(Note_HasNoChildren);
    private const string Note_HasChildren = nameof(Note_HasChildren);
    private const string Note_IsChild = nameof(Note_IsChild);

    private const string Note_PrimaryParent = nameof(Note_PrimaryParent);
    private const string Note_PrimaryChild = nameof(Note_PrimaryChild);

    protected override void Build(IServiceProvider services)
    {
        base.Build(services);

        ServiceA!.UserId = CurrentUser;

        AddProject(Project_HasNoNotes);
        AddProject(Project_HasSomeNotes);

        AddNote(Project_HasSomeNotes, Note_HasNoChildren);

        AddNote(Project_HasSomeNotes, Note_HasChildren);
        AddNote(Project_HasSomeNotes, Note_IsChild, Note_HasChildren);

        AddNote(Project_HasSomeNotes, Note_PrimaryParent);
        AddNote(Project_HasSomeNotes, Note_PrimaryChild, Note_PrimaryParent);

        Db!.SaveChanges();
    }

    // Increment each addition by 1 hour to avoid time sort problems
    private int editCount;

    private void AddProject(string projectId, string userId = CurrentUser, DateTimeOffset? editted = null)
    {
        Db!.Projects.Add(new()
        {
            ProjectId = projectId,
            Description = projectId,
            Name = projectId,
            UserId = userId
        });

        Db.EditHistory.Add(new()
        {
            ProjectId = projectId,
            EdittedById = userId,
            Timestamp = editted ?? DateTimeOffset.Now.AddHours(editCount++)
        });
    }

    private void AddNote(
        string projectId,
        string noteId,
        string? parentId = null,
        string? content = null,
        string userId = CurrentUser,
        DateTimeOffset? editted = null
    )
    {
        Db!.Notes.Add(new()
        {
            ProjectId = projectId,
            NoteId = noteId,
            ParentNoteId = parentId,
            ContentRaw = content ?? noteId,
            HtmlContent = content ?? noteId,
            Name = noteId,
        });

        Db.EditHistory.Add(new()
        {
            NoteId = noteId,
            EdittedById = userId,
            Timestamp = editted ?? DateTimeOffset.Now.AddHours(editCount++)
        });
    }

    [Test]
    public async Task TryGetOverview_InvalidDirectoryId_Fails()
    {
        var (result, _, _, data) = await PrimaryService!.TryGetOverviewAsync(Db!, "DoesntExist");

        Assert.That(result, Is.False);

        Assert.That(data, Is.Null);
    }

    [Test]
    public async Task TryGetOverview_Home_ReturnsOnlyHomeAndProjects()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetOverviewAsync(Db!, "0");

        Assert.That(result, Is.True);

        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel.Primaries.Keys, Has.Count.EqualTo(0));
        Assert.That(readModel.Notes, Has.Count.EqualTo(0));

        Assert.That(readModel.Projects, Has.Count.EqualTo(2));

        Assert.That(readModel.Projects[0].Name, Is.EqualTo(Project_HasNoNotes));
        Assert.That(readModel.Projects[1].Name, Is.EqualTo(Project_HasSomeNotes));
    }


    [Test]
    public async Task TryGetOverview_ProjectNoNotes_ReturnsProjectsAndIsPrime()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetOverviewAsync(Db!, Project_HasNoNotes);

        Assert.That(result, Is.True);

        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel.Notes, Has.Count.EqualTo(0));

        Assert.That(readModel.Primaries.Keys, Has.Count.EqualTo(1));
        Assert.That(readModel.Primaries.ContainsKey(Project_HasNoNotes), Is.True);
        Assert.That(readModel.Primaries[Project_HasNoNotes].Name, Is.EqualTo(Project_HasNoNotes));
        Assert.That(readModel.Primaries[Project_HasNoNotes].Id, Is.EqualTo(Project_HasNoNotes));
        Assert.That(readModel.Primaries[Project_HasNoNotes].ParentId, Is.EqualTo("0"));
        Assert.That(readModel.Primaries[Project_HasNoNotes].Selected, Is.True);

        Assert.That(readModel.Projects, Has.Count.EqualTo(1));
        Assert.That(readModel.Projects[0].Name, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Projects[0].Id, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Projects[0].ParentId, Is.EqualTo("0"));
    }

    [Test]
    public async Task TryGetOverview_PrimaryParent_ReturnsPrimePathAndProjectAndChild()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetOverviewAsync(Db!, Note_PrimaryParent);

        Assert.That(result, Is.True);

        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel.Primaries.Keys, Has.Count.EqualTo(2));

        Assert.That(readModel.Primaries.ContainsKey(Project_HasSomeNotes), Is.True);
        Assert.That(readModel.Primaries[Project_HasSomeNotes].Name, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Primaries[Project_HasSomeNotes].Id, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Primaries[Project_HasSomeNotes].ParentId, Is.EqualTo("0"));
        Assert.That(readModel.Primaries[Project_HasSomeNotes].Selected, Is.False);

        Assert.That(readModel.Primaries.ContainsKey(Note_PrimaryParent), Is.True);
        Assert.That(readModel.Primaries[Note_PrimaryParent].Name, Is.EqualTo(Note_PrimaryParent));
        Assert.That(readModel.Primaries[Note_PrimaryParent].Id, Is.EqualTo(Note_PrimaryParent));
        Assert.That(readModel.Primaries[Note_PrimaryParent].ParentId, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Primaries[Note_PrimaryParent].Selected, Is.True);

        Assert.That(readModel.Notes, Has.Count.EqualTo(3));
        Assert.That(readModel.Notes, Has.One.Items.Matches<DirectoryReadModel>(m => 
            m.Name == Note_HasNoChildren
        ));
        Assert.That(readModel.Notes, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_HasChildren
        ));
        Assert.That(readModel.Notes, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_PrimaryChild
        ));

        Assert.That(readModel.Projects, Has.Count.EqualTo(1));
        Assert.That(readModel.Projects[0].Name, Is.EqualTo(Project_HasNoNotes));
        Assert.That(readModel.Projects[0].Id, Is.EqualTo(Project_HasNoNotes));
        Assert.That(readModel.Projects[0].ParentId, Is.EqualTo("0"));
    }

    [Test]
    public async Task TryGetOverview_Primarychild_ReturnsPrimePathAndProjectAndParent()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetOverviewAsync(Db!, Note_PrimaryChild);

        Assert.That(result, Is.True);

        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel.Primaries.Keys, Has.Count.EqualTo(3));

        Assert.That(readModel.Primaries.ContainsKey(Project_HasSomeNotes), Is.True);
        Assert.That(readModel.Primaries[Project_HasSomeNotes].Name, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Primaries[Project_HasSomeNotes].Id, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Primaries[Project_HasSomeNotes].ParentId, Is.EqualTo("0"));
        Assert.That(readModel.Primaries[Project_HasSomeNotes].Selected, Is.False);

        Assert.That(readModel.Primaries.ContainsKey(Note_PrimaryParent), Is.True);
        Assert.That(readModel.Primaries[Note_PrimaryParent].Name, Is.EqualTo(Note_PrimaryParent));
        Assert.That(readModel.Primaries[Note_PrimaryParent].Id, Is.EqualTo(Note_PrimaryParent));
        Assert.That(readModel.Primaries[Note_PrimaryParent].ParentId, Is.EqualTo(Project_HasSomeNotes));
        Assert.That(readModel.Primaries[Note_PrimaryParent].Selected, Is.False);

        Assert.That(readModel.Primaries.ContainsKey(Note_PrimaryChild), Is.True);
        Assert.That(readModel.Primaries[Note_PrimaryChild].Name, Is.EqualTo(Note_PrimaryChild));
        Assert.That(readModel.Primaries[Note_PrimaryChild].Id, Is.EqualTo(Note_PrimaryChild));
        Assert.That(readModel.Primaries[Note_PrimaryChild].ParentId, Is.EqualTo(Note_PrimaryParent));
        Assert.That(readModel.Primaries[Note_PrimaryChild].Selected, Is.True);

        Assert.That(readModel.Notes, Has.Count.EqualTo(2));
        Assert.That(readModel.Notes, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_HasNoChildren
        ));
        Assert.That(readModel.Notes, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_HasChildren
        ));

        Assert.That(readModel.Projects, Has.Count.EqualTo(1));
        Assert.That(readModel.Projects[0].Name, Is.EqualTo(Project_HasNoNotes));
        Assert.That(readModel.Projects[0].Id, Is.EqualTo(Project_HasNoNotes));
        Assert.That(readModel.Projects[0].ParentId, Is.EqualTo("0"));
    }

    [Test]
    public async Task TryGetDescendants_InvalidDir_Fails()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetDescendantsAsync(Db!, "DoesntExist");

        Assert.That(result, Is.False);

        Assert.That(readModel, Is.Null);
    }

    [Test]
    public async Task TryGetDescendants_EmptyProj_Fails()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetDescendantsAsync(Db!, Project_HasNoNotes);

        Assert.That(result, Is.False);
        Assert.That(readModel, Is.Null);
    }

    [Test]
    public async Task TryGetDescendants_LoadedProj_ReturnsNotes()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetDescendantsAsync(Db!, Project_HasSomeNotes);

        Assert.That(result, Is.True);

        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel.Descendants, Has.Count.EqualTo(3));
        Assert.That(readModel.Descendants, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_HasNoChildren
        ));
        Assert.That(readModel.Descendants, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_HasChildren
        ));
        Assert.That(readModel.Descendants, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_PrimaryParent
        ));
    }

    [Test]
    public async Task TryGetDescendants_EmptyNote_ReturnsEmpty()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetDescendantsAsync(Db!, Note_HasNoChildren);

        Assert.That(result, Is.False);
        Assert.That(readModel, Is.Null);
    }

    [Test]
    public async Task TryGetDescendants_LoadedNote_ReturnsNotes()
    {
        var (result, _, _, readModel) = await PrimaryService!.TryGetDescendantsAsync(Db!, Note_HasChildren);

        Assert.That(result, Is.True);
        Assert.That(readModel.Descendants, Has.Count.EqualTo(1));
        Assert.That(readModel.Descendants, Has.One.Items.Matches<DirectoryReadModel>(m =>
            m.Name == Note_IsChild
        ));
    }
}
