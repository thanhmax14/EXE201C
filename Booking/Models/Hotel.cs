using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Hotel
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid(); // Khóa chính
        public string UserID { get; set; }
        public string HotelName { get; set; } // Tên khách sạn
        public string Category { get; set; } // Phân loại
        public DateTime? Established { get; set; } // Ngày thành lập
        public DateTime? Renovation { get; set; } // Ngày cải tạo
        public int StarRatings { get; set; } // Số sao
        public int TotalRooms { get; set; } // Tổng số phòng
        public int MaxCapacity { get; set; } // Sức chứa tối đa

        public string Country { get; set; } // Quốc gia
        public string City { get; set; } // Thành phố
        public string State { get; set; } // Bang/Tỉnh
        public string ZipCode { get; set; } // Mã bưu điện
        public string Address { get; set; } // Địa chỉ chính
        public string Address1 { get; set; } // Địa chỉ phụ
        public string Description { get; set; } // Mô tả chi tiết

        // Navigation Properties
        public ICollection<Highlight> Highlights { get; set; }
        public ICollection<Service> Services { get; set; }
        public ICollection<RoomType> RoomTypes { get; set; }
        public ICollection<Amenity> Amenities { get; set; }
        public ICollection<WishlistHotel> WishlistHotels { get; set; }
        public ICollection<FAQ> FAQs { get; set; }
        public ICollection<Gallery> Galleries { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public AppUser AppUser { get; set; }
    }
}
