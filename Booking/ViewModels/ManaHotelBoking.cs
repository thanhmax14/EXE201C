namespace Booking.ViewModels
{
    public class ManaHotelBoking
    {
        public string orderCode { get; set; }
        public string? hotemName { get; set; }
        public string? addreddHotel { get; set; }
        public Guid HotelID { get; set; }
        public string? roomNAME { get; set; }
        public string? guest { get; set; }
        public string? user { get; set; }
        public string? addresUser { get; set; }
        public string?  date { get; set; }
        public decimal? price { get; set; } = 0;
        public string?  booked { get; set; }
        public string? status { get; set; }
        public Guid DatphongID { get; set; }


    }
}
