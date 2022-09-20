using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class MoreFileAttchOnTaskAndHint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tests",
                table: "Tasks",
                newName: "TestsName");

            migrationBuilder.RenameColumn(
                name: "ReadyMadeFunctions",
                table: "Tasks",
                newName: "TestsMime");

            migrationBuilder.RenameColumn(
                name: "DescriptionName",
                table: "Tasks",
                newName: "RMFName");

            migrationBuilder.RenameColumn(
                name: "DescriptionMime",
                table: "Tasks",
                newName: "RMFMime");

            migrationBuilder.RenameColumn(
                name: "DescriptionContent",
                table: "Tasks",
                newName: "TestsContent");

            migrationBuilder.RenameColumn(
                name: "HtmlFile",
                table: "Hint",
                newName: "FileName");

            migrationBuilder.AddColumn<byte[]>(
                name: "DescContent",
                table: "Tasks",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescMime",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescName",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RMFContent",
                table: "Tasks",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "Hint",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileMime",
                table: "Hint",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescContent",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DescMime",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DescName",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RMFContent",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Hint");

            migrationBuilder.DropColumn(
                name: "FileMime",
                table: "Hint");

            migrationBuilder.RenameColumn(
                name: "TestsName",
                table: "Tasks",
                newName: "Tests");

            migrationBuilder.RenameColumn(
                name: "TestsMime",
                table: "Tasks",
                newName: "ReadyMadeFunctions");

            migrationBuilder.RenameColumn(
                name: "TestsContent",
                table: "Tasks",
                newName: "DescriptionContent");

            migrationBuilder.RenameColumn(
                name: "RMFName",
                table: "Tasks",
                newName: "DescriptionName");

            migrationBuilder.RenameColumn(
                name: "RMFMime",
                table: "Tasks",
                newName: "DescriptionMime");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Hint",
                newName: "HtmlFile");
        }
    }
}
