using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class cleanUpLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Languages");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Languages",
                newName: "CompilerLanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompilerLanguageId",
                table: "Languages",
                newName: "Version");

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
