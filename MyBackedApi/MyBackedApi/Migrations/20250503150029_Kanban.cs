using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class Kanban : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedSteps",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "Kanban",
                table: "student_details",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kanban",
                table: "student_details");

            migrationBuilder.AddColumn<int>(
                name: "CompletedSteps",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
