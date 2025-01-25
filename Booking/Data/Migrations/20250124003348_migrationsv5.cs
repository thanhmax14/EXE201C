using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Data.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Hotels",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Room_HotelID",
                table: "Room",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_UserID",
                table: "Hotels",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Users_UserID",
                table: "Hotels",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Hotels_HotelID",
                table: "Room",
                column: "HotelID",
                principalTable: "Hotels",
                principalColumn: "HotelID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Users_UserID",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_Hotels_HotelID",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Room_HotelID",
                table: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_UserID",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Hotels");
        }
    }
}
