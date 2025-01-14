using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class Tours
    {
        [Key]
        public Guid Id { get; set; }  
        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; } = 0;

        public string Duration { get; set; }  // Thời gian tour, ví dụ "3 ngày 2 đêm"

        public int AvailableSeats { get; set; } = 0;
    }
}
