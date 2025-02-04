using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "img",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "joinin",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "linkLocation",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "joinin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "linkLocation",
                table: "Hotels");
        }
    }
}
