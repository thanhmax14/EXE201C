namespace Booking.ViewModels
{
    public class TourDetail
    {
        public Guid TourID { get; set; }
        public string? TorName { get; set; }
        public string? Category { get; set; }
        public string? Locations { get; set; }
        public string? LocationsURL { get; set; }
        public int TotalPreople { get; set; } = 0;
        public List<string> imgView { get; set; } = new List<string>();
        public string? Descriptions { get; set; }
        public List<string?> Excludes { get; set; } = new List<string?>();
        public List<string?> Includes { get; set; } = new List<string?>();
        public List<string?> Activities { get; set; } = new List<string?>(); 
        public List<string?> Gallery { get; set; } = new List<string?>();
        public string? SellerName { get; set; }
        public DateTime? dateCreateSeller { get; set; }
        public string? PhoneSeller { get; set; }
        public string? EmailSeller { get; set; }
        public string? status { get; set; } = "Verified";
        public string farovite { get; set; } = "";
        public bool isComment { get; set; } = false;
        public string strarDate { get; set; }
        public string endDate { get; set; }
        public string duration { get; set; }
        public string ngaydi { get; set; }
        public string ngaive { get; set; }
        public decimal price { get; set; } = 0;


    }
}
