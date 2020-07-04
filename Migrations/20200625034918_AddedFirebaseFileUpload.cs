using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerProject.Migrations
{
    public partial class AddedFirebaseFileUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ScreenShots");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ScreenShots",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "ScreenShots");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ScreenShots",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
