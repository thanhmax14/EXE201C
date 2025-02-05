namespace Booking.ViewModels
{
    public class HotelBookingViewodels
    {
        public string? OrderID { get; set; }
        public string img { get; set; }
        public string? HotelName { get; set; }
        public string? location { get; set; }
        public Guid hotelID { get; set; }
        public string? RomeName { get; set; }
        public string? Guest { get; set; }
        public string? date { get; set; }
        public decimal? price { get; set; } = 0;
        public DateTime? Booked { get; set; }
        public string? status { get; set; }
    }
}
