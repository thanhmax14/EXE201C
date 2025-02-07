using System.ComponentModel.DataAnnotations;

namespace Booking.Models
{
    public class tour
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid(); // Khóa chính
        public string UserID { get; set; }
        public string TourName { get; set; } // Tên khách sạn
        public string Category { get; set; } // Phân loại
        public string? startDate { get; set; } // Ngày thành lập
        public string? EndDATE { get; set; } // Ngày thành lập
        public string? Destination { get; set; } // Số sao
        public string? DurationDay { get; set; } // Số sao
        public string? DurationNight { get; set; } // Số sao
        public int totalPreoPle { get; set; } = 0;
        public decimal price { get; set; } = 0;
        public int minAge { get; set; } = 0;

        public string Country { get; set; } // Quốc gia
        public string City { get; set; } // Thành phố
        public string State { get; set; } // Bang/Tỉnh
        public string ZipCode { get; set; } // Mã bưu điện
        public string Address { get; set; } // Địa chỉ chính
        public string Description { get; set; } // Mô tả chi tiết
        public string? linkLocation { get; set; } = "";

        // Navigation Properties
        public ICollection<Activities> Activities { get; set; }
        public ICollection<Includes> Includes { get; set; }
        public ICollection<Excludes> Excludes { get; set; }
        public ICollection<WishlistTour> WishlistTours { get; set; }
        public ICollection<GalleryTour> GalleryTours { get; set; }
        public ICollection<DaTour> DaTours { get; set; }
        public ICollection<ReviewTour> ReviewTours { get; set; }
      
        public AppUser AppUser { get; set; }
    }
}
