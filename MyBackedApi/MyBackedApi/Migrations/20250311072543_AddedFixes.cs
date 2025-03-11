using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "student_coordinator",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoordinatorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_coordinator", x => new { x.StudentId, x.CoordinatorId });
                    table.ForeignKey(
                        name: "FK_student_coordinator_users_CoordinatorId",
                        column: x => x.CoordinatorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_coordinator_users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_student_coordinator_CoordinatorId",
                table: "student_coordinator",
                column: "CoordinatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "student_coordinator");
        }
    }
}
