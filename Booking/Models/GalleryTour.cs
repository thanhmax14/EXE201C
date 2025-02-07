using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class GalleryTour
    {
        [Key]
        public Guid ImageID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid TourID { get; set; } // Khóa ngoại
        public string ImagePath { get; set; } // Đường dẫn ảnh
        public bool IsFeatureImage { get; set; } // Có phải ảnh chính hay không

        // Navigation Property
        public tour Tour { get; set; } // Liên kết đến Hotel 
    }
}
