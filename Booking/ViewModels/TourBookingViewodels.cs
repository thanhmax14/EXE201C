namespace Booking.ViewModels
{
    public class TourBookingViewodels
    {
        public string? OrderID { get; set; }
        public string img { get; set; }
        public string? TourName { get; set; }
        public string? Category { get; set; }
        public Guid TourID { get; set; }
        public string? Guest { get; set; }
        public string? date { get; set; }
        public decimal? price { get; set; } = 0;
        public DateTime? Booked { get; set; }
        public string? status { get; set; }
    }
}
