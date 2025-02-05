using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class RoomType
    {
        [Key]
        public Guid RoomTypeID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid HotelID { get; set; } // Khóa ngoại
        public string RoomTypeName { get; set; } // Tên loại phòng

        // Navigation Property
        public Hotel Hotel { get; set; } // Liên kết đến Hotel
    }
}
