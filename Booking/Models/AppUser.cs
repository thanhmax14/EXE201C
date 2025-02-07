using Microsoft.AspNetCore.Identity;

namespace Booking.Models
{
    public class AppUser: IdentityUser
    {
        public string? address { get; set; }
        public string? img { get; set; } = "assets/img/users/user-01.jpg";
        public DateTime? joinin { get; set; }= DateTime.Now;
        public DateTime? sinhNhat { get; set; }
        public string? ZipCode { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? RequestSeller { get; set; }
        public bool isUpdateProfile { get; set; } = false;
        public bool IsBanByadmin { get; set; } = false;
        public ICollection<Hotel> Hotels { get; set; }
        public ICollection<tour> Tours { get; set; }
        public ICollection<WishlistTour> WishlistTours { get; set; }
        public ICollection<WishlistHotel> WishlistHotels { get; set; }
        public ICollection<ReviewHotels> ReviewHotels { get; set; }
        public ICollection<dongtien> Dongtiens { get; set; }
        public ICollection<datphong> Datphongs { get; set; }
        public ICollection<DaTour> DaTours { get; set; }
        public ICollection<ReviewTour> ReviewTours { get; set; }
    }
}
