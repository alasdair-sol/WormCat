using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddMetaDataToRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Record",
                newName: "Synopsis");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Record",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageCount",
                table: "Record",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicationDate",
                table: "Record",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "PublicationDate",
                table: "Record");

            migrationBuilder.RenameColumn(
                name: "Synopsis",
                table: "Record",
                newName: "Description");
        }
    }
}
