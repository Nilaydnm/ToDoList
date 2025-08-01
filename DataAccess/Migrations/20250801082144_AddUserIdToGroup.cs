using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ToDoGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoGroups_UserId",
                table: "ToDoGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoGroups_Users_UserId",
                table: "ToDoGroups",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoGroups_Users_UserId",
                table: "ToDoGroups");

            migrationBuilder.DropIndex(
                name: "IX_ToDoGroups_UserId",
                table: "ToDoGroups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ToDoGroups");
        }
    }
}
