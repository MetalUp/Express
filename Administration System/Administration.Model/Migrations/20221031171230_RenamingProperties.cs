using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class RenamingProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_HiddenFunctionsFileId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tasks",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "HiddenFunctionsFileId",
                table: "Tasks",
                newName: "HiddenCodeFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_HiddenFunctionsFileId",
                table: "Tasks",
                newName: "IX_Tasks_HiddenCodeFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_HiddenCodeFileId",
                table: "Tasks",
                column: "HiddenCodeFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_HiddenCodeFileId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tasks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "HiddenCodeFileId",
                table: "Tasks",
                newName: "HiddenFunctionsFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_HiddenCodeFileId",
                table: "Tasks",
                newName: "IX_Tasks_HiddenFunctionsFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_HiddenFunctionsFileId",
                table: "Tasks",
                column: "HiddenFunctionsFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
