using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class activityChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastCodeSubmitted",
                table: "Activities",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "HighestHintUsed",
                table: "Activities",
                newName: "HintUsed");

            migrationBuilder.RenameColumn(
                name: "ErrorMessage",
                table: "Activities",
                newName: "CodeSubmitted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Activities",
                newName: "LastCodeSubmitted");

            migrationBuilder.RenameColumn(
                name: "HintUsed",
                table: "Activities",
                newName: "HighestHintUsed");

            migrationBuilder.RenameColumn(
                name: "CodeSubmitted",
                table: "Activities",
                newName: "ErrorMessage");
        }
    }
}
