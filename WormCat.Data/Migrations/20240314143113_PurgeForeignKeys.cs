using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class PurgeForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContainerIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LocationIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RecordIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserIds",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "UserIds",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "UserIds",
                table: "Container");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContainerIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
