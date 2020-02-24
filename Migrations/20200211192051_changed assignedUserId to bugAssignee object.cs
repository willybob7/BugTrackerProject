using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerProject.Migrations
{
    public partial class changedassignedUserIdtobugAssigneeobject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeUserName",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "AssingeeId",
                table: "Bugs");

            migrationBuilder.AddColumn<int>(
                name: "Assigneeid",
                table: "Bugs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BugAssignee",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugAssignee", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bugs_Assigneeid",
                table: "Bugs",
                column: "Assigneeid");

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_BugAssignee_Assigneeid",
                table: "Bugs",
                column: "Assigneeid",
                principalTable: "BugAssignee",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_BugAssignee_Assigneeid",
                table: "Bugs");

            migrationBuilder.DropTable(
                name: "BugAssignee");

            migrationBuilder.DropIndex(
                name: "IX_Bugs_Assigneeid",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "Assigneeid",
                table: "Bugs");

            migrationBuilder.AddColumn<string>(
                name: "AssigneeUserName",
                table: "Bugs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssingeeId",
                table: "Bugs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
