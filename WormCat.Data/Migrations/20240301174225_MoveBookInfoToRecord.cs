using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class MoveBookInfoToRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Record_RecordBarcode",
                table: "Book");

            migrationBuilder.DropTable(
                name: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Book_RecordBarcode",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "RecordBarcode",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Book");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "Book",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecordBarcode",
                table: "Book",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Book",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Record",
                columns: table => new
                {
                    Barcode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Record", x => x.Barcode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_RecordBarcode",
                table: "Book",
                column: "RecordBarcode");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Record_RecordBarcode",
                table: "Book",
                column: "RecordBarcode",
                principalTable: "Record",
                principalColumn: "Barcode");
        }
    }
}
