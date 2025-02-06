namespace Booking.ViewModels
{
    public class DashBoardUser
    {
        public int numberBoking { get; set; } = 0;
        public decimal totalTrans { get; set; } = 0;
        public List<RecentBookings> RecentBookings { get; set; } = new List<RecentBookings>();

        public List<(string, string, string, Guid,DateTime?,decimal?,string)> RecentInvoices { get; set; } = new();


    }
}
