using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixToDoGroupTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDos_ToDoGroup_GroupId",
                table: "ToDos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ToDoGroup",
                table: "ToDoGroup");

            migrationBuilder.RenameTable(
                name: "ToDoGroup",
                newName: "ToDoGroups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ToDoGroups",
                table: "ToDoGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDos_ToDoGroups_GroupId",
                table: "ToDos",
                column: "GroupId",
                principalTable: "ToDoGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDos_ToDoGroups_GroupId",
                table: "ToDos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ToDoGroups",
                table: "ToDoGroups");

            migrationBuilder.RenameTable(
                name: "ToDoGroups",
                newName: "ToDoGroup");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ToDoGroup",
                table: "ToDoGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDos_ToDoGroup_GroupId",
                table: "ToDos",
                column: "GroupId",
                principalTable: "ToDoGroup",
                principalColumn: "Id");
        }
    }
}
