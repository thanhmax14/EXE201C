using Microsoft.AspNetCore.Identity;

namespace Booking.Models
{
    public class AppUser: IdentityUser
    {
        public string? address { get; set; }
        public ICollection<Hotel> Hotels { get; set; }
        public ICollection<WishlistHotel> WishlistHotels { get; set; }
    }
}
