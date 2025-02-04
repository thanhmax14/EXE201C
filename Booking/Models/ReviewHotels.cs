namespace Booking.Models
{
    public class ReviewHotels
    {
        public Guid ID { get; set; } = Guid.NewGuid(); // Khóa chính
        public string? cmt { get; set; }
        public DateTime datecmt { get; set; } = DateTime.Now;

        public string? relay { get; set; }
        public DateTime dateRelay { get; set; } = DateTime.Now;
        public bool status { get; set; } = false;
        public int rating { get; set; } = 5;


        public string UserID { get; set; }
        public Guid HotelID { get; set; } // Liên kết với bảng Hotels
        public Hotel Hotel { get; set; } // Liên kết đến Hotel
        public AppUser AppUser { get; set; }
    }
}
