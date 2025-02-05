using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class AccessibilityRoom
    {
        [Key]
        public Guid AmenityID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid RoomID { get; set; } // Khóa ngoại
        public string AmenityName { get; set; } // Tên tiện nghi

        // Navigation Property
        public Room Room { get; set; } // Liên kết đến Hotel 
    }
}
