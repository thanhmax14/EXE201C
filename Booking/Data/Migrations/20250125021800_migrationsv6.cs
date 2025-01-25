using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Data.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GalleryRoom",
                columns: table => new
                {
                    ImageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFeatureImage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryRoom", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_GalleryRoom_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GalleryRoom_RoomID",
                table: "GalleryRoom",
                column: "RoomID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GalleryRoom");
        }
    }
}
