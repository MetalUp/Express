using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class ticket101 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdditionalValidationRulesFileId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseValidationRulesFileId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AdditionalValidationRulesFileId",
                table: "Tasks",
                column: "AdditionalValidationRulesFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BaseValidationRulesFileId",
                table: "Tasks",
                column: "BaseValidationRulesFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_AdditionalValidationRulesFileId",
                table: "Tasks",
                column: "AdditionalValidationRulesFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Files_BaseValidationRulesFileId",
                table: "Tasks",
                column: "BaseValidationRulesFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_AdditionalValidationRulesFileId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_BaseValidationRulesFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AdditionalValidationRulesFileId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_BaseValidationRulesFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AdditionalValidationRulesFileId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "BaseValidationRulesFileId",
                table: "Tasks");
        }
    }
}
