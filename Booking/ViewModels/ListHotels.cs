namespace Booking.ViewModels
{
    public class ListHotels
    {
        public Guid HotelID { get; set; }
        public  string? HotelName { get; set; }
        public  string? Location { get; set; }
        public decimal price { get; set; } = 0;
        public string? NameSeller { get; set; }
        public string? Rating { get; set; } = "5.0";
        public int NumberReview { get; set; } = 0;
        public string farovite { get; set; } = "";
        public List<GalleriesImg> img { get; set; }
    }
}
