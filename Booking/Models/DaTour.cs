namespace Booking.Models
{
    public class DaTour
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string OrderID { get; set; }
        public string? Guests { get; set; }
        public DateTime? BookedOn { get; set; }
       
        public string? NoOfDate { get; set; }
        public string? messess { get; set; }
        public string paymentStatus { get; set; }
        public DateTime? DatePayment { get; set; }
        public int tax { get; set; } = 0;
        public int Discount { get; set; } = 0;
        public int BookingFees { get; set; } = 0;
        public decimal? totalPaid { get; set; } = 0;
        public string UserID { get; set; }
        public Guid TourID { get; set; }
        public string progress { get; set; }
        public bool isComment { get; set; } = false;
        public tour Tour { get; set; }

        public AppUser AppUser { get; set; }
    }
}
