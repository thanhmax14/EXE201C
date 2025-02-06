namespace Booking.ViewModels
{
    public class invoicesViewModels
    {

        public string? orderID { get; set; }
        public DateTime? creteDate { get; set; }
        public DateTime? duedate { get; set; }
        public string? UserName { get; set; }
        public string? address { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public List<(string, decimal?, decimal?, decimal?)> list { get; set; } = new();

        public decimal subTotal { get; set; } = 0;
        public int discount { get; set; } = 0;
        public int vat { get; set; } = 0;

        public decimal? Total { get; set; } = 0;
        public string status { get; set; }

    }
}
