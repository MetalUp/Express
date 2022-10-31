using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class IntroduceLanguageObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_BaseRulesFileId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_ExtraRulesFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_BaseRulesFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ExtraRulesFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "BaseRulesFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ExtraRulesFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "LanguageID",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MIMEType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WrapperFileId = table.Column<int>(type: "int", nullable: true),
                    HelpersFileId = table.Column<int>(type: "int", nullable: true),
                    RegExRulesFileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageID);
                    table.ForeignKey(
                        name: "FK_Languages_Files_HelpersFileId",
                        column: x => x.HelpersFileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Languages_Files_RegExRulesFileId",
                        column: x => x.RegExRulesFileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Languages_Files_WrapperFileId",
                        column: x => x.WrapperFileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LanguageID",
                table: "Projects",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_HelpersFileId",
                table: "Languages",
                column: "HelpersFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_RegExRulesFileId",
                table: "Languages",
                column: "RegExRulesFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WrapperFileId",
                table: "Languages",
                column: "WrapperFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Languages_LanguageID",
                table: "Projects",
                column: "LanguageID",
                principalTable: "Languages",
                principalColumn: "LanguageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Languages_LanguageID",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LanguageID",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "BaseRulesFileId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtraRulesFileId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BaseRulesFileId",
                table: "Tasks",
                column: "BaseRulesFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ExtraRulesFileId",
                table: "Tasks",
                column: "ExtraRulesFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_BaseRulesFileId",
                table: "Tasks",
                column: "BaseRulesFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_ExtraRulesFileId",
                table: "Tasks",
                column: "ExtraRulesFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
