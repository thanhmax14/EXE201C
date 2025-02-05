using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Amenity
    {
        [Key]
        public Guid AmenityID { get; set; } = Guid.NewGuid(); // Khóa chính

        public Guid HotelID { get; set; } // Khóa ngoại
        public string AmenityName { get; set; } // Tên tiện nghi

        // Navigation Property
        public Hotel Hotel { get; set; } // Liên kết đến Hotel 
    }
}
