using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class RenamedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "user_auth_tokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "activation_codes",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_auth_tokens",
                newName: "Uuid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "activation_codes",
                newName: "Uuid");
        }
    }
}
