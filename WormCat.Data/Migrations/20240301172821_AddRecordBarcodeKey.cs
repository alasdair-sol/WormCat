using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordBarcodeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Book");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Record",
                newName: "Barcode");

            migrationBuilder.RenameColumn(
                name: "RecordId",
                table: "Book",
                newName: "RecordBarcode");

            migrationBuilder.RenameIndex(
                name: "IX_Book_RecordId",
                table: "Book",
                newName: "IX_Book_RecordBarcode");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Record_RecordBarcode",
                table: "Book",
                column: "RecordBarcode",
                principalTable: "Record",
                principalColumn: "Barcode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Record_RecordBarcode",
                table: "Book");

            migrationBuilder.RenameColumn(
                name: "Barcode",
                table: "Record",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RecordBarcode",
                table: "Book",
                newName: "RecordId");

            migrationBuilder.RenameIndex(
                name: "IX_Book_RecordBarcode",
                table: "Book",
                newName: "IX_Book_RecordId");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Book",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book",
                column: "RecordId",
                principalTable: "Record",
                principalColumn: "Id");
        }
    }
}
