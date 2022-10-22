using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class renamePropertiesInTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_AdditionalValidationRulesFileId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_BaseValidationRulesFileId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "BaseValidationRulesFileId",
                table: "Tasks",
                newName: "ExtraRulesFileId");

            migrationBuilder.RenameColumn(
                name: "AdditionalValidationRulesFileId",
                table: "Tasks",
                newName: "BaseRulesFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_BaseValidationRulesFileId",
                table: "Tasks",
                newName: "IX_Tasks_ExtraRulesFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AdditionalValidationRulesFileId",
                table: "Tasks",
                newName: "IX_Tasks_BaseRulesFileId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_BaseRulesFileId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Files_ExtraRulesFileId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ExtraRulesFileId",
                table: "Tasks",
                newName: "BaseValidationRulesFileId");

            migrationBuilder.RenameColumn(
                name: "BaseRulesFileId",
                table: "Tasks",
                newName: "AdditionalValidationRulesFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_ExtraRulesFileId",
                table: "Tasks",
                newName: "IX_Tasks_BaseValidationRulesFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_BaseRulesFileId",
                table: "Tasks",
                newName: "IX_Tasks_AdditionalValidationRulesFileId");

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
    }
}
