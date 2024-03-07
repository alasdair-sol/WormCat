using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class OwnersAndLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnLoan",
                table: "Book",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Book",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnLoan",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Book");
        }
    }
}
