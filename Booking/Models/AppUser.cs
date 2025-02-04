using Microsoft.AspNetCore.Identity;

namespace Booking.Models
{
    public class AppUser: IdentityUser
    {
        public string? address { get; set; }
        public string? img { get; set; } = "assets/img/users/user-01.jpg";
        public DateTime? joinin { get; set; }= DateTime.Now;
        public ICollection<Hotel> Hotels { get; set; }
        public ICollection<WishlistHotel> WishlistHotels { get; set; }
        public ICollection<ReviewHotels> ReviewHotels { get; set; }
        public ICollection<dongtien> Dongtiens { get; set; }
    }
}
