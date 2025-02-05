using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Gallery
    {
        [Key]
        public Guid ImageID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid HotelID { get; set; } // Khóa ngoại
        public string ImagePath { get; set; } // Đường dẫn ảnh
        public bool IsFeatureImage { get; set; } // Có phải ảnh chính hay không

        // Navigation Property
        public Hotel Hotel { get; set; } // Liên kết đến Hotel
    }
}
