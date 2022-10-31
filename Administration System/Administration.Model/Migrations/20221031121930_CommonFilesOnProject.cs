using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class CommonFilesOnProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommonHiddenCodeFileId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommonTestsFileId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CommonHiddenCodeFileId",
                table: "Projects",
                column: "CommonHiddenCodeFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CommonTestsFileId",
                table: "Projects",
                column: "CommonTestsFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Files_CommonHiddenCodeFileId",
                table: "Projects",
                column: "CommonHiddenCodeFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Files_CommonTestsFileId",
                table: "Projects",
                column: "CommonTestsFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Files_CommonHiddenCodeFileId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Files_CommonTestsFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CommonHiddenCodeFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CommonTestsFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CommonHiddenCodeFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CommonTestsFileId",
                table: "Projects");
        }
    }
}
