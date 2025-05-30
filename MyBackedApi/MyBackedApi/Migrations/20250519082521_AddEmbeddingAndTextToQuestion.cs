using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEmbeddingAndTextToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentText",
                table: "questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double[]>(
                name: "EmbeddingVector",
                table: "questions",
                type: "float8[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageText",
                table: "questions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentText",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "EmbeddingVector",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "ImageText",
                table: "questions");
        }
    }
}
