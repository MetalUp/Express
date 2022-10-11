using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class AddReferencesToFilesDefaultingToPlaceholder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescriptionFileId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "HiddenFunctionsFileId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TestsFileId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Hint",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DescriptionFileId",
                table: "Tasks",
                column: "DescriptionFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_HiddenFunctionsFileId",
                table: "Tasks",
                column: "HiddenFunctionsFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TestsFileId",
                table: "Tasks",
                column: "TestsFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Hint_FileId",
                table: "Hint",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hint_Files_FileId",
                table: "Hint",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_DescriptionFileId",
                table: "Tasks",
                column: "DescriptionFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_HiddenFunctionsFileId",
                table: "Tasks",
                column: "HiddenFunctionsFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_TestsFileId",
                table: "Tasks",
                column: "TestsFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hint_Files_FileId",
                table: "Hint");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_DescriptionFileId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_HiddenFunctionsFileId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_TestsFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DescriptionFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_HiddenFunctionsFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TestsFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Hint_FileId",
                table: "Hint");

            migrationBuilder.DropColumn(
                name: "DescriptionFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HiddenFunctionsFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TestsFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Hint");
        }
    }
}
