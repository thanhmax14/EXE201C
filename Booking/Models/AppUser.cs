using Microsoft.AspNetCore.Identity;

namespace Booking.Models
{
    public class AppUser: IdentityUser
    {
        public string? address { get; set; }
    }
}
