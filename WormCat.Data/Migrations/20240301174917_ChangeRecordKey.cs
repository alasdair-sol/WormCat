using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRecordKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Record_RecordBarcode",
                table: "Book");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Record",
                table: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Book_RecordBarcode",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "RecordBarcode",
                table: "Book");

            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Record",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Record",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "RecordId",
                table: "Book",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Record",
                table: "Record",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Book_RecordId",
                table: "Book",
                column: "RecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book",
                column: "RecordId",
                principalTable: "Record",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Record",
                table: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Book_RecordId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "RecordId",
                table: "Book");

            migrationBuilder.AlterColumn<Guid>(
                name: "Barcode",
                table: "Record",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "RecordBarcode",
                table: "Book",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Record",
                table: "Record",
                column: "Barcode");

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
