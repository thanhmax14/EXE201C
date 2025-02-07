using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class WishlistTour
    {
        [Key]
        public Guid ID { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UserID { get; set; }
        public Guid TourID { get; set; } // Khóa ngoại
        public AppUser AppUser { get; set; }
        public tour Tour { get; set; } // Liên kết đến Hotel 
    }
}
