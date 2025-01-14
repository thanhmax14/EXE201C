using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models
{
    public class HotelBooking
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("AppUser")]
        public string UserId { get; set; }
        public AppUser User { get; set; }
        [ForeignKey("Hotels")]
        public Guid HotelId { get; set; }
        public Hotels Hotel { get; set; }
        [ForeignKey("Rooms")]
        public Guid RoomId { get; set; }
        public Rooms Room { get; set; }
        public DateTime CheckInDate { get; set; } = DateTime.Now;
        public DateTime CheckOutDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public virtual Hotels Hotels { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual Rooms Rooms { get; set; }

    }
}
