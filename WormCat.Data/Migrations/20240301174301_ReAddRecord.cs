using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class ReAddRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RecordBarcode",
                table: "Book",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Record",
                columns: table => new
                {
                    Barcode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ISBN = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "RecordBarcode",
                table: "Book");
        }
    }
}
