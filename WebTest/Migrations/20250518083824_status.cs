﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTest.Migrations
{
    /// <inheritdoc />
    public partial class status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Friendship",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Friendship");
        }
    }
}
