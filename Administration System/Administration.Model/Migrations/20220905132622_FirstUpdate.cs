using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class FirstUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Organisation_OrganisationId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organisation",
                table: "Organisation");

            migrationBuilder.RenameTable(
                name: "Organisation",
                newName: "Organisations");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tasks",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxMarks",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "NextTaskDoesNotClearFunctions",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NextTaskId",
                table: "Tasks",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "PreviousTaskId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadyMadeFunctions",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tests",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organisations",
                table: "Organisations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    AssignedToId = table.Column<int>(type: "int", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: false),
                    DueBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MarksAwarded = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_Users_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assignments_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    ToJoinId = table.Column<int>(type: "int", nullable: false),
                    AsRole = table.Column<int>(type: "int", nullable: false),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidForDays = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitations_Organisations_ToJoinId",
                        column: x => x.ToJoinId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitations_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_NextTaskId",
                table: "Tasks",
                column: "NextTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PreviousTaskId",
                table: "Tasks",
                column: "PreviousTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssignedById",
                table: "Assignments",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssignedToId",
                table: "Assignments",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TaskId",
                table: "Assignments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_OrganisationId",
                table: "Groups",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_FromUserId",
                table: "Invitations",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ToJoinId",
                table: "Invitations",
                column: "ToJoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_NextTaskId",
                table: "Tasks",
                column: "NextTaskId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Tasks_PreviousTaskId",
                table: "Tasks",
                column: "PreviousTaskId",
                principalTable: "Tasks",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_NextTaskId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Tasks_PreviousTaskId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Organisations_OrganisationId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Users_GroupId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_NextTaskId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PreviousTaskId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Organisations",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "HintCosts",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Hints",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "MaxMarks",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NextTaskDoesNotClearFunctions",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "NextTaskId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PasteExpression",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PasteFunctions",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PreviousTaskId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ReadyMadeFunctions",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Tests",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "Organisations",
                newName: "Organisation");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tasks",
                newName: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Organisation",
                table: "Organisation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Organisation_OrganisationId",
                table: "Users",
                column: "OrganisationId",
                principalTable: "Organisation",
                principalColumn: "Id");
        }
    }
}
