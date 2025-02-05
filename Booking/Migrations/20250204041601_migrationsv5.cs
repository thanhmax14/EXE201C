using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dongtien_Users_UserID",
                table: "dongtien");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dongtien",
                table: "dongtien");

            migrationBuilder.RenameTable(
                name: "dongtien",
                newName: "Dongtiens");

            migrationBuilder.RenameIndex(
                name: "IX_dongtien_UserID",
                table: "Dongtiens",
                newName: "IX_Dongtiens_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dongtiens",
                table: "Dongtiens",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dongtiens_Users_UserID",
                table: "Dongtiens",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dongtiens_Users_UserID",
                table: "Dongtiens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dongtiens",
                table: "Dongtiens");

            migrationBuilder.RenameTable(
                name: "Dongtiens",
                newName: "dongtien");

            migrationBuilder.RenameIndex(
                name: "IX_Dongtiens_UserID",
                table: "dongtien",
                newName: "IX_dongtien_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dongtien",
                table: "dongtien",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_dongtien_Users_UserID",
                table: "dongtien",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
