using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Highlight
    {
        [Key]
        public Guid HighlightID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid HotelID { get; set; } // Khóa ngoại
        public string HighlightText { get; set; } // Nội dung điểm nổi bật

        // Navigation Property
        public Hotel Hotel { get; set; } // Liên kết đến Hotel
    }
}
