using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTopicEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing Topic column
            migrationBuilder.DropColumn(
                name: "Topic",
                table: "questions");

            // Add the new Topic column as integer
            migrationBuilder.AddColumn<int>(
                name: "Topic",
                table: "questions",
                type: "int",
                nullable: false,
                defaultValue: 0); // Set a default value to prevent issues with existing rows
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the new Topic column
            migrationBuilder.DropColumn(
                name: "Topic",
                table: "questions");

            // Add back the old Topic column as text
            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "questions",
                type: "text",
                nullable: false);
        }

    }
}
