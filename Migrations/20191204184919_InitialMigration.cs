using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerProject.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bugs",
                columns: table => new
                {
                    BugId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociatedProject = table.Column<int>(nullable: false),
                    ReporterID = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EnteredDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    AssingeeId = table.Column<int>(nullable: false),
                    Severity = table.Column<int>(nullable: false),
                    Reproducible = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bugs", x => x.BugId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(nullable: false),
                    ProjectDescription = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    ProjectStatus = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "ScreenShots",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssociatedBug = table.Column<int>(nullable: false),
                    FilePath = table.Column<string>(nullable: true),
                    BugAttributesBugId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreenShots", x => x.id);
                    table.ForeignKey(
                        name: "FK_ScreenShots_Bugs_BugAttributesBugId",
                        column: x => x.BugAttributesBugId,
                        principalTable: "Bugs",
                        principalColumn: "BugId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectBugs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(nullable: false),
                    BugId = table.Column<int>(nullable: false),
                    ProjectAttributesProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectBugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectBugs_Projects_ProjectAttributesProjectId",
                        column: x => x.ProjectAttributesProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersAssigned",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(nullable: false),
                    AssignedUserId = table.Column<int>(nullable: false),
                    ProjectAttributesProjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersAssigned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersAssigned_Projects_ProjectAttributesProjectId",
                        column: x => x.ProjectAttributesProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectBugs_ProjectAttributesProjectId",
                table: "ProjectBugs",
                column: "ProjectAttributesProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreenShots_BugAttributesBugId",
                table: "ScreenShots",
                column: "BugAttributesBugId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersAssigned_ProjectAttributesProjectId",
                table: "UsersAssigned",
                column: "ProjectAttributesProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectBugs");

            migrationBuilder.DropTable(
                name: "ScreenShots");

            migrationBuilder.DropTable(
                name: "UsersAssigned");

            migrationBuilder.DropTable(
                name: "Bugs");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
