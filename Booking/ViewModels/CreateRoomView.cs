using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Booking.ViewModels
{
    public class CreateRoomView
    {
        // Basic Info
        [Required(ErrorMessage = "Vui lòng nhập tên phòng.")]
        public string RoomName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại phòng.")]
        public string RoomType { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khách sạn.")]
        public Guid HotelID { get; set; }

        public SelectList Hotels { get; set; } = new SelectList(new List<SelectListItem>());

        public string Description { get; set; }

        // Specifications
        public string MaximumOccupancy { get; set; }
        public float RoomSize { get; set; }
        public int Sleeps { get; set; }
        public string BedType { get; set; }
        public string View { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá phòng.")]
        public decimal PricePerNight { get; set; }

        // Services (Dịch vụ phòng)
        public List<bool> Services { get; set; } = new List<bool>();

        // Accessibility (Tiện ích tiếp cận)
        public List<bool> Accessibility { get; set; } = new List<bool>();

        // Amenities (Tiện nghi)
        public List<bool> Amenities { get; set; } = new List<bool>();

        // Hình ảnh
        public List<IFormFile> GalleryImages { get; set; } = new List<IFormFile>();
    }
}
