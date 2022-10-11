using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class HintRemoveFileAndMakeTaskHintNtoN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hint_Files_FileId",
                table: "Hint");

            migrationBuilder.DropForeignKey(
                name: "FK_Hint_Tasks_TaskId",
                table: "Hint");

            migrationBuilder.DropIndex(
                name: "IX_Hint_FileId",
                table: "Hint");

            migrationBuilder.DropIndex(
                name: "IX_Hint_TaskId",
                table: "Hint");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Hint");

            migrationBuilder.DropColumn(
                name: "FileMime",
                table: "Hint");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Hint");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Hint");

            migrationBuilder.CreateTable(
                name: "HintTask",
                columns: table => new
                {
                    HintsId = table.Column<int>(type: "int", nullable: false),
                    TasksId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HintTask", x => new { x.HintsId, x.TasksId });
                    table.ForeignKey(
                        name: "FK_HintTask_Hint_HintsId",
                        column: x => x.HintsId,
                        principalTable: "Hint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HintTask_Tasks_TasksId",
                        column: x => x.TasksId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HintTask_TasksId",
                table: "HintTask",
                column: "TasksId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HintTask");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Hint",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "FileMime",
                table: "Hint",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Hint",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "Hint",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Hint_FileId",
                table: "Hint",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Hint_TaskId",
                table: "Hint",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hint_Files_FileId",
                table: "Hint",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hint_Tasks_TaskId",
                table: "Hint",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
