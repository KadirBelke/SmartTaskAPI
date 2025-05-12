using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTaskAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsAndTaskItemTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemTag_Tag_TagId",
                table: "TaskItemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemTag_TaskItems_TaskItemId",
                table: "TaskItemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.RenameTable(
                name: "TaskItemTag",
                newName: "TaskItemTags");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemTag_TagId",
                table: "TaskItemTags",
                newName: "IX_TaskItemTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItemTags",
                table: "TaskItemTags",
                columns: new[] { "TaskItemId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTags_Tags_TagId",
                table: "TaskItemTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTags_TaskItems_TaskItemId",
                table: "TaskItemTags",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemTags_Tags_TagId",
                table: "TaskItemTags");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskItemTags_TaskItems_TaskItemId",
                table: "TaskItemTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskItemTags",
                table: "TaskItemTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.RenameTable(
                name: "TaskItemTags",
                newName: "TaskItemTag");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameIndex(
                name: "IX_TaskItemTags_TagId",
                table: "TaskItemTag",
                newName: "IX_TaskItemTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskItemTag",
                table: "TaskItemTag",
                columns: new[] { "TaskItemId", "TagId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTag_Tag_TagId",
                table: "TaskItemTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItemTag_TaskItems_TaskItemId",
                table: "TaskItemTag",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
