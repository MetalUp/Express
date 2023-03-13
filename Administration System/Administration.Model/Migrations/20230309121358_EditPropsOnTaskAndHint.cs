using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class EditPropsOnTaskAndHint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Hint");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Hint",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
