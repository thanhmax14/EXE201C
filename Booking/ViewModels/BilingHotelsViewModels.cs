namespace Booking.ViewModels
{
    public class BilingHotelsViewModels
    {
        public string? id { get; set; }

        public string? img { get; set; }
        public string? firstName { get; set; }

        public string? lastName { get; set; }

        public string? address { get; set; }
        public DateTime? birthday { get; set; }
        public string? zipcode { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public List<OrderHotelDetail> list { get; set; } = new List<OrderHotelDetail>();
    }
}
