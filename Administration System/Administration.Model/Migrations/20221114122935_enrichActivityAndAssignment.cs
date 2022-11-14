using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class enrichActivityAndAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Activities",
                newName: "TaskId");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Activities",
                newName: "LastCodeSubmitted");

            migrationBuilder.AddColumn<string>(
                name: "TeacherNotes",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityType",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HighestHintUsed",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TaskId",
                table: "Activities",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Tasks_TaskId",
                table: "Activities",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Tasks_TaskId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_TaskId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TeacherNotes",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ActivityType",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "HighestHintUsed",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Activities",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "LastCodeSubmitted",
                table: "Activities",
                newName: "Details");
        }
    }
}
