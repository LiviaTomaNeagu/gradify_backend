using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class RenamedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthTokens_users_UserId",
                table: "UserAuthTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAuthTokens",
                table: "UserAuthTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivationCodes",
                table: "ActivationCodes");

            migrationBuilder.RenameTable(
                name: "UserAuthTokens",
                newName: "user_auth_tokens");

            migrationBuilder.RenameTable(
                name: "ActivationCodes",
                newName: "activation_codes");

            migrationBuilder.RenameIndex(
                name: "IX_UserAuthTokens_UserId",
                table: "user_auth_tokens",
                newName: "IX_user_auth_tokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_auth_tokens",
                table: "user_auth_tokens",
                column: "Uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_activation_codes",
                table: "activation_codes",
                column: "Uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_user_auth_tokens_users_UserId",
                table: "user_auth_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_auth_tokens_users_UserId",
                table: "user_auth_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_auth_tokens",
                table: "user_auth_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_activation_codes",
                table: "activation_codes");

            migrationBuilder.RenameTable(
                name: "user_auth_tokens",
                newName: "UserAuthTokens");

            migrationBuilder.RenameTable(
                name: "activation_codes",
                newName: "ActivationCodes");

            migrationBuilder.RenameIndex(
                name: "IX_user_auth_tokens_UserId",
                table: "UserAuthTokens",
                newName: "IX_UserAuthTokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAuthTokens",
                table: "UserAuthTokens",
                column: "Uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivationCodes",
                table: "ActivationCodes",
                column: "Uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthTokens_users_UserId",
                table: "UserAuthTokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
