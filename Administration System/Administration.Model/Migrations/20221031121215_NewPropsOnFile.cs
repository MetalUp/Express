using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class NewPropsOnFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContentType",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageId",
                table: "Files",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_LanguageId",
                table: "Files",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Languages_LanguageId",
                table: "Files",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "LanguageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Languages_LanguageId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_LanguageId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Files");
        }
    }
}
