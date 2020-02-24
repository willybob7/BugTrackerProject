using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerProject.Migrations
{
    public partial class addedusernametobugattributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssigneeUserName",
                table: "Bugs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssigneeUserName",
                table: "Bugs");
        }
    }
}
