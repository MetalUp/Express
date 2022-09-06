using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Users_FromUserId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_FromUserId",
                table: "Invitations");

            migrationBuilder.RenameColumn(
                name: "ValidForDays",
                table: "Invitations",
                newName: "Valid");

            migrationBuilder.RenameColumn(
                name: "ToUser",
                table: "Invitations",
                newName: "ToUserName");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FromId",
                table: "Invitations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_FromId",
                table: "Invitations",
                column: "FromId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Users_FromId",
                table: "Invitations",
                column: "FromId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Users_FromId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_FromId",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FromId",
                table: "Invitations");

            migrationBuilder.RenameColumn(
                name: "Valid",
                table: "Invitations",
                newName: "ValidForDays");

            migrationBuilder.RenameColumn(
                name: "ToUserName",
                table: "Invitations",
                newName: "ToUser");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_FromUserId",
                table: "Invitations",
                column: "FromUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Users_FromUserId",
                table: "Invitations",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
