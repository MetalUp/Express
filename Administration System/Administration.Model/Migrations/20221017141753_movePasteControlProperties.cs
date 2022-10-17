using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class movePasteControlProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasteExpression",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PasteFunctions",
                table: "Tasks");

            migrationBuilder.AddColumn<bool>(
                name: "PasteExpression",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PasteFunctions",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasteExpression",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PasteFunctions",
                table: "Projects");

            migrationBuilder.AddColumn<bool>(
                name: "PasteExpression",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PasteFunctions",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
