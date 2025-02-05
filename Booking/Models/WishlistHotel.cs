namespace Booking.Models
{
    public class WishlistHotel
    {
        public Guid ID { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UserID { get; set; }
        public Guid HotelID { get; set; } // Liên kết với bảng Hotels
        public AppUser AppUser { get; set; }
        public Hotel Hotel { get; set; } // Liên kết đến Hotel

    }
}
