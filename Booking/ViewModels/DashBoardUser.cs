namespace Booking.ViewModels
{
    public class DashBoardUser
    {
        public int numberBoking { get; set; } = 0;
        public decimal totalTrans { get; set; } = 0;
        public List<RecentBookings> RecentBookings { get; set; } = new List<RecentBookings>();


    }
}
