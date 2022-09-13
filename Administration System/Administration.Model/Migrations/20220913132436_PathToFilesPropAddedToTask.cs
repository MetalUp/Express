using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class PathToFilesPropAddedToTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MarksAwarded",
                table: "Assignments",
                newName: "Marks");

            migrationBuilder.AddColumn<string>(
                name: "PathToFiles",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathToFiles",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Marks",
                table: "Assignments",
                newName: "MarksAwarded");
        }
    }
}
