using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DaTours",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoOfDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    messess = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePayment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    tax = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    BookingFees = table.Column<int>(type: "int", nullable: false),
                    totalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TourID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    progress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isComment = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaTours", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DaTours_Tours_TourID",
                        column: x => x.TourID,
                        principalTable: "Tours",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DaTours_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DaTours_TourID",
                table: "DaTours",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_DaTours_UserID",
                table: "DaTours",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DaTours");
        }
    }
}
