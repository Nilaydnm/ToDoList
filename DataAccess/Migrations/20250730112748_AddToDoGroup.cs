using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddToDoGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "ToDos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "ToDos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ToDoGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoGroup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDos_GroupId",
                table: "ToDos",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDos_ToDoGroup_GroupId",
                table: "ToDos",
                column: "GroupId",
                principalTable: "ToDoGroup",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDos_ToDoGroup_GroupId",
                table: "ToDos");

            migrationBuilder.DropTable(
                name: "ToDoGroup");

            migrationBuilder.DropIndex(
                name: "IX_ToDos_GroupId",
                table: "ToDos");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "ToDos");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ToDos");
        }
    }
}
