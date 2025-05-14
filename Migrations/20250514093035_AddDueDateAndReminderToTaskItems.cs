using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTaskAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDueDateAndReminderToTaskItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "TaskItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderTime",
                table: "TaskItems",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "ReminderTime",
                table: "TaskItems");
        }
    }
}
