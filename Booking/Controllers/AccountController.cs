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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddWish(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var checkhotek = await this._context.Hotels.FindAsync(id);
            if(checkhotek==null)
            {
                return Json(new { success = false, message = "Hotel không tồn tại!!" });
            }else if (await this._context.WishlistHotels.AnyAsync(u => u.UserID == user.Id && u.HotelID == id))
            {
                return Json(new { success = false, message = $"Hotel {checkhotek.HotelName} đã tồn tại trong danh sách yêu thích.!" });
            }
            else
            {
                var tem = new WishlistHotel
                {
                    HotelID = id,
                    ID = Guid.NewGuid(),
                    CreateDate = DateTime.Now,
                    UserID = user.Id
                };
                try
                {
                    var add = await this._context.WishlistHotels.AddAsync(tem);
                    await this._context.SaveChangesAsync();
                    return Json(new { success = true, message = $"Thêm Hotels {checkhotek.HotelName} thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = "Thêm thất bại!" });
                }   
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> RemoveWish(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var checkhotek = await this._context.Hotels.FindAsync(id);
            if (checkhotek == null)
            {
                return Json(new { success = false, message = "Hotel không tồn tại!!" });
            }
            else if (await this._context.WishlistHotels.AnyAsync(u => u.UserID == user.Id && u.HotelID == id))
            {
                var getHotel = await this._context.WishlistHotels.FirstOrDefaultAsync(u => u.UserID == user.Id && u.HotelID == id);

                try
                {
                    var add =  this._context.WishlistHotels.Remove(getHotel);
                    await this._context.SaveChangesAsync();
                    return Json(new { success = true, message = $"Xóa {checkhotek.HotelName} khỏi danh sách yêu thích thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = $"Xóa {checkhotek.HotelName} khỏi danh sách yêu thích thất bại!" });
                }
            }
            else
            {
                return Json(new { success = true, message = $"Hotel {checkhotek.HotelName} đã bị xóa khỏi danh sách yêu thích!" });
            }
        }
        public async Task<IActionResult> Wallet()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var tem = new walletViewModels();
          var getBalace = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.trangthai=="done")
          .OrderByDescending(h => h.thoigian)
          .ToListAsync();

            if (!getBalace.Any())
            {
                tem.tien = 0;
            }
            else
            {
                tem.tien = getBalace.FirstOrDefault().sotiensau;
                tem.numberTrans = getBalace.Count();
                foreach(var item in getBalace)
                {
                    tem.list.Add(new tranList
                    {
                        amount = item.sotientruoc,
                        Balance = item.sotiensau+"",
                        detail= item.noidung,
                        paymentType = item.ordercode!=1?"Wallet":"Online Banking",
                        time = item.thoigian,
                        type= item.method
                    });
                }
            }
            return View(tem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> AddBalance(long number)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { status = "error", msg = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }else if(number < 100000)
            {
                return Json(new { status = "error", msg = "Nạp tối thiểu 100,000 VND" });
            }
            else
            {



                return Json(new { status = "success", msg = $"Tạo đơn nạp tiền thành công.!  {number}" });
            }
        

        }


            public async Task<IActionResult> Review()
        {
            return View();
        }


        public async Task<IActionResult> Payment()
        {
            return View();
        } 
        public async Task<IActionResult> Profile()
        {
            return View();
        }
        public async Task<IActionResult> invoices()
        {
            return View();
        }













        }


    }

