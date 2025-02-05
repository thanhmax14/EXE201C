    namespace Booking.ViewModels
    {
        public class BookingRequest
        {
            public List<Guid> rooms { get; set; } 
            public string CheckIn { get; set; }  
            public string CheckOut { get; set; } 

            public GuestInfo Guests { get; set; } 
        }

        public class GuestInfo
        {
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }
        }
    }
