using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddBookRecordRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book");

            migrationBuilder.AlterColumn<int>(
                name: "RecordId",
                table: "Book",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book",
                column: "RecordId",
                principalTable: "Record",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book");

            migrationBuilder.AlterColumn<int>(
                name: "RecordId",
                table: "Book",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Record_RecordId",
                table: "Book",
                column: "RecordId",
                principalTable: "Record",
                principalColumn: "Id");
        }
    }
}
