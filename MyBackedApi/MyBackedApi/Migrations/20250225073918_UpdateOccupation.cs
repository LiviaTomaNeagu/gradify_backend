using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOccupation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "occupations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminEmail",
                table: "occupations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminName",
                table: "occupations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminSurname",
                table: "occupations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "occupations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "occupations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "occupations");

            migrationBuilder.DropColumn(
                name: "AdminEmail",
                table: "occupations");

            migrationBuilder.DropColumn(
                name: "AdminName",
                table: "occupations");

            migrationBuilder.DropColumn(
                name: "AdminSurname",
                table: "occupations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "occupations");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "occupations");
        }
    }
}
