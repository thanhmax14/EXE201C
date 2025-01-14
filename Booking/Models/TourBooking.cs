using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models
{
    public class TourBooking
    {
        public Guid Id { get; set; }
        [ForeignKey("AppUser")]
        public string UserId { get; set; }
        public AppUser User { get; set; }  // Ánh xạ khóa ngoại tới AppUser
        [ForeignKey("Tours")]
        public Guid TourId { get; set; }
        public Tours Tour { get; set; }  // Ánh xạ khóa ngoại tới Tour
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public int NumOfPeople { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;

        public virtual Hotels Hotels { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
