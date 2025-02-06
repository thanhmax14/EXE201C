using Booking.Models;

namespace Booking.ViewModels
{
    public class HotelsDetail
    {
        public Guid HotelID { get; set; }
        public string? HotelName { get; set; }
        public string? HotelTye { get; set; }
        public string? Locations { get; set; }
        public string? LocationsURL { get; set; }
        public int TotalRom { get; set; } = 0;
        public List<string> imgView { get; set; } = new List<string>();
        public string? Descriptions { get; set; }
        public List<string?> Highlights { get; set; }= new List<string?>();
        public List<string?> Amenities { get; set; } = new List<string?>();
        public List<string?> Roomtypes { get; set; } = new List<string?>();
        public List<string?> Services { get; set; } = new List<string?>();
        public List<string?> Gallery { get; set; } = new List<string?>();
        public Dictionary<string, string> faq { get; set; } = new Dictionary<string, string>();

        public string? SellerName { get; set; }
        public DateTime? dateCreateSeller { get; set; }
        public string? PhoneSeller { get; set; }
        public string? EmailSeller { get; set; }
        public string? status { get; set; } = "Verified";
        public string farovite { get; set; } = "";
        public bool isComment { get; set; } = false;
        public List<RoomView> Room { get; set; } = new List<RoomView>();
        public List<readcmt> readcmt { get; set; } = new List<readcmt>();


    }
}
