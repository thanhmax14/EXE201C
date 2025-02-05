namespace Booking.ViewModels
{
    public class RoomView
    {
        public Guid RoomID { get; set; }
        public string? RoomName { get; set; }
        List<string> shortService  { get; set; }
        public decimal newPrice { get; set; } = 0;
        public decimal oldPrice { get; set; } = 0;
        public List<string?> imgRoom { get; set; } = new List<string?>();
    }
}
