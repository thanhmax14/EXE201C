using Booking.Data;
using Booking.Models;
using Booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Settings()
        {
            return View();
        }
       
        public async Task<IActionResult> Wishlist()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var list = new List<WishList>();
            var getWishHotel = await this._context.WishlistHotels
        .Where(h => h.UserID == user.Id)
        .OrderBy(h => h.CreateDate)
        .ToListAsync();
            
             foreach(var h in getWishHotel)
            {
                var checkHotel = await this._context.Hotels.FirstOrDefaultAsync(u => u.ID == h.HotelID);
                List<string?> img = new List<string?>();

                if (checkHotel!=null)
                {
                    img.AddRange(await this._context.Galleries.Where(u => u.HotelID == checkHotel.ID).OrderByDescending(h => h.IsFeatureImage).Select(h => h.ImagePath).ToListAsync());
                    list.Add(new WishList
                    {
                        wishHotel = new List<WishListHotel>
    {
        new WishListHotel { ID =h.HotelID,
                            Descriptions = checkHotel.Description,
                            HotelName = checkHotel.HotelName,
                            img = img,
                            Location =  $"{checkHotel.City},{checkHotel.Country}",
                            NameSeller = user.UserName,
                            
                            

        }
    }
                    });
                }
            }

            return View(list);
        }
    }
}
