namespace Booking.ViewModels
{
    public class OrderTourDetail
    {
        public Guid TourID { get; set; }
        public string? img { get; set; }
        public string? NameTour { get; set; }
        public string Departure { get; set; }
        public string Return { get; set; }
        public int Adults { get; set; } = 0;
        public int Infants { get; set; } = 0;
        public int Children { get; set; } = 0;
        public string NoOfdate { get; set; }
        public int Tax { get; set; } = 0;
        public int BookingFees { get; set; } = 0;
        public int Discount { get; set; } = 0;
        public decimal total { get; set; } = 0;

    }
}
