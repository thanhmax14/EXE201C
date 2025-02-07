namespace Booking.Models
{
    public class ReviewTour
    {
        public Guid ID { get; set; } = Guid.NewGuid(); // Khóa chính
        public string? cmt { get; set; }
        public DateTime datecmt { get; set; } = DateTime.Now;

        public string? relay { get; set; }
        public DateTime dateRelay { get; set; } = DateTime.Now;
        public bool status { get; set; } = false;
        public int rating { get; set; } = 5;
        public string UserID { get; set; }
        public Guid TourID { get; set; } // Liên kết với bảng Hotels
        public tour Tour { get; set; } // Liên kết đến Hotel
        public AppUser AppUser { get; set; }
    }
}
