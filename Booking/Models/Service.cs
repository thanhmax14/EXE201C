using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Service
    {
        [Key]
        public Guid ServiceID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid HotelID { get; set; } // Khóa ngoại
        public string ServiceName { get; set; } // Tên dịch vụ

        // Navigation Property
        public Hotel Hotel { get; set; } // Liên kết đến Hotel
    }
}
