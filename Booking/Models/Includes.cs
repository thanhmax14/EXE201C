using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Includes
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid TourID { get; set; } // Khóa ngoại
        public string IncludesName { get; set; } // Tên tiện nghi

        // Navigation Property
        public tour Tour { get; set; } // Liên kết đến Hotel 
    }
}
