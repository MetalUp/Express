using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class ChangeStudentGroupNtoNrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_Group_GroupId",
                table: "StudentGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_User_StudentId",
                table: "StudentGroups");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "StudentGroups",
                newName: "StudentsId");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentGroups",
                newName: "GroupsId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGroups_GroupId",
                table: "StudentGroups",
                newName: "IX_StudentGroups_StudentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_Group_GroupsId",
                table: "StudentGroups",
                column: "GroupsId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_User_StudentsId",
                table: "StudentGroups",
                column: "StudentsId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_Group_GroupsId",
                table: "StudentGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_User_StudentsId",
                table: "StudentGroups");

            migrationBuilder.RenameColumn(
                name: "StudentsId",
                table: "StudentGroups",
                newName: "GroupId");

            migrationBuilder.RenameColumn(
                name: "GroupsId",
                table: "StudentGroups",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentGroups_StudentsId",
                table: "StudentGroups",
                newName: "IX_StudentGroups_GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_Group_GroupId",
                table: "StudentGroups",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_User_StudentId",
                table: "StudentGroups",
                column: "StudentId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
