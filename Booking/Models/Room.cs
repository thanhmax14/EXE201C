using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Room
    {
        [Key]
        public Guid RoomID { get; set; }
        public Guid HotelID { get; set; } // Liên kết với bảng Hotels
        public string RoomName { get; set; }
        public string RoomType { get; set; } // Loại phòng
        public string Description { get; set; }
        public int MaximumOccupancy { get; set; }
        public float RoomSize { get; set; }
        public int Sleeps { get; set; }
        public string BedType { get; set; }
        public string View { get; set; }
        public decimal PricePerNight { get; set; }
        public Hotel Hotel { get; set; } // Liên kết đến Hotel
        public ICollection<ServiceRoom> ServiceRooms { get; set; }
        public ICollection<AmenityRoom> AmenityRooms { get; set; }
        public ICollection<AccessibilityRoom> AccessibilityRooms { get; set; }
        public ICollection<GalleryRoom> GalleryRooms { get; set; }
    }
}
