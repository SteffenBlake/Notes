using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notes.Data.Migrations
{
    /// <inheritdoc />
    public partial class Directories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Notes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NoteLinkId",
                table: "NoteLinks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteLinks",
                table: "NoteLinks",
                column: "NoteLinkId");

            migrationBuilder.CreateTable(
                name: "EditHistory",
                columns: table => new
                {
                    EditHistoryId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EdittedById = table.Column<string>(type: "text", nullable: false),
                    NoteId = table.Column<string>(type: "text", nullable: true),
                    ProjectId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditHistory", x => x.EditHistoryId);
                    table.ForeignKey(
                        name: "FK_EditHistory_AspNetUsers_EdittedById",
                        column: x => x.EdittedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EditHistory_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "NoteId");
                    table.ForeignKey(
                        name: "FK_EditHistory_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId");
                });

            migrationBuilder.CreateTable(
                name: "WebsiteConfiguration",
                columns: table => new
                {
                    WebsiteName = table.Column<string>(type: "text", nullable: false),
                    WebsiteDescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_ProjectId",
                table: "Notes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EditHistory_EdittedById",
                table: "EditHistory",
                column: "EdittedById");

            migrationBuilder.CreateIndex(
                name: "IX_EditHistory_NoteId",
                table: "EditHistory",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_EditHistory_ProjectId",
                table: "EditHistory",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Projects_ProjectId",
                table: "Notes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Projects_ProjectId",
                table: "Notes");

            migrationBuilder.DropTable(
                name: "EditHistory");

            migrationBuilder.DropTable(
                name: "WebsiteConfiguration");

            migrationBuilder.DropIndex(
                name: "IX_Notes_ProjectId",
                table: "Notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteLinks",
                table: "NoteLinks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "NoteLinkId",
                table: "NoteLinks");
        }
    }
}
