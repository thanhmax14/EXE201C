using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class FAQ
    {
        [Key]
        public Guid FAQID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid HotelID { get; set; } // Khóa ngoại
        public string Question { get; set; } // Câu hỏi
        public string Answer { get; set; } // Câu trả lời

        // Navigation Property
        public Hotel Hotel { get; set; } // Liên kết đến Hotel
    }
}
