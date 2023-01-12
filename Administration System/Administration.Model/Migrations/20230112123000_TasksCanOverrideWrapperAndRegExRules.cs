using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class TasksCanOverrideWrapperAndRegExRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Files_HelpersFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_HelpersFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "HelpersFileId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "RegExRulesFileId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TestsRunOnClient",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WrapperFileId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_RegExRulesFileId",
                table: "Tasks",
                column: "RegExRulesFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_WrapperFileId",
                table: "Tasks",
                column: "WrapperFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_RegExRulesFileId",
                table: "Tasks",
                column: "RegExRulesFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_WrapperFileId",
                table: "Tasks",
                column: "WrapperFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_RegExRulesFileId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_WrapperFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_RegExRulesFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_WrapperFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RegExRulesFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TestsRunOnClient",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "WrapperFileId",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "HelpersFileId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_HelpersFileId",
                table: "Projects",
                column: "HelpersFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Files_HelpersFileId",
                table: "Projects",
                column: "HelpersFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
