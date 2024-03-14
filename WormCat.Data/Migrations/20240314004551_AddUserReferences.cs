using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class AddUserReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Container");

            migrationBuilder.AddColumn<string>(
                name: "UserIds",
                table: "Record",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserIds",
                table: "Location",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserIds",
                table: "Container",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIds",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "UserIds",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "UserIds",
                table: "Container");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Record",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Location",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Container",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
