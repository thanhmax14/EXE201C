using System.ComponentModel.DataAnnotations;

namespace Booking.ViewModels
{
    public class CreateTourViewModeks
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; } // Hotel, Villa, Apartment, etc.

        [Required]
        public string startDate{ get; set; }

        [Required]
        public string endDate { get; set; }

        [Required]
        public string Destination { get; set; }

        [Required]
        public int totalProple { get; set; }
        [Required]
        public decimal Pricing { get; set; } = 0;

        [Required]
        public int mindAge { get; set; }
        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(100)]
        public string City { get; set; }
        [Required]
        public string linkLocation { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }
        [Required]
        public string dess { get; set; }
        [Required]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
