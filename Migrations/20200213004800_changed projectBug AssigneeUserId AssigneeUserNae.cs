using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerProject.Migrations
{
    public partial class changedprojectBugAssigneeUserIdAssigneeUserNae : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "UsersAssigned",
                table: "Bugs");

            migrationBuilder.AddColumn<string>(
                name: "AssigneeUserId",
                table: "Bugs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssingeeUserName",
                table: "Bugs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeUserId",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "AssingeeUserName",
                table: "Bugs");

            migrationBuilder.AddColumn<int>(
                name: "Assigneeid",
                table: "Bugs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersAssigned",
                table: "Bugs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BugAssignee",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
    }
}
