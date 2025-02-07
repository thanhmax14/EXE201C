namespace Booking.ViewModels
{
    public class WishListTour
    {

        public Guid ID { get; set; }
        public string? TourName { get; set; }
        public string? Location { get; set; }
        public string? Descriptions { get; set; }
        public decimal price { get; set; } = 0;
        public List<string?> img { get; set; } = new List<string?>();
        public string rating { get; set; } = "5.0";
        public int NumberReview { get; set; } = 0;
        public string? NameSeller { get; set; }
        public string category { get; set; }
        public string date { get; set; }
    }
}
