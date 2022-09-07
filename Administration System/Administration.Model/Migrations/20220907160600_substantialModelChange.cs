using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class substantialModelChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Tasks_TaskId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_AssignedById",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Users_AssignedToId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Organisations_OrganisationId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Users_FromId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Organisations_OrganisationId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GroupId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_OrganisationId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailAddressConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "Invitations");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameColumn(
                name: "NextTaskDoesNotClearFunctions",
                table: "Tasks",
                newName: "NextTaskClearsFunctions");

            migrationBuilder.RenameColumn(
                name: "ToUserName",
                table: "Invitations",
                newName: "ToEmailAddress");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Activities",
                newName: "AssignmentId");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Activities",
                newName: "SubmittedCode");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_TaskId",
                table: "Activities",
                newName: "IX_Activities_AssignmentId");

            migrationBuilder.RenameColumn(
                name: "GroupName",
                table: "Group",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_OrganisationId",
                table: "Group",
                newName: "IX_Group_OrganisationId");

            migrationBuilder.AlterColumn<int>(
                name: "MaxMarks",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FromId",
                table: "Invitations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RealName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Teachers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroups",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroups", x => new { x.StudentId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_StudentGroups_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentGroups_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AuthorId",
                table: "Tasks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_GroupId",
                table: "StudentGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_OrganisationId",
                table: "Students",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_OrganisationId",
                table: "Teachers",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Assignments_AssignmentId",
                table: "Activities",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Students_AssignedToId",
                table: "Assignments",
                column: "AssignedToId",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Teachers_AssignedById",
                table: "Assignments",
                column: "AssignedById",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Organisations_OrganisationId",
                table: "Group",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Teachers_FromId",
                table: "Invitations",
                column: "FromId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AuthorId",
                table: "Tasks",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Assignments_AssignmentId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Students_AssignedToId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Teachers_AssignedById",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_Organisations_OrganisationId",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Teachers_FromId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AuthorId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AuthorId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "Organisations");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameColumn(
                name: "NextTaskClearsFunctions",
                table: "Tasks",
                newName: "NextTaskDoesNotClearFunctions");

            migrationBuilder.RenameColumn(
                name: "ToEmailAddress",
                table: "Invitations",
                newName: "ToUserName");

            migrationBuilder.RenameColumn(
                name: "SubmittedCode",
                table: "Activities",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "Activities",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_AssignmentId",
                table: "Activities",
                newName: "IX_Activities_TaskId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Groups",
                newName: "GroupName");

            migrationBuilder.RenameIndex(
                name: "IX_Group_OrganisationId",
                table: "Groups",
                newName: "IX_Groups_OrganisationId");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailAddressConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreferredLanguage",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxMarks",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FromId",
                table: "Invitations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FromUserId",
                table: "Invitations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganisationId",
                table: "Users",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Tasks_TaskId",
                table: "Activities",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_AssignedById",
                table: "Assignments",
                column: "AssignedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Users_AssignedToId",
                table: "Assignments",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Organisations_OrganisationId",
                table: "Groups",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Users_FromId",
                table: "Invitations",
                column: "FromId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_GroupId",
                table: "Users",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Organisations_OrganisationId",
                table: "Users",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id");
        }
    }
}
