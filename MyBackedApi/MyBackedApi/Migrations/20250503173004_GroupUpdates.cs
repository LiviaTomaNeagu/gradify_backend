using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class GroupUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "student_details",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("0e428e37-5fc8-4f91-84d0-6c2fb9d4e2cc"));

            migrationBuilder.CreateIndex(
                name: "IX_student_details_GroupId",
                table: "student_details",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_student_details_groups_GroupId",
                table: "student_details",
                column: "GroupId",
                principalTable: "groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_student_details_groups_GroupId",
                table: "student_details");

            migrationBuilder.DropIndex(
                name: "IX_student_details_GroupId",
                table: "student_details");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "student_details");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "student_details",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
