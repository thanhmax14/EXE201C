using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Booking.Models
{
    public class Rooms
    {
        [Key]
        public Guid ID { get; set; }

        public string RoomType { get; set; } // Single,Double,Four People

        public int Capacity { get; set; } = 1;  // Sức chứa phòng
        public bool available { get; set; } = false;
        public DateTime? CreateDate { get; set; }
        public DateTime? Modify { get; set; }
        public decimal price { get; set; } = 0;
        [ForeignKey("Hotels")]
        public Guid HotelsID { get; set; }
        public virtual Hotels Hotels { get; set; }
    }
}
