using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewHotels",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cmt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    datecmt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    relay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dateRelay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewHotels", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReviewHotels_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReviewHotels_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHotels_HotelID",
                table: "ReviewHotels",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHotels_UserID",
                table: "ReviewHotels",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewHotels");
        }
    }
}
