namespace Booking.ViewModels
{
    public class walletViewModels
    {
        public decimal tien { get; set; } = 0;
        public int numberTrans { get; set; } = 0;
        public List<tranList?> list { get; set; } = new List<tranList?>();
    }
}
