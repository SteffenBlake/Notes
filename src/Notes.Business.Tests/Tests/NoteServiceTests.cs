using Notes.Business.Models.Notes;
using Notes.Business.Services;
using Notes.Business.Tests.Mocks;
using Notes.Data.Models;

namespace Notes.Business.Tests.Tests;

[TestFixture]
public class NoteServiceTests : TestBase<NoteService, MockHttpContextService>
{
    private const string ProjectName = nameof(ProjectName);
    private const string ExistingUserId = nameof(ExistingUserId);

    protected override void Build(IServiceProvider services)
    {
        base.Build(services);

        Db!.Projects.Add(new Project
        {
            ProjectId = "1",
            Name = ProjectName,
            UserId = ExistingUserId
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "1",
            Name = "foo",
            ProjectId = "1",
            HtmlContent = "foo",
            ContentRaw = "foo"
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "2",
            Name = "bar",
            ProjectId = "1",
            HtmlContent = "foo/bar",
            ContentRaw = "foo/bar",
            ParentNoteId = "1",
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "3",
            Name = "phi",
            ProjectId = "1",
            HtmlContent = "foo/bar/phi",
            ContentRaw = "foo/bar/phi",
            ParentNoteId = "2"
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "4",
            Name = "phi",
            ProjectId = "1",
            HtmlContent = "foo/phi",
            ContentRaw = "foo/phi",
            ParentNoteId = "1"
        });
        Db.SaveChanges();

        ServiceA!.UserId = ExistingUserId;
    }


    [Test]
    public void Index_EmptyProject_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            PrimaryService!.TryIndex(Db!, "", out _);
        });
    }

    [TestCase("DoesntExist")]
    public void Index_InvalidProject_ReturnsFail(string projectName)
    {
        var result = PrimaryService!.TryIndex(Db!, projectName, out var indexModel);

        Assert.That(result, Is.False);
        Assert.That(indexModel, Is.Null);
    }

    [TestCase(ProjectName)]
    public void Index_ValidProject_ReturnsSuccess(string projectName)
    {
        var result = PrimaryService!.TryIndex(Db!, projectName, out var indexModel);

        Assert.That(result, Is.True);
        Assert.That(indexModel, Is.Not.Null);
        Assert.That(indexModel!.Data, Has.Count.EqualTo(4));
    }

    [TestCase("", "")]
    [TestCase("name", "")]
    [TestCase("", "name")]
    public void Put_EmptyStrings_Throws(string projectName, string path)
    {
        var writeModel = new NoteWriteModel
        {
            HtmlContent = "foo/foo",
            ContentRaw = "foo/foo"
        };

        Assert.Throws<ArgumentNullException>(() =>
        {
            PrimaryService!.TryPut(Db!, projectName, path, writeModel, out _);
        });
    }

    [TestCase("DoesntExist")]
    public void Put_InvalidProject_ReturnsFail(string projectName)
    {
        var writeModel = new NoteWriteModel
        {
            HtmlContent = "foo/foo",
            ContentRaw = "foo/foo"
        };

        var result = PrimaryService!.TryPut(Db!, projectName, "foo/foo", writeModel, out var readModel);
        Assert.That(result, Is.False);
        Assert.That(readModel, Is.Null);
    }

    public static object[] Put_ExistingPath_Idemptotent_Cases =
    {
        new object?[] { "foo", "1", "foo", null, "foo", "foo" },
        new object?[] { "foo/bar", "2", "bar", "1", "newRaw", "newHtml" },
        new object?[] { "foo/bar/phi", "3", "phi", "2", "foo/bar/phi", "foo/bar/phi" },
        new object?[] { "foo/phi", "4", "phi", "1", "foo/phi", "foo/phi" }
    };

    [Test]
    [TestCaseSource(nameof(Put_ExistingPath_Idemptotent_Cases))]
    public void Put_ExistingPath_Idemptotent(
        string path, 
        string checkId, 
        string expectedName, 
        string expectedParentId,
        string expectedRaw, 
        string expectedHtml
    )
    {
        var writeModel = new NoteWriteModel
        {
            ContentRaw = "newRaw",
            HtmlContent = "newHtml"
        };

        var result = PrimaryService!.TryPut(Db!, ProjectName, "foo/bar", writeModel, out _);
        var checkedNote = Db!.Notes.Find(checkId)!;

        Assert.That(result, Is.True);
        
        Assert.That(checkedNote.ParentNoteId, Is.EqualTo(expectedParentId));
        Assert.That(checkedNote.Name, Is.EqualTo(expectedName));
        Assert.That(checkedNote.ContentRaw, Is.EqualTo(expectedRaw));
        Assert.That(checkedNote.HtmlContent, Is.EqualTo(expectedHtml));
    }

    [Test]
    public void Put_OrphanPath_ConnectsHeirarchy()
    {
        var writeModel = new NoteWriteModel
        {
            ContentRaw = "newRaw",
            HtmlContent = "newHtml"
        };

        var result = PrimaryService!.TryPut(Db!, ProjectName, "foo/alpha/beta", writeModel, out _);
        var alpha = Db!.Notes.SingleOrDefault(n => n.Name == "alpha");
        var beta = Db.Notes.SingleOrDefault(n => n.Name == "beta");

        Assert.That(result, Is.True);

        Assert.That(alpha, Is.Not.Null);
        Assert.That(alpha!.ParentNoteId, Is.EqualTo("1"));
        Assert.That(alpha.ProjectId, Is.EqualTo("1"));
        Assert.That(alpha.ContentRaw, Is.Null);
        Assert.That(alpha.HtmlContent, Is.Null);

        Assert.That(beta, Is.Not.Null);
        Assert.That(beta!.ParentNoteId, Is.EqualTo(alpha.NoteId));
        Assert.That(beta.ProjectId, Is.EqualTo("1"));
        Assert.That(beta.ContentRaw, Is.EqualTo("newRaw"));
        Assert.That(beta.HtmlContent, Is.EqualTo("newHtml"));
    }

    [TestCase("", "")]
    [TestCase("doesntexist", "")]
    [TestCase("", "doesntexist")]
    [TestCase("", "foo")]
    [TestCase(ProjectName, "")]
    public void TryGet_EmptyInputs_Throws(string projectName, string path)
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            PrimaryService!.TryGet(Db!, projectName, path, out _);
        });
    }

    [TestCase("doesntexist", "foo")]
    [TestCase(ProjectName, "doesntexist")]
    public void TryGet_InvalidInputs_Fail(string projectName, string path)
    {
        var result = PrimaryService!.TryGet(Db!, projectName, path, out var readModel);
        Assert.That(result, Is.False);
        Assert.That(readModel, Is.Null);
    }

    public static object[] TryGet_ValidInput_MatchingOutput_Cases =
    {
        new object?[] { "foo", "1", null, "foo", "foo" },
        new object?[] { "foo/bar", "2", "1", "foo/bar", "foo/bar" },
        new object?[] { "foo/bar/phi", "3", "2", "foo/bar/phi", "foo/bar/phi" },
        new object?[] { "foo/phi", "4", "1", "foo/phi", "foo/phi" }
    };
    [Test]
    [TestCaseSource(nameof(TryGet_ValidInput_MatchingOutput_Cases))]
    public void TryGet_ValidInput_MatchingOutput(
        string path, 
        string expectedId,
        string expectedParentId,
        string expectedHtml,
        string expectedRaw
    )
    {
        var result = PrimaryService!.TryGet(Db!, ProjectName, path, out var readModel);

        Assert.That(result, Is.True);
        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel!.Id, Is.EqualTo(expectedId));
        Assert.That(readModel.ParentId, Is.EqualTo(expectedParentId));
        Assert.That(readModel.HtmlContent, Is.EqualTo(expectedHtml));
        Assert.That(readModel.ContentRaw, Is.EqualTo(expectedRaw));
    }

    [TestCase("", "")]
    [TestCase("doesntexist", "")]
    [TestCase("", "doesntexist")]
    [TestCase("", "foo")]
    [TestCase(ProjectName, "")]
    public void TryDelete_EmptyInputs_Throws(string projectName, string path)
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            PrimaryService!.TryDelete(Db!, projectName, path);
        });
    }

    [TestCase("doesntexist", "foo")]
    [TestCase(ProjectName, "doesntexist")]
    public void TryDelete_InvalidInputs_Fail(string projectName, string path)
    {
        var result = PrimaryService!.TryDelete(Db!, projectName, path);
        Assert.That(result, Is.False);
    }

    [Test]
    public void TryDelete_ValidLeaf_Deletes()
    {
        var result = PrimaryService!.TryDelete(Db!, ProjectName, "foo/bar/phi");
        var foo_bar_phi = Db!.Notes.Find("3");

        Assert.That(result, Is.True);
        Assert.That(foo_bar_phi, Is.Null);
    }

    public static object[] TryDelete_ValidBranch_Clears_Cases =
    {
        new object?[] { "foo", "1", "foo", null, "foo", "foo" },
        new object?[] { "foo/bar", "2", "bar", "1", null, null },
        new object?[] { "foo/bar/phi", "3", "phi", "2", "foo/bar/phi", "foo/bar/phi" },
        new object?[] { "foo/phi", "4", "phi", "1", "foo/phi", "foo/phi" }
    };
    [Test]
    [TestCaseSource(nameof(TryDelete_ValidBranch_Clears_Cases))]
    public void TryDelete_ValidBranch_Clears(
        string path,
        string checkId,
        string expectedName,
        string expectedParentId,
        string expectedHtml,
        string expectedRaw
    )
    {
        var result = PrimaryService!.TryDelete(Db!, ProjectName, "foo/bar");
        var checkNote = Db!.Notes.Find(checkId);

        Assert.That(result, Is.True);
        Assert.That(checkNote, Is.Not.Null);
        Assert.That(checkNote!.Name, Is.EqualTo(expectedName));
        Assert.That(checkNote.ParentNoteId, Is.EqualTo(expectedParentId));
        Assert.That(checkNote.HtmlContent, Is.EqualTo(expectedHtml));
        Assert.That(checkNote.ContentRaw, Is.EqualTo(expectedRaw));
    }
}