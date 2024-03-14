using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WormCat.Razor.Migrations
{
    /// <inheritdoc />
    public partial class ReworkUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Record",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Location",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Container",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Record_UserId",
                table: "Record",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_UserId",
                table: "Location",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Container_UserId",
                table: "Container",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Container_Users_UserId",
                table: "Container",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Users_UserId",
                table: "Location",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Record_Users_UserId",
                table: "Record",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Container_Users_UserId",
                table: "Container");

            migrationBuilder.DropForeignKey(
                name: "FK_Location_Users_UserId",
                table: "Location");

            migrationBuilder.DropForeignKey(
                name: "FK_Record_Users_UserId",
                table: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Record_UserId",
                table: "Record");

            migrationBuilder.DropIndex(
                name: "IX_Location_UserId",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Container_UserId",
                table: "Container");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Record");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Container");
        }
    }
}
