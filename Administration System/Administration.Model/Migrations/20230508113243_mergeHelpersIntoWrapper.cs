using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class mergeHelpersIntoWrapper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Files_HelpersFileId",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_HelpersFileId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "HelpersFileId",
                table: "Languages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HelpersFileId",
                table: "Languages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_HelpersFileId",
                table: "Languages",
                column: "HelpersFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Files_HelpersFileId",
                table: "Languages",
                column: "HelpersFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
