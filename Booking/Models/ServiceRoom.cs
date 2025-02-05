using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class ServiceRoom
    {
        [Key]
        public Guid ServiceID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid RoomID { get; set; } // Khóa ngoại
        public string ServiceName { get; set; } // Tên dịch vụ

        // Navigation Property
        public Room Room { get; set; } // Liên kết đến Hotel
    }
}
