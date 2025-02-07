using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Booking.ViewModels
{
    public class CreateHotelViewModel
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; } // Hotel, Villa, Apartment, etc.

 /*       public DateOnly? Established { get; set; }
        public DateOnly? Renovation { get; set; }*/

        [Range(1, 5)]
        public int StarRating { get; set; }

        [Range(1, 10000)]
        public int TotalRooms { get; set; }

        [Range(1, 10000)]
        public int MaxCapacity { get; set; }

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

        public List<string> Highlights { get; set; }= new List<string>() ;

        [Required]
        public List<bool> IsRoomService24h { get; set; } = new List<bool>();

        [Required]
        public List<bool> Accessibility { get; set; } = new List<bool>();

        [Required]
        public List<bool> RoomTypes { get; set; } = new List<bool>();

/*        [Required]
        public List<bool> Amenities { get; set; } = new List<bool>();*/

        [Required]
        public List<IFormFile> Images { get; set; } = new List<IFormFile> ();


    }
}
