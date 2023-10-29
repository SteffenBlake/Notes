using Notes.Business.Services;
using Notes.Data.Models;

namespace Notes.Business.Tests.Tests;

[TestFixture]
public class NoteServiceTest : TestBase<NoteService>
{
    protected override void Build(IServiceProvider services)
    {
        base.Build(services);

        Db!.Projects.Add(new Project
        {
            ProjectId = "1",
            Name = "project",
            UserId = "1"
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "1",
            Name = "foo",
            ProjectId = "1",
            HtmlContent = "foo"
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "2",
            Name = "bar",
            ProjectId = "1",
            HtmlContent = "foo/bar",
            ParentNoteId = "1"
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "3",
            Name = "phi",
            ProjectId = "1",
            HtmlContent = "foo/bar/phi",
            ParentNoteId = "2"
        });
        Db!.Notes.Add(new Note
        {
            NoteId = "4",
            Name = "phi",
            ProjectId = "1",
            HtmlContent = "foo/phi",
            ParentNoteId = "1"
        });
        Db.SaveChanges();
    }

    [TestCase("foo", ExpectedResult = "foo")]
    [TestCase("foo/bar", ExpectedResult = "foo/bar")]
    [TestCase("foo/bar/phi", ExpectedResult = "foo/bar/phi")]
    [TestCase("foo/phi", ExpectedResult = "foo/phi")]
    public string ValidPath_MatchingOutput(string path)
    {
        var result = PrimaryService!.TryGet(Db!, "1", path, out var readModel);

        Assert.That(result, Is.True);
        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel!.HtmlContent, Is.Not.Null);

        return readModel!.HtmlContent!;
    }
}