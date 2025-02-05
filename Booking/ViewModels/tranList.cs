namespace Booking.ViewModels
{
    public class tranList
    {
        public string? paymentType { get; set; }
        public string? type { get; set; }
        public DateTime? time { get; set; }= DateTime.Now;
        public decimal? amount { get; set; } = 0;
        public string? Balance { get; set; }
        public string? detail { get; set; }
        public string? trangthai { get; set; }

    }
}
