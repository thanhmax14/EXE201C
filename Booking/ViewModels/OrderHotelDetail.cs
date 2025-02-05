namespace Booking.ViewModels
{
    public class OrderHotelDetail
    {


        public Guid HotelID { get; set; }
        public string? img { get; set; }
        public string? NameRoom { get; set; }
        public DateTime checkIn { get; set; }
        public DateTime checkOut { get; set; }
        public int Adults { get; set; } = 0;
        public int Infants{ get; set; } = 0;
        public int Children { get; set; } = 0;

        public int Tax { get; set; } = 0;
        public int BookingFees { get; set; } = 0;
        public int Discount { get; set; } = 0;
        public decimal total { get; set; } = 0;





    }
}
