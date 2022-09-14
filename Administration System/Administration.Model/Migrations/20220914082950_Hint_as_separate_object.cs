using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class Hint_as_separate_object : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HintCosts",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Hints",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "MinimumRoleToAccess",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Hint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HtmlFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostInMarks = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hint_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hint_TaskId",
                table: "Hint",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hint");

            migrationBuilder.DropColumn(
                name: "MinimumRoleToAccess",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "HintCosts",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hints",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
