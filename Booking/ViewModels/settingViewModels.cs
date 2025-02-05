using System.ComponentModel.DataAnnotations;

namespace Booking.ViewModels
{
    public class settingViewModels
    {
        public string? id { get; set; }

        [StringLength(255, ErrorMessage = "Image URL cannot be longer than 255 characters.")]
        public string? img { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string? firstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string? lastName { get; set; }

        // Address is now required
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot be longer than 255 characters.")]
        public string? address { get; set; }
        [Required(ErrorMessage = "Birthday is required.")]

        [DataType(DataType.Date)]
        public DateTime? birthday { get; set; }

        [StringLength(10, MinimumLength = 5, ErrorMessage = "Zipcode should be between 5 and 10 characters.")]
        public string? zipcode { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string? phone { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "District is required.")]
        public string? District { get; set; }

        [Required(ErrorMessage = "Ward is required.")]
        public string? Ward { get; set; }
    }
}
