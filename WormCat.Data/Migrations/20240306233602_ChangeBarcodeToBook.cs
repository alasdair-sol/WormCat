using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBarcodeToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Record");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Book");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Record",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
