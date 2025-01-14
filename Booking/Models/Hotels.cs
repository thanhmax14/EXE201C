using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Hotels
    {
        [Key]
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
      /*  public decimal price { get; set; } = 0;*/
        public int availableRom { get; set; } = 0;
    }
}
