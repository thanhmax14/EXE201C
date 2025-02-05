using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Datphongs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    checkIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    checkOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoOfDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messess = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePayment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    tax = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    BookingFees = table.Column<int>(type: "int", nullable: false),
                    totalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datphongs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Datphongs_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID");
                    table.ForeignKey(
                        name: "FK_Datphongs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Datphongs_RoomID",
                table: "Datphongs",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Datphongs_UserID",
                table: "Datphongs",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Datphongs");
        }
    }
}
