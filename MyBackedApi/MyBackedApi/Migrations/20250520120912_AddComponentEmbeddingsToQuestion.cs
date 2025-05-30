using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddComponentEmbeddingsToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float[]>(
                name: "ContentEmbedding",
                table: "questions",
                type: "real[]",
                nullable: true);

            migrationBuilder.AddColumn<float[]>(
                name: "DocumentEmbedding",
                table: "questions",
                type: "real[]",
                nullable: true);

            migrationBuilder.AddColumn<float[]>(
                name: "ImageEmbedding",
                table: "questions",
                type: "real[]",
                nullable: true);

            migrationBuilder.AddColumn<float[]>(
                name: "TitleEmbedding",
                table: "questions",
                type: "real[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentEmbedding",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "DocumentEmbedding",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "ImageEmbedding",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "TitleEmbedding",
                table: "questions");
        }
    }
}
