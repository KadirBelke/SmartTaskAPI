using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTaskAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToTaskItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TaskItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TaskItems");
        }
    }
}
