using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class AddFileToHint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Hint",
                type: "int",
                nullable: true);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hint_Files_FileId",
                table: "Hint");

            migrationBuilder.DropIndex(
                name: "IX_Hint_FileId",
                table: "Hint");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Hint");
        }
    }
}
