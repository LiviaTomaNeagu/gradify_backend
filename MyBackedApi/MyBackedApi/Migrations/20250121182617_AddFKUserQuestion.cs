using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFKUserQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_questions_UserId",
                table: "questions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_users_UserId",
                table: "questions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_users_UserId",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_questions_UserId",
                table: "questions");
        }
    }
}
