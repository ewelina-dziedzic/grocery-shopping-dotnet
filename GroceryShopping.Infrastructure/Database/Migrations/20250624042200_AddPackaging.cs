using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryShopping.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPackaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Packaging",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Packaging",
                table: "Products");
        }
    }
}
