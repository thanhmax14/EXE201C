namespace Booking.Models
{
    public class dongtien
    {
        public  Guid ID { get; set; }= Guid.NewGuid();
        public decimal sotientruoc { get; set; } = 0;
        public decimal sotienthaydoi { get; set; } = 0;
        public decimal sotiensau { get; set; } = 0;
        public DateTime thoigian { get; set; }=DateTime.Now;
        public string? noidung { get; set; }
        public string? method { get; set; }
        public string? trangthai { get; set; }
        public long ordercode { get; set; } = 0;
        public string UserID { get; set; }
        public AppUser AppUser { get; set; }

    }
}
