using Booking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace Booking.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Highlight> Highlights { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<ServiceRoom> ServiceRooms { get; set; }
        public DbSet<AmenityRoom> AmenityRooms { get; set; }
        public DbSet<AccessibilityRoom> AccessibilityRooms { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<GalleryRoom> GalleryRooms { get; set; }
        public DbSet<WishlistHotel> WishlistHotels { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

          modelBuilder.Entity<Hotel>()
         .HasOne(h => h.AppUser)
         .WithMany(h => h.Hotels)
         .HasForeignKey(h => h.UserID);
            // Thiết lập quan hệ 1-n
            modelBuilder.Entity<Highlight>()
                .HasOne(h => h.Hotel)
                .WithMany(h => h.Highlights)
                .HasForeignKey(h => h.HotelID);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Hotel)
                .WithMany(h => h.Services)
                .HasForeignKey(s => s.HotelID);

            modelBuilder.Entity<RoomType>()
                .HasOne(r => r.Hotel)
                .WithMany(h => h.RoomTypes)
                .HasForeignKey(r => r.HotelID);

            modelBuilder.Entity<Amenity>()
                .HasOne(a => a.Hotel)
                .WithMany(h => h.Amenities)
                .HasForeignKey(a => a.HotelID);




            modelBuilder.Entity<FAQ>()
                .HasOne(f => f.Hotel)
                .WithMany(h => h.FAQs)
                .HasForeignKey(f => f.HotelID);

            modelBuilder.Entity<Gallery>()
                .HasOne(g => g.Hotel)
                .WithMany(h => h.Galleries)
                .HasForeignKey(g => g.HotelID);

            modelBuilder.Entity<Room>()
                .HasOne(f => f.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(f => f.HotelID);



            modelBuilder.Entity<ServiceRoom>()
            .HasKey(a => a.ServiceID);
            modelBuilder.Entity<ServiceRoom>()
              .HasOne(s => s.Room)
              .WithMany(h => h.ServiceRooms)
              .HasForeignKey(s => s.RoomID);

            modelBuilder.Entity<AmenityRoom>()
          .HasKey(a => a.AmenityID);
            modelBuilder.Entity<AmenityRoom>()
                .HasOne(a => a.Room)
                .WithMany(h => h.AmenityRooms)
                .HasForeignKey(a => a.RoomID);

            modelBuilder.Entity<AccessibilityRoom>()
           .HasKey(a => a.AmenityID);
            modelBuilder.Entity<AccessibilityRoom>()
          .HasOne(a => a.Room)
          .WithMany(h => h.AccessibilityRooms)
          .HasForeignKey(a => a.RoomID);

            modelBuilder.Entity<GalleryRoom>()
           .HasKey(a => a.ImageID);
            modelBuilder.Entity<GalleryRoom>()
              .HasOne(s => s.Room)
              .WithMany(h => h.GalleryRooms)
              .HasForeignKey(s => s.RoomID);



            modelBuilder.Entity<WishlistHotel>().HasKey(a => a.ID);

            modelBuilder.Entity<WishlistHotel>()
        .HasOne(w => w.AppUser)
        .WithMany(h => h.WishlistHotels)
        .HasForeignKey(w => w.UserID)
        .OnDelete(DeleteBehavior.NoAction); 

         modelBuilder.Entity<WishlistHotel>()
        .HasOne(w => w.Hotel)
         .WithMany(h => h.WishlistHotels)
        .HasForeignKey(w => w.HotelID)
        .OnDelete(DeleteBehavior.NoAction);

            /*
                        // Dữ liệu mẫu cho bảng Services
                        modelBuilder.Entity<Service>().HasData(
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ phòng 24 giờ" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Ăn uống tại phòng" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ trợ giúp đặc biệt" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dọn phòng hàng ngày" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Quầy lễ tân 24 giờ" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Nhà hàng tại chỗ" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Hỗ trợ check-in/check-out" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ giữ hành lý miễn phí" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ giặt là và ủi đồ" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ giặt khô" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ làm tóc và làm đẹp" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ đỗ xe" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ trông trẻ" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ gọi báo thức" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ thông dịch viên" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Dịch vụ đổi tiền tệ" },
                            new Service { ServiceID = Guid.NewGuid(), HotelID = Guid.Empty, ServiceName = "Trị liệu spa trong phòng" }
                        );

                        // Dữ liệu mẫu cho bảng Accessibility
                        modelBuilder.Entity<Amenity>().HasData(
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Có lối đi cho xe lăn" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Hệ thống cảnh báo bằng hình ảnh trong hành lang" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Thang máy" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Ký hiệu nổi hoặc chữ Braille" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Phòng tắm có ghế ngồi hoặc ghế băng" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Thanh vịn gần bồn vệ sinh và vòi sen" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Phòng gym có lối đi cho xe lăn" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Trung tâm dịch vụ doanh nhân có lối đi cho xe lăn" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Phòng khách có lối đi cho xe lăn" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Bàn tiếp tân có lối đi cho xe lăn" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Thanh treo khăn ở vị trí thấp" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Chữ Braille trên số phòng, nút thang máy" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Lối vào không bậc hoặc có dốc" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Cửa tự động" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Thanh vịn trong phòng tắm" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Cửa rộng" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Công tắc ở vị trí thấp" },
                            new Amenity { AmenityID = Guid.NewGuid(), HotelID = Guid.Empty, AmenityName = "Rèm cửa dễ tiếp cận" }
                        );


                        // Dữ liệu mẫu cho bảng RoomTypes
                        // Dữ liệu mẫu cho bảng RoomTypes
                        modelBuilder.Entity<RoomType>().HasData(
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng đơn" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng đôi" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng hai giường đơn" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng Deluxe" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Suite" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng Junior Suite" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng gia đình" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng thông nhau" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng dễ tiếp cận" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng Studio" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Căn hộ Penthouse" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Biệt thự" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng tiết kiệm" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng nhìn ra thành phố" },
                            new RoomType { RoomTypeID = Guid.NewGuid(), HotelID = Guid.Empty, RoomTypeName = "Phòng nhìn ra biển" }
                        );


                        // Dữ liệu mẫu cho bảng FAQ
                        modelBuilder.Entity<FAQ>().HasData(
                            new FAQ { FAQID = Guid.NewGuid(), HotelID = Guid.Empty, Question = "Khách sạn có hỗ trợ hủy miễn phí không?", Answer = "Có, nếu hủy trước 24 giờ." },
                            new FAQ { FAQID = Guid.NewGuid(), HotelID = Guid.Empty, Question = "Khách sạn có hồ bơi không?", Answer = "Có, hồ bơi nằm ở tầng 3." },
                            new FAQ { FAQID = Guid.NewGuid(), HotelID = Guid.Empty, Question = "Khách sạn có cung cấp dịch vụ đưa đón sân bay không?", Answer = "Có, vui lòng liên hệ trước để sắp xếp." },
                            new FAQ { FAQID = Guid.NewGuid(), HotelID = Guid.Empty, Question = "Khách sạn có cung cấp bữa sáng không?", Answer = "Có, bữa sáng được phục vụ tại nhà hàng từ 6:30 đến 10:00." }
                        );
            */









        }
    }
}
