﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBackedApi.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
