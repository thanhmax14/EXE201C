using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Data.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Hotels");

            migrationBuilder.RenameColumn(
                name: "availableRom",
                table: "Hotels",
                newName: "TotalRooms");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Hotels",
                newName: "HotelID");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Established",
                table: "Hotels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxCapacity",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Renovation",
                table: "Hotels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StarRatings",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    AmenityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmenityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.AmenityID);
                    table.ForeignKey(
                        name: "FK_Amenities_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    FAQID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.FAQID);
                    table.ForeignKey(
                        name: "FK_FAQs_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    ImageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFeatureImage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_Galleries_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Highlights",
                columns: table => new
                {
                    HighlightID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HighlightText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Highlights", x => x.HighlightID);
                    table.ForeignKey(
                        name: "FK_Highlights_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaximumOccupancy = table.Column<int>(type: "int", nullable: false),
                    RoomSize = table.Column<float>(type: "real", nullable: false),
                    Sleeps = table.Column<int>(type: "int", nullable: false),
                    BedType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    View = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.RoomID);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    RoomTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.RoomTypeID);
                    table.ForeignKey(
                        name: "FK_RoomTypes_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceID);
                    table.ForeignKey(
                        name: "FK_Services_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "HotelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccessibilityRooms",
                columns: table => new
                {
                    AmenityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmenityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessibilityRooms", x => x.AmenityID);
                    table.ForeignKey(
                        name: "FK_AccessibilityRooms_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AmenityRooms",
                columns: table => new
                {
                    AmenityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmenityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmenityRooms", x => x.AmenityID);
                    table.ForeignKey(
                        name: "FK_AmenityRooms_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRooms",
                columns: table => new
                {
                    ServiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRooms", x => x.ServiceID);
                    table.ForeignKey(
                        name: "FK_ServiceRooms_Room_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Room",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessibilityRooms_RoomID",
                table: "AccessibilityRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_HotelID",
                table: "Amenities",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_AmenityRooms_RoomID",
                table: "AmenityRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_HotelID",
                table: "FAQs",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_HotelID",
                table: "Galleries",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Highlights_HotelID",
                table: "Highlights",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomTypes_HotelID",
                table: "RoomTypes",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRooms_RoomID",
                table: "ServiceRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Services_HotelID",
                table: "Services",
                column: "HotelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessibilityRooms");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "AmenityRooms");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "Highlights");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "ServiceRooms");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Address1",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Established",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HotelName",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Renovation",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "StarRatings",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "Hotels");

            migrationBuilder.RenameColumn(
                name: "TotalRooms",
                table: "Hotels",
                newName: "availableRom");

            migrationBuilder.RenameColumn(
                name: "HotelID",
                table: "Hotels",
                newName: "ID");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    available = table.Column<bool>(type: "bit", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelsID",
                        column: x => x.HotelsID,
                        principalTable: "Hotels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelsID",
                table: "Rooms",
                column: "HotelsID");
        }
    }
}
