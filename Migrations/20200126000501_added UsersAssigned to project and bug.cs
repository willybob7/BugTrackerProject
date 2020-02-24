using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerProject.Migrations
{
    public partial class addedUsersAssignedtoprojectandbug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsersAssigned",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersAssigned",
                table: "Bugs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersAssigned",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UsersAssigned",
                table: "Bugs");
        }
    }
}
