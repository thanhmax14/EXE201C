using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTour_Tours_TourID",
                table: "ReviewTour");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTour_Users_AppUserId",
                table: "ReviewTour");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewTour",
                table: "ReviewTour");

            migrationBuilder.DropIndex(
                name: "IX_ReviewTour_AppUserId",
                table: "ReviewTour");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "ReviewTour");

            migrationBuilder.RenameTable(
                name: "ReviewTour",
                newName: "ReviewTours");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewTour_TourID",
                table: "ReviewTours",
                newName: "IX_ReviewTours_TourID");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "ReviewTours",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewTours",
                table: "ReviewTours",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTours_UserID",
                table: "ReviewTours",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTours_Tours_TourID",
                table: "ReviewTours",
                column: "TourID",
                principalTable: "Tours",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTours_Users_UserID",
                table: "ReviewTours",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTours_Tours_TourID",
                table: "ReviewTours");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTours_Users_UserID",
                table: "ReviewTours");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewTours",
                table: "ReviewTours");

            migrationBuilder.DropIndex(
                name: "IX_ReviewTours_UserID",
                table: "ReviewTours");

            migrationBuilder.RenameTable(
                name: "ReviewTours",
                newName: "ReviewTour");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewTours_TourID",
                table: "ReviewTour",
                newName: "IX_ReviewTour_TourID");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "ReviewTour",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "ReviewTour",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewTour",
                table: "ReviewTour",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTour_AppUserId",
                table: "ReviewTour",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTour_Tours_TourID",
                table: "ReviewTour",
                column: "TourID",
                principalTable: "Tours",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTour_Users_AppUserId",
                table: "ReviewTour",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
