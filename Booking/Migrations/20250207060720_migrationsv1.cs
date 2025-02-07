using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationsv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    joinin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    sinhNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    firstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestSeller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isUpdateProfile = table.Column<bool>(type: "bit", nullable: false),
                    IsBanByadmin = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dongtiens",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sotientruoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sotienthaydoi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sotiensau = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    thoigian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    noidung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    trangthai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ordercode = table.Column<long>(type: "bigint", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dongtiens", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Dongtiens_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HotelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Established = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Renovation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StarRatings = table.Column<int>(type: "int", nullable: false),
                    TotalRooms = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    linkLocation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Hotels_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TourName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    startDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDATE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationNight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    totalPreoPle = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    minAge = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    linkLocation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tours_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        principalColumn: "ID",
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
                        principalColumn: "ID",
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
                        principalColumn: "ID",
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
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    OrderID = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaximumOccupancy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomSize = table.Column<float>(type: "real", nullable: false),
                    Sleeps = table.Column<int>(type: "int", nullable: false),
                    BedType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    View = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomID);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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
                        principalColumn: "ID",
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
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlistHotels",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HotelID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistHotels", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WishlistHotels_Hotels_HotelID",
                        column: x => x.HotelID,
                        principalTable: "Hotels",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WishlistHotels_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivitiesName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.id);
                    table.ForeignKey(
                        name: "FK_Activities_Tours_TourID",
                        column: x => x.TourID,
                        principalTable: "Tours",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Excludes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExcludesName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Excludes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Excludes_Tours_TourID",
                        column: x => x.TourID,
                        principalTable: "Tours",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GalleryTours",
                columns: table => new
                {
                    ImageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFeatureImage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryTours", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_GalleryTours_Tours_TourID",
                        column: x => x.TourID,
                        principalTable: "Tours",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Includes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TourID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncludesName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Includes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Includes_Tours_TourID",
                        column: x => x.TourID,
                        principalTable: "Tours",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewTours",
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
                    TourID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTours", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReviewTours_Tours_TourID",
                        column: x => x.TourID,
                        principalTable: "Tours",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReviewTours_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WishlistTours",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TourID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistTours", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WishlistTours_Tours_TourID",
                        column: x => x.TourID,
                        principalTable: "Tours",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_WishlistTours_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id");
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
                        name: "FK_AccessibilityRooms_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
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
                        name: "FK_AmenityRooms_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    progress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isComment = table.Column<bool>(type: "bit", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "GalleryRooms",
                columns: table => new
                {
                    ImageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoomID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFeatureImage = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryRooms", x => x.ImageID);
                    table.ForeignKey(
                        name: "FK_GalleryRooms_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
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
                        name: "FK_ServiceRooms_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "RoomID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessibilityRooms_RoomID",
                table: "AccessibilityRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_TourID",
                table: "Activities",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_HotelID",
                table: "Amenities",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_AmenityRooms_RoomID",
                table: "AmenityRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_DaTours_TourID",
                table: "DaTours",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_DaTours_UserID",
                table: "DaTours",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Datphongs_RoomID",
                table: "Datphongs",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Datphongs_UserID",
                table: "Datphongs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Dongtiens_UserID",
                table: "Dongtiens",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Excludes_TourID",
                table: "Excludes",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_HotelID",
                table: "FAQs",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_HotelID",
                table: "Galleries",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryRooms_RoomID",
                table: "GalleryRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_GalleryTours_TourID",
                table: "GalleryTours",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_Highlights_HotelID",
                table: "Highlights",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_UserID",
                table: "Hotels",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Includes_TourID",
                table: "Includes",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHotels_HotelID",
                table: "ReviewHotels",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewHotels_UserID",
                table: "ReviewHotels",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTours_TourID",
                table: "ReviewTours",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTours_UserID",
                table: "ReviewTours",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelID",
                table: "Rooms",
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

            migrationBuilder.CreateIndex(
                name: "IX_Tours_UserID",
                table: "Tours",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistHotels_HotelID",
                table: "WishlistHotels",
                column: "HotelID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistHotels_UserID",
                table: "WishlistHotels",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistTours_TourID",
                table: "WishlistTours",
                column: "TourID");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistTours_UserID",
                table: "WishlistTours",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessibilityRooms");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "AmenityRooms");

            migrationBuilder.DropTable(
                name: "DaTours");

            migrationBuilder.DropTable(
                name: "Datphongs");

            migrationBuilder.DropTable(
                name: "Dongtiens");

            migrationBuilder.DropTable(
                name: "Excludes");

            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "GalleryRooms");

            migrationBuilder.DropTable(
                name: "GalleryTours");

            migrationBuilder.DropTable(
                name: "Highlights");

            migrationBuilder.DropTable(
                name: "Includes");

            migrationBuilder.DropTable(
                name: "ReviewHotels");

            migrationBuilder.DropTable(
                name: "ReviewTours");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "ServiceRooms");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "WishlistHotels");

            migrationBuilder.DropTable(
                name: "WishlistTours");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
