using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class WrapperRulesHelpersAddedToProject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Languages_LanguageID",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "LanguageID",
                table: "Projects",
                newName: "LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_LanguageID",
                table: "Projects",
                newName: "IX_Projects_LanguageId");

            migrationBuilder.AddColumn<int>(
                name: "HelpersFileId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegExRulesFileId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WrapperFileId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_HelpersFileId",
                table: "Projects",
                column: "HelpersFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_RegExRulesFileId",
                table: "Projects",
                column: "RegExRulesFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_WrapperFileId",
                table: "Projects",
                column: "WrapperFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Files_HelpersFileId",
                table: "Projects",
                column: "HelpersFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Files_RegExRulesFileId",
                table: "Projects",
                column: "RegExRulesFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Files_WrapperFileId",
                table: "Projects",
                column: "WrapperFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Languages_LanguageId",
                table: "Projects",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "LanguageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Files_HelpersFileId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Files_RegExRulesFileId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Files_WrapperFileId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Languages_LanguageId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_HelpersFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_RegExRulesFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_WrapperFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "HelpersFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RegExRulesFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "WrapperFileId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "LanguageId",
                table: "Projects",
                newName: "LanguageID");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_LanguageId",
                table: "Projects",
                newName: "IX_Projects_LanguageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Languages_LanguageID",
                table: "Projects",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID");
        }
    }
}
