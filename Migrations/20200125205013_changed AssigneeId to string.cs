using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTrackerProject.Migrations
{
    public partial class changedAssigneeIdtostring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersAssigned_Projects_ProjectAttributesProjectId",
                table: "UsersAssigned");

            migrationBuilder.DropIndex(
                name: "IX_UsersAssigned_ProjectAttributesProjectId",
                table: "UsersAssigned");

            migrationBuilder.DropColumn(
                name: "ProjectAttributesProjectId",
                table: "UsersAssigned");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AssingeeId",
                table: "Bugs",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectAttributesProjectId",
                table: "UsersAssigned",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Projects",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssingeeId",
                table: "Bugs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersAssigned_ProjectAttributesProjectId",
                table: "UsersAssigned",
                column: "ProjectAttributesProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersAssigned_Projects_ProjectAttributesProjectId",
                table: "UsersAssigned",
                column: "ProjectAttributesProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
