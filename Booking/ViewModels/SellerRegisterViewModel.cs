using System.ComponentModel.DataAnnotations;

namespace Booking.ViewModels
{
    public class SellerRegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }
        [Required]
        public DateTime Birthday { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Ward { get; set; }
        [Required]
        public string Address { get; set; }

        [Required]
        public bool AcceptTerms { get; set; }
    }
}
