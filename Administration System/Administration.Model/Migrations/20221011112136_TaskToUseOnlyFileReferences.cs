using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    public partial class TaskToUseOnlyFileReferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescContent",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DescName",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RMFContent",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TestsContent",
                table: "Tasks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "DescContent",
                table: "Tasks",
                type: "varbinary(max)",
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
                name: "TestsContent",
                table: "Tasks",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
