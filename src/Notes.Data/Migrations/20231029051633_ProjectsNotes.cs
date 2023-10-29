using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notes.Data.Migrations;

/// <inheritdoc />
public partial class ProjectsNotes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "NoteLinks",
            columns: table => new
            {
                FromNoteId = table.Column<string>(type: "text", nullable: false),
                ToNoteId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
            });

        migrationBuilder.CreateTable(
            name: "Notes",
            columns: table => new
            {
                NoteId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                ParentNoteId = table.Column<string>(type: "text", nullable: true),
                ProjectId = table.Column<string>(type: "text", nullable: false),
                HtmlContent = table.Column<string>(type: "text", nullable: true),
                ContentRaw = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notes", x => x.NoteId);
            });

        migrationBuilder.CreateTable(
            name: "Projects",
            columns: table => new
            {
                ProjectId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                UserId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Projects", x => x.ProjectId);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "NoteLinks");

        migrationBuilder.DropTable(
            name: "Notes");

        migrationBuilder.DropTable(
            name: "Projects");
    }
}