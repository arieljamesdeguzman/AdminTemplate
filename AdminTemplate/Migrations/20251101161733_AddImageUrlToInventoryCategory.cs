using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminTemplate.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToInventoryCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "InventoryCategories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "InventoryCategories");
        }
    }
}
