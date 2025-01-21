using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
