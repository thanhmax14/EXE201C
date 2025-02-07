using Booking.Data;
using Booking.Models;
using Booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using Net.payOS;
using System.Transactions;
using Booking.Services;
using Booking.BaseRepo;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using System.Globalization;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Booking.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PayOS _Payos;
        private readonly ManagerTransastions _transactionManager;

        public AccountController(ApplicationDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, PayOS payos, ManagerTransastions transactionManager)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _Payos = payos;
            _transactionManager = transactionManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var tem = new DashBoardUser();
            var temRecBoking = new List<RecentBookings>();
            var reBoking = new List<(string, string, string, Guid, DateTime?, decimal?, string)>();
            var getList = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.method == "buy")
      .OrderByDescending(h => h.thoigian)
      .ToListAsync();
             tem.numberBoking= getList.Count();
            tem.totalTrans = getList.Sum(u => Math.Abs(u.sotienthaydoi));

            var list = await this._context.Datphongs
      .Where(u => u.UserID == user.Id)
      .OrderByDescending(query => query.BookedOn)
      .Take(3) // Lấy tối đa 3 phần tử
      .ToListAsync();

            foreach(var item in list)
            {
                var getHotsl = await this._context.Rooms.Where(u => u.RoomID == item.RoomID).ToListAsync();
                if (getHotsl.Any())
                {

                    var getInfoHotel = await this._context.Hotels.FindAsync(getHotsl.FirstOrDefault().HotelID);
                    if (getInfoHotel != null)
                    {
                        var (date, time) = SplitDateTime(item.checkIn);
                        var getImg = this._context.Galleries.FirstOrDefault(u => u.HotelID == getInfoHotel.ID && u.IsFeatureImage);


                        var imgPath = "https://dreamstour.dreamstechnologies.com/html/assets/img/tours/tour-large-01.jpg";
                        if (getImg != null)
                        {
                            imgPath = getImg.ImagePath;
                        }

                        temRecBoking.Add(new RecentBookings
                        {
                            ID = getInfoHotel.ID,
                            Name =$"{getInfoHotel.HotelName}"+$"<span class=\"text-gray-5 fw-normal fs-14\">( {getHotsl.FirstOrDefault().RoomName} )</span>",
                            status = item.progress,
                            time = time,
                            Type = "<span class=\"badge badge-soft-info badge-xs rounded-pill mb-1\"><i class=\"isax isax-buildings me-1\"></i>Hotel</span>",
                            date = date,
                            link = $"/home/HotelDetail/{getInfoHotel.ID}",
                            img = imgPath,
                            
                            
                        });
                        reBoking.Add((getInfoHotel.HotelName, $"{imgPath}", $"{item.OrderID}",
                            getInfoHotel.ID, item.DatePayment, item.totalPaid, item.paymentStatus));

                    }
                }
            }
            tem.RecentBookings = temRecBoking;
            tem.RecentInvoices=reBoking;





            return View(tem);
        }
        public static (string Date, string Time) SplitDateTime(DateTime? dateTime)
        {
            string date = dateTime?.ToString("yyyy-MM-dd");
            string time = dateTime?.ToString("HH:mm:ss");  
            return (date, time);
        }

        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var tem = new settingViewModels { 
                 id = user.Id,
                   address= user.address,
                   birthday = user.sinhNhat,
                   District = user.District,
                   email = user.Email,
                   firstName = user.firstName,
                   img = user.img,
                lastName = user.lastName,
                   phone = user.PhoneNumber,
                   Province = user.Province,
                   Ward = user.Ward,
                   zipcode = user.ZipCode
            };

            return View(tem);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(settingViewModels model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Erro404", "Home");
                }

                // Kiểm tra nếu email đã tồn tại trong hệ thống
                /*var existingEmailUser = await _userManager.FindByEmailAsync(model.id);
                if (existingEmailUser != null && existingEmailUser.Id != user.Id)  // Kiểm tra xem có user khác trùng email không
                {
                    ModelState.AddModelError("email", "Email already exists!");
                    return View(model);
                }*/

                // Kiểm tra nếu phone đã tồn tại trong hệ thống
                var existingPhoneUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == model.phone);

                if (existingPhoneUser != null && existingPhoneUser.Id != user.Id)  // Kiểm tra xem có user khác trùng số điện thoại không
                {
                    ModelState.AddModelError("phone", "Phone number already exists!");
                    model.email = user.Email;
                    return View(model);
                }

                user.firstName = model.firstName;
                user.lastName = model.lastName;
                user.PhoneNumber = model.phone;
                user.address = model.address;
                user.sinhNhat = model.birthday;
                user.ZipCode = model.zipcode;
                user.Province = model.Province;
                user.District = model.District;
                user.Ward = model.Ward;
                user.isUpdateProfile = true;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Messse"] = "Your profile update was successful!";
                    return View(model);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code.Contains("Email"))
                        {
                            ModelState.AddModelError("email", error.Description);
                        }
                        else if (error.Code.Contains("PhoneNumber"))
                        {
                            ModelState.AddModelError("phone", error.Description);
                        }
                        else if (error.Code.Contains("FirstName"))
                        {
                            ModelState.AddModelError("firstName", error.Description);
                        }
                        else if (error.Code.Contains("LastName"))
                        {
                            ModelState.AddModelError("lastName", error.Description);
                        }
                        else if (error.Code.Contains("ZipCode"))
                        {
                            ModelState.AddModelError("zipcode", error.Description);
                        }
                        else if (error.Code.Contains("Province"))
                        {
                            ModelState.AddModelError("Province", error.Description);
                        }
                        else if (error.Code.Contains("District"))
                        {
                            ModelState.AddModelError("District", error.Description);
                        }
                        else if (error.Code.Contains("Ward"))
                        {
                            ModelState.AddModelError("Ward", error.Description);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                model.email = user.Email;
            }

          
            return View(model);
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


            var getWishTour = await this._context.WishlistTours
     .Where(h => h.UserID == user.Id)
     .OrderBy(h => h.CreateDate)
     .ToListAsync();


            foreach (var h in getWishTour)
            {
                var checkHotel = await this._context.Tours.FirstOrDefaultAsync(u => u.ID == h.TourID);
                List<string?> img = new List<string?>();

                if (checkHotel != null)
                {
                    img.AddRange(await this._context.GalleryTours.Where(u => u.TourID == checkHotel.ID).OrderByDescending(h => h.IsFeatureImage).Select(h => h.ImagePath).ToListAsync());
                    list.Add(new WishList
                    {
                        ListTours = new List<WishListTour>
    {
        new WishListTour { ID =h.TourID,
                            Descriptions = checkHotel.Description,
                            TourName = checkHotel.TourName,
                            img = img,
                            Location =  $"{checkHotel.City},{checkHotel.Country}",
                            NameSeller = user.UserName,
                            category = checkHotel.Category,
                            NumberReview =0,
                            price =checkHotel.price,
                            rating =5+".0"
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






        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> AddWishTour(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var checkhotek = await this._context.Tours.FindAsync(id);
            if (checkhotek == null)
            {
                return Json(new { success = false, message = "Tours không tồn tại!!" });
            }
            else if (await this._context.WishlistTours.AnyAsync(u => u.UserID == user.Id && u.TourID == id))
            {
                return Json(new { success = false, message = $"Tours {checkhotek.TourName} đã tồn tại trong danh sách yêu thích.!" });
            }
            else
            {
                var tem = new WishlistTour
                {
                    TourID = id,
                    ID = Guid.NewGuid(),
                    CreateDate = DateTime.Now,
                    UserID = user.Id
                };
                try
                {
                    var add = await this._context.WishlistTours.AddAsync(tem);
                    await this._context.SaveChangesAsync();
                    return Json(new { success = true, message = $"Thêm Tours {checkhotek.TourName} thành công!" });
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

        public async Task<IActionResult> RemoveWishTour(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            var checkhotek = await this._context.Tours.FindAsync(id);
            if (checkhotek == null)
            {
                return Json(new { success = false, message = "Tours không tồn tại!!" });
            }
            else if (await this._context.WishlistTours.AnyAsync(u => u.UserID == user.Id && u.TourID == id))
            {
                var getHotel = await this._context.WishlistTours.FirstOrDefaultAsync(u => u.UserID == user.Id && u.TourID == id);

                try
                {
                    var add = this._context.WishlistTours.Remove(getHotel);
                    await this._context.SaveChangesAsync();
                    return Json(new { success = true, message = $"Xóa {checkhotek.TourName} khỏi danh sách yêu thích thành công!" });
                }
                catch
                {
                    return Json(new { success = false, message = $"Xóa {checkhotek.TourName} khỏi danh sách yêu thích thất bại!" });
                }
            }
            else
            {
                return Json(new { success = true, message = $"Tours {checkhotek.TourName} đã bị xóa khỏi danh sách yêu thích!" });
            }
        }












        public async Task<IActionResult> Wallet()
        {
        
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
           

            var getInvoide  = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.method == "nap").OrderByDescending(h => h.thoigian)
              .ToListAsync(); 
            if (getInvoide.Any())
            {
                foreach (var itemDogntien in getInvoide)
                {
                    var getBalace1 = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.trangthai == "done")
           .OrderByDescending(h => h.thoigian)
           .FirstOrDefaultAsync();

                    if (!itemDogntien.IsComplete)
                    {
                        var checkORder = await this._Payos.getPaymentLinkInformation(itemDogntien.ordercode);
                        var status = checkORder.status.ToUpper();
                        var url = $"https://pay.payos.vn/web/{checkORder.id}/";
                        switch (status)
                        {
                            case "CANCELLED":

                                itemDogntien.trangthai = "huy";
                                itemDogntien.IsComplete = true;
                                break;
                            case "PENDING":
                                break;
                            case "EXPIRED":
                                itemDogntien.trangthai = "huy";
                                itemDogntien.IsComplete = true;
                                this._context.Update(itemDogntien);
                                await this._context.SaveChangesAsync();
                                break;
                            case "UNDERPAID":
                                itemDogntien.trangthai = "huy";
                                itemDogntien.IsComplete = true;
                                break;
                            case "PROCESSING":

                                break;
                            case "FAILED":
                                itemDogntien.trangthai = "huy";
                                itemDogntien.IsComplete = true;
                                this._context.Update(itemDogntien);
                                await this._context.SaveChangesAsync();
                                break;
                            case "PAID":
                                itemDogntien.trangthai = "done";
                                itemDogntien.IsComplete = true;
                                // Cập nhật số tiền sau
                                itemDogntien.sotiensau = getBalace1 != null ? itemDogntien.sotienthaydoi + (getBalace1.sotiensau ) : itemDogntien.sotienthaydoi;
                                itemDogntien.thoigian = DateTime.Now;
                                break;
                            default:
                                break;
                        }

                        this._context.Update(itemDogntien);
                    }
                    await this._context.SaveChangesAsync();
                }

            }



            var tem = new walletViewModels();

            var getBalace = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.trangthai == "done")
            .OrderByDescending(h => h.thoigian)
            .ToListAsync();

            var getList = await this._context.Dongtiens.Where(u => u.UserID == user.Id)
      .OrderByDescending(h => h.thoigian)
      .ToListAsync();
            if (!getBalace.Any())
            {
                tem.tien = 0;
            }
            else
            {
                tem.tien = getBalace.FirstOrDefault().sotiensau;
               
            }
            if (getList.Any())
            {
                tem.numberTrans = getList.Count();
                foreach (var item in getList)
                {
                    tem.list.Add(new tranList
                    {
                        amount = item.sotienthaydoi,
                        Balance = item.sotiensau + "",
                        detail = item.noidung,
                        paymentType = item.ordercode == 1 ? "Wallet" : "Online Banking",
                        time = item.thoigian,
                        type = item.method,
                        trangthai = item.trangthai
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
                var tien = 0m;
                var getBalace = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.trangthai == "done")
         .OrderByDescending(h => h.thoigian)
         .ToListAsync();

                if (getBalace.Any())
                {
                    tien = getBalace.FirstOrDefault().sotiensau;
                }
                    try
                {

                    int orderCode = RandomCode.GenerateOrderCode();
                    var check = await this._context.Dongtiens.FirstOrDefaultAsync(u => u.ordercode == orderCode);
                    while (check != null)
                    {
                        orderCode = RandomCode.GenerateOrderCode();
                        check = await this._context.Dongtiens.FirstOrDefaultAsync(u => u.ordercode == orderCode);
                    }
                    long expirationTimestamp = DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds();

                    ItemData item = new ItemData($"Thực hiện nạp tiền vào tài khoản {user.UserName}:", 1, int.Parse(number+""));
                    List<ItemData> items = new List<ItemData> { item };
                    var request = _httpContextAccessor.HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    PaymentData paymentData = new PaymentData(orderCode, int.Parse(number + ""), "", items, $"{baseUrl}/Account/depositBiling?user={user.UserName}&address={user.address}&email={user.Email}&phone={user.PhoneNumber}&create={DateTime.Now}&amount={number}",
                        $"{baseUrl}/Account/depositBiling?user={user.UserName}&address={user.address}&email={user.Email}&phone={user.PhoneNumber}&create={DateTime.Now}&amount={number}"
                    , null, null, null, null, null, expirationTimestamp
                       );
                    CreatePaymentResult createPayment = await _Payos.createPaymentLink(paymentData);
                    var url = $"https://pay.payos.vn/web/{createPayment.paymentLinkId}/";
                    await _transactionManager.ExecuteInTransactionAsync(async () =>
                    {
                        
                        var temDongTien = new dongtien
                        {
                            sotientruoc = tien,
                            sotienthaydoi = number,
                            sotiensau =0m,
                            noidung = $"{url}",
                            trangthai ="Doi",
                            method ="Nap",
                            ordercode = orderCode,
                            thoigian = DateTime.Now,
                            UserID = user.Id
                        };
                       
                        await this._context.Dongtiens.AddAsync(temDongTien);
                      
                    });
                    await this._context.SaveChangesAsync();

                    return Json(new { status = "success", msg = $"Tạo đơn nạp tiền thành công.!", url = $"{url}" }); ;
                }
                catch (System.Exception exception)
                {
                    Console.WriteLine(exception);
                    return Json(new { status = "error", msg = "Đã xảy ra lỗi vui lòng thử lại hoặc nhắn tin với Admin" });
                }
            }
        

        }


        public async Task<IActionResult> BookingHotels(BilingHotelsViewModels models)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || models==null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var billingData = HttpContext.Session.GetString("BillingInfo");

            if (string.IsNullOrEmpty(billingData))
            {
                return RedirectToAction("Erro404", "Home");
            }

            var model = JsonConvert.DeserializeObject<BilingHotelsViewModels>(billingData);
            return View(model);
            
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking([FromBody] BookingRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, msg = "Bạn phải đăng nhập để thực hiện hành động này!" });
            }
            if (!user.isUpdateProfile)
            {
                return Json(new { notUpdate = true, msg = "Bạn phải cập nhật đầy đủ thông tin cá nhân!." });
            }
            if (request == null || request.rooms == null || !request.rooms.Any())
            {
                return Json(new { status = "error", msg = "Dữ liệu đặt phòng không hợp lệ." });
            }
            if (string.IsNullOrEmpty(request.CheckIn) || string.IsNullOrEmpty(request.CheckOut))
            {
                return Json(new { status = "error", msg = "Ngày nhận phòng và trả phòng không được để trống." });
            }
            if (!DateTime.TryParseExact(request.CheckIn, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime checkInDate) ||
                !DateTime.TryParseExact(request.CheckOut, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime checkOutDate))
            {
                return Json(new { status = "error", msg = "Định dạng ngày không hợp lệ. Vui lòng nhập theo định dạng dd-MM-yyyy." });
            }
            if (checkInDate < DateTime.Today)
            {
                return Json(new { status = "error", msg = "Ngày nhận phòng không thể là ngày trong quá khứ." });
            }
            if (checkOutDate <= checkInDate)
            {
                return Json(new { status = "error", msg = "Ngày trả phòng phải sau ngày nhận phòng." });
            }
            
            if (request.Guests == null)
            {
                return Json(new { status = "error", msg = "Bạn chưa chọn số lượng khách." });
            }
            var rooms = await _context.Rooms
                .Where(u => request.rooms.Contains(u.RoomID))
                .OrderByDescending(x => x.PricePerNight)
                .ToListAsync();

            if (!rooms.Any())
            {
                return Json(new { status = "error", msg = "Không tìm thấy phòng phù hợp." });
            }
            var checkExit = await this._context.Hotels.FindAsync(rooms.FirstOrDefault().HotelID);
              if(checkExit != null && checkExit.UserID == user.Id)
            {
                return Json(new { status = "error", msg = "Bạn không thể đặt khách sạn của chính bạn." });
            }
            var galleryImages = await _context.GalleryRooms
                .Where(u => request.rooms.Contains(u.RoomID) && u.IsFeatureImage)
                .ToDictionaryAsync(x => x.RoomID, x => x.ImagePath);


            var checkOut = DateTime.ParseExact(request.CheckOut, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(12);
            var checkIn = DateTime.ParseExact(request.CheckIn, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(12);

            var tongtien = 0m;
            foreach (var gettien in rooms)
            {
                tongtien += CalculateStayCost(checkIn, checkOut) * gettien.PricePerNight;
            }

                   

            var listOrder = rooms.Select(item => new OrderHotelDetail
            {
                RomID = item.RoomID,
                HotelID = item.HotelID,
                checkOut = DateTime.ParseExact(request.CheckOut, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(12),
                checkIn = DateTime.ParseExact(request.CheckIn, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(12),
                Infants = request.Guests?.Infants ?? 0,
                Adults = request.Guests?.Adults ?? 1,
                Children = request.Guests?.Children ?? 0,
                Tax = 0,
                Discount = 0,
                img = galleryImages.ContainsKey(item.RoomID) ? galleryImages[item.RoomID] : string.Empty,
                NameRoom = item.RoomName,
                BookingFees = 0,
                total = tongtien
            }).ToList();

            var billingInfo = new BilingHotelsViewModels
            {
                id = user.Id,
                address = user.address,
                birthday = user.sinhNhat,
                District = user.District,
                email = user.Email,
                firstName = user.firstName,
                img = user.img,
                lastName = user.lastName,
                phone = user.PhoneNumber,
                Province = user.Province,
                Ward = user.Ward,
                zipcode = user.ZipCode,
                list = listOrder
            };
            HttpContext.Session.SetString("BillingInfo", JsonConvert.SerializeObject(billingInfo));

            return Json(new { status = "success", url = Url.Action("BookingHotels", "Account") });
        }








        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComfirmTour([FromBody] TourRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { notAuth = true, msg = "Bạn phải đăng nhập để thực hiện hành động này!" });
            }
            if (!user.isUpdateProfile)
            {
                return Json(new { notUpdate = true, msg = "Bạn phải cập nhật đầy đủ thông tin cá nhân!." });
            }
            if (request == null || request.tourID == null)
            {
                return Json(new { status = "error", msg = "Dữ liệu đặt phòng không hợp lệ." });
            }
           
            if (request.Guests == null)
            {
                return Json(new { status = "error", msg = "Bạn chưa chọn số lượng khách." });
            }
            var rooms = await _context.Tours
                .Where(u => request.tourID == u.ID)
                .OrderByDescending(x => x.price)
                .ToListAsync();

            if (!rooms.Any())
            {
                return Json(new { status = "error", msg = "Không tìm thấy Tour!." });
            }
            var soluong = request.Guests.Adults + request.Guests.Infants + request.Guests.Children;

            if(soluong> rooms.FirstOrDefault().totalPreoPle)
            {
                return Json(new { status = "error", msg = "Số lượng khách bạn chọn quá mức!" });
            }

            var checkExit = await this._context.Tours.FindAsync(rooms.FirstOrDefault().ID);
            if (checkExit != null && checkExit.UserID == user.Id)
            {
                return Json(new { status = "error", msg = "Bạn không thể đặt khách sạn của chính bạn." });
            }
            try
            {
                var galleryImages = await _context.GalleryTours
               .Where(u => u.TourID == request.tourID && u.IsFeatureImage)
               .FirstOrDefaultAsync();

                var imgPath = "https://dreamstour.dreamstechnologies.com/html/assets/img/tours/tour-large-01.jpg";
                if (galleryImages != null)
                {
                    imgPath = galleryImages.ImagePath;
                }

                var stardate = DateTime.ParseExact(checkExit.startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(5);
                var endDate = DateTime.ParseExact(checkExit.EndDATE, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(22);

                var listTour = new OrderTourDetail
                {
                    TourID = request.tourID,
                    Departure = stardate + "",
                    Return = endDate + "",
                    Infants = request.Guests?.Infants ?? 0,
                    Adults = request.Guests?.Adults ?? 1,
                    Children = request.Guests?.Children ?? 0,
                    Tax = 0,
                    Discount = 0,
                    img = imgPath,
                    NameTour = checkExit.TourName,
                    BookingFees = 0,
                    total = checkExit.price,
                    NoOfdate = CalculateStayDuration(stardate, endDate)
                };

                var billingInfo = new BilingTourViewModels
                {
                    id = user.Id,
                    address = user.address,
                    birthday = user.sinhNhat,
                    District = user.District,
                    email = user.Email,
                    firstName = user.firstName,
                    img = user.img,
                    lastName = user.lastName,
                    phone = user.PhoneNumber,
                    Province = user.Province,
                    Ward = user.Ward,
                    zipcode = user.ZipCode,
                    list = listTour
                };
                HttpContext.Session.SetString("BillingTourInfo", JsonConvert.SerializeObject(billingInfo));
                return Json(new { status = "success", url = Url.Action("Bookingtour", "Account") });
            }
            catch(Exception e)
            {
                return Json(new { status = "error", msg = $"{e.Message}" });
            }
        }


        public async Task<IActionResult> BookingTour()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var billingData = HttpContext.Session.GetString("BillingTourInfo");

            if (string.IsNullOrEmpty(billingData))
            {
                return RedirectToAction("Erro404", "Home");
            }

            var model = JsonConvert.DeserializeObject<BilingTourViewModels>(billingData);
            var a = model;
            return View(model);
        }





            public async Task<IActionResult> Review()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }

            var tem = new List<readcmt>();

            var getInfo = await this._context.ReviewHotels.Where(u => u.UserID == user.Id).OrderByDescending(q =>q.datecmt).ToListAsync();

            if (getInfo.Any())
            {
                foreach(var item in getInfo)
                {
                    var getInfoHotel = await this._context.Hotels.FindAsync(item.HotelID);
                    tem.Add(new readcmt
                    {
                        UserName = $"{user.firstName} {user.lastName}",
                        cmt = item.cmt,
                        imgUser = user.img,
                        datecmt = item.datecmt,
                        relay = getInfoHotel.HotelName,
                        rating = item.rating + ".0",
                        imgSeller = item.HotelID.ToString()
                    });
                }  
            }

            var getInfoTour = await this._context.ReviewTours.Where(u => u.UserID == user.Id).OrderByDescending(q => q.datecmt).ToListAsync();

            if (getInfoTour.Any())
            {
                foreach (var item in getInfoTour)
                {
                    var getInfoHotel = await this._context.Tours.FindAsync(item.TourID);
                    tem.Add(new readcmt
                    {
                        UserName = $"{user.firstName} {user.lastName}",
                        cmt = item.cmt,
                        imgUser = user.img,
                        datecmt = item.datecmt,
                        relay = getInfoHotel.TourName,
                        rating = item.rating + ".0",
                        imgSeller = item.TourID.ToString()
                    });
                }
            }



            return View(tem);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComfirmPay(string optional)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn phải đăng nhập để thực hiện hành động này!" });
            }
            var money = 0m;
            try
            {
                var getBalace = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.trangthai == "done")
                 .OrderByDescending(h => h.thoigian)
                 .FirstOrDefaultAsync();
                if (getBalace != null)
                {
                    money = getBalace.sotiensau;
                }
                var romName = "Room";
                var order = "123456";
                var billingData = HttpContext.Session.GetString("BillingInfo");

                var model = JsonConvert.DeserializeObject<BilingHotelsViewModels>(billingData);

                var tongtien = 0m;
                foreach (var gettien in model.list)
                {
                    tongtien+= CalculateStayCost(gettien.checkIn, gettien.checkOut) * gettien.total;
                }
                if (money < tongtien)
                {
                    return Json(new { notFunds = true, message = "Bạn không đủ tiền thanh toán vui lòng nạp!." });
                }

                if (string.IsNullOrEmpty(billingData) || tongtien ==0m)
                {
                    return Json(new { success = false, message = "Lỗi khi thanh toán: " });
                }
                foreach (var itemModel in model.list)
                {
                    romName = itemModel.NameRoom;
                    string guests =
    (itemModel.Adults > 0 ? $"Adults {itemModel.Adults}" : "") +
    (itemModel.Adults > 0 && itemModel.Children > 0 ? ", " : "") +
    (itemModel.Children > 0 ? $"Children {itemModel.Children}" : "");

                    var orderID = RandomCode.GenerateUniqueCode();
                    order = orderID;
                    var temp = new datphong
                    {
                        OrderID = orderID,
                        ID = Guid.NewGuid(),
                        UserID = user.Id,
                        BookedOn = DateTime.Now,
                        BookingFees = 0,
                        checkIn = itemModel.checkIn,
                        checkOut = itemModel.checkOut,
                        DatePayment = DateTime.Now,
                        Discount =0 ,
                        Guests = string.IsNullOrEmpty(guests) ? "No guests" : guests,
                        messess = optional,
                        RoomID =  itemModel.RomID,
                        NoOfDate = CalculateStayDuration(itemModel.checkIn,itemModel.checkOut),
                        paymentStatus = "PAID",
                        tax =0,
                        totalPaid = tongtien,
                        progress = "Upcoming"

                    };

                    var updateDongtien = this._context.Dongtiens.AddAsync(new dongtien
                    {
                        IsComplete = true,
                        method="buy",
                        UserID = user.Id,
                        noidung = $"/account/invoices/{orderID}",
                        sotientruoc = getBalace.sotiensau,
                        sotienthaydoi = -tongtien,
                        sotiensau = getBalace.sotiensau - tongtien,
                        thoigian = DateTime.Now,
                        trangthai = "done",
                       
                    });
                    this._context.Datphongs.Add(temp);
                    this._context.SaveChanges();
                }
              
                return Json(new { success = true, message = "Thanh toán thành công!", roomName = $"{romName}", referenceNumber = $"{order}" }); ;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi thanh toán: " + ex.Message });
            }
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComfirmPayTour(string optional)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Bạn phải đăng nhập để thực hiện hành động này!" });
            }
            var money = 0m;
            try
            {
                var getBalace = await this._context.Dongtiens.Where(u => u.UserID == user.Id && u.trangthai == "done")
                 .OrderByDescending(h => h.thoigian)
                 .FirstOrDefaultAsync();
                if (getBalace != null)
                {
                    money = getBalace.sotiensau;
                }
                var romName = "Tour";
                var order = "123456";
                var billingData = HttpContext.Session.GetString("BillingTourInfo");

                var model = JsonConvert.DeserializeObject<BilingTourViewModels>(billingData);

                var tongtien = model.list.total;
               
               

                if (string.IsNullOrEmpty(billingData) || tongtien == 0m)
                {
                    return Json(new { success = false, message = "Lỗi khi thanh toán: " });
                }

                var checkTour = await this._context.Tours.FindAsync(model.list.TourID);
                if (checkTour == null)
                {
                    return Json(new { success = false, message = "Lỗi khi thanh toán: " });
                }
                if (money < checkTour.price)
                {
                    return Json(new { Notfunds = true, message = "Bạn không đủ tiền thanh toán vui lòng nạp!." });
                }
                var stardate = DateTime.ParseExact(checkTour.startDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(5);
                var endDate = DateTime.ParseExact(checkTour.EndDATE, "dd-MM-yyyy", CultureInfo.InvariantCulture).AddHours(22);



                romName = model.list.NameTour;
                    string guests =
    (model.list.Adults > 0 ? $"Adults {model.list.Adults}" : "") +
    (model.list.Adults > 0 && model.list.Children > 0 ? ", " : "") +
    (model.list.Children > 0 ? $"Children {model.list.Children}" : "");

                    var orderID = RandomCode.GenerateUniqueCode();
                    order = orderID;
                    var temp = new DaTour
                    {
                        OrderID = orderID,
                        ID = Guid.NewGuid(),
                        UserID = user.Id,
                        BookedOn = DateTime.Now,
                        BookingFees = 0,                      
                        DatePayment = DateTime.Now,
                        Discount = 0,
                        Guests = string.IsNullOrEmpty(guests) ? "No guests" : guests,
                        messess = optional,
                        TourID = model.list.TourID,
                        NoOfDate = CalculateStayDuration(stardate, endDate),
                        paymentStatus = "PAID",
                        tax = 0,
                        totalPaid = tongtien,
                        progress = "Upcoming",

                    };

                    var updateDongtien = this._context.Dongtiens.AddAsync(new dongtien
                    {
                        IsComplete = true,
                        method = "buy",
                        UserID = user.Id,
                        noidung = $"/account/invoices/{orderID}",
                        sotientruoc = getBalace.sotiensau,
                        sotienthaydoi = -tongtien,
                        sotiensau = getBalace.sotiensau - tongtien,
                        thoigian = DateTime.Now,
                        trangthai = "done",

                    });
                  await   this._context.DaTours.AddAsync(temp);
                  await  this._context.SaveChangesAsync();
                

                return Json(new { success = true, message = "Thanh toán thành công!", roomName = $"{romName}", referenceNumber = $"{order}" }); ;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi thanh toán: " + ex.Message });
            }
        }
        public static string CalculateStayDuration(DateTime checkIn, DateTime checkOut)
        {
            var stayDuration = checkOut.Date - checkIn.Date;
            int noOfDays = stayDuration.Days + 1;
            int noOfNights = stayDuration.Days;
            return $"{noOfDays} Days, {noOfNights} Nights";
        }
        public static int CalculateStayCost(DateTime checkIn, DateTime checkOut)
        {
            var stayDuration = checkOut.Date - checkIn.Date;
            int noOfDays = stayDuration.Days + 1;
            int noOfNights = stayDuration.Days;

            return noOfDays;
        }


        public async Task<IActionResult> Payment()
        {
            return View();
        } 
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var tem = new settingViewModels
            {
                id = user.Id,
                address = user.address,
                birthday = user.sinhNhat,
                District = user.District,
                email = user.Email,
                firstName = user.firstName,
                img = user.img,
                lastName = user.lastName,
                phone = user.PhoneNumber,
                Province = user.Province,
                Ward = user.Ward,
                zipcode = user.ZipCode
            };

            return View(tem);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> invoices(string id)
        {
           /* var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }*/

            var tem = new invoicesViewModels();
            var getHotel = await this._context.Datphongs.Where(u => u.OrderID == id).ToListAsync();

            if (getHotel.Any())
            {
                var user = await this._userManager.FindByIdAsync(getHotel.FirstOrDefault().UserID);
                if (user == null)
                {
                    return RedirectToAction("Erro404", "Home");
                }
                


                    var totel = getHotel.Sum(u => u.totalPaid);
                foreach (var item in getHotel)
                {
                    var getHotsl = await this._context.Rooms.Where(u => u.RoomID == item.RoomID).ToListAsync();
                    if (getHotsl.Any())
                    {

                        var getInfoHotel = await this._context.Hotels.FindAsync(getHotsl.FirstOrDefault().HotelID);
                        if (getInfoHotel != null)
                        {
                            tem.vat = 0;
                            tem.UserName = $"{user.firstName} {user.lastName}";
                            tem.address = user.address;
                            tem.phone = user.PhoneNumber;
                            tem.email = user.Email;
                            tem.duedate = item.DatePayment;
                            tem.creteDate = item.BookedOn;
                            tem.discount = 0;
                            tem.Total = totel;
                            tem.list.Add((getInfoHotel.HotelName, item.totalPaid, 0m, item.totalPaid));
                            tem.orderID = id;
                            tem.status = item.progress;

                        }
                    }

                }


                return View(tem);

            }
            else
            {
                return RedirectToAction("Erro404", "Home");
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Sendcmt(string rating, string comment,Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { status = "error", msg = "Bạn phải đăng nhập để thực hiện hành động này!" });
            }
            var a = id;
            var infoHotel = await this._context.Hotels.FirstOrDefaultAsync(u => u.ID == id);
            if(infoHotel==null)
            {
                return Json(new { status = "error", msg = "Khách sạn không tồn tại!" });
            }
            if (string.IsNullOrEmpty(rating) || string.IsNullOrEmpty(comment))
            {
                return Json(new { status = "error", msg = "Vui lòng nhập đầy đủ thông tin!" });
            }
            var getList = await this._context.Datphongs.Where(u => u.UserID == user.Id && u.paymentStatus == "PAID" && !u.isComment)
                    .ToListAsync();
            if (getList.Any())  {     
                try
                {
                    await this._context.ReviewHotels.AddAsync(new ReviewHotels
                    {
                         cmt = comment,
                         datecmt = DateTime.Now,
                         UserID = user.Id,
                         HotelID= infoHotel.ID, 
                         OrderID= ""
                    });

                    var tem = getList.FirstOrDefault();
                    tem.isComment = true;
                    this._context.Datphongs.Update(tem);
                    await this._context.SaveChangesAsync();
                }
                catch
                {
                    return Json(new { status = "error", msg = "Đã xảy ra lỗi không mong muốn!" });
                }
                return Json(new { status = "success", msg = "Cảm ơn bạn đã đánh giá!" });
            }
            else
            {
                return Json(new { status = "error", msg = "Bạn phải sử dụng dịch vụ mới được đánh giá!" });
            }
          
        }

        public async Task<IActionResult> DepositBiling()
        {
            return View();
        }


        public async Task<IActionResult> TourBooking()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var list = await this._context.DaTours.Where(u => u.UserID == user.Id).OrderByDescending(query => query.BookedOn).ToListAsync();
            var tem = new List<TourBookingViewodels>();
            if (list.Any())
            {
                foreach(var item in list)
                {
                   

                        var getInfoTour = await this._context.Tours.FindAsync(item.TourID);
                        if (getInfoTour != null)
                        {
                            var getImg = this._context.GalleryTours.FirstOrDefault(u => u.TourID == getInfoTour.ID && u.IsFeatureImage);

                        var imgPath = "https://dreamstour.dreamstechnologies.com/html/assets/img/tours/tour-large-01.jpg";
                        if (getImg != null)
                        {
                            imgPath = getImg.ImagePath;
                        }
                        tem.Add(new TourBookingViewodels
                            {
                                date = item.NoOfDate,
                                TourID = item.TourID,
                                Guest = item.Guests,
                                TourName = getInfoTour.TourName,
                                img = imgPath,
                                OrderID = item.OrderID,
                                price = item.totalPaid,
                                status = item.progress,
                                Category =getInfoTour.Category,
                                Booked = item.BookedOn
                                
                            });
                        }
                    
                }
            }

            return View(tem);
        }


        public async Task<IActionResult> HotelsBooking()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var list = await this._context.Datphongs.Where(u => u.UserID == user.Id).OrderByDescending(query => query.BookedOn).ToListAsync();
            var tem = new List<HotelBookingViewodels>();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    var getHotsl = await this._context.Rooms.Where(u => u.RoomID == item.RoomID).ToListAsync();
                    if (getHotsl.Any())
                    {

                        var getInfoHotel = await this._context.Hotels.FindAsync(getHotsl.FirstOrDefault().HotelID);
                        if (getInfoHotel != null)
                        {
                            var getImg = this._context.Galleries.FirstOrDefault(u => u.HotelID == getInfoHotel.ID);
                            
                            var imgPath = "https://dreamstour.dreamstechnologies.com/html/assets/img/tours/tour-large-01.jpg";
                            if (getImg != null)
                            {
                                imgPath = getImg.ImagePath;
                            }


                            tem.Add(new HotelBookingViewodels
                            {
                                Booked = item.BookedOn,
                                date = item.NoOfDate,
                                hotelID = getHotsl.FirstOrDefault().HotelID,
                                Guest = item.Guests,
                                HotelName = getInfoHotel.HotelName,
                                img = imgPath,
                                location = $"{getInfoHotel.City},{getInfoHotel.Country}",
                                OrderID = item.OrderID,
                                price = item.totalPaid,
                                RomeName = getHotsl.FirstOrDefault().RoomName,
                                status = item.paymentStatus,
                                
                            });
                        }
                    }
                }
            }

            return View(tem);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SendcmtTour(string rating, string comment, Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { status = "error", msg = "Bạn phải đăng nhập để thực hiện hành động này!" });
            }
            var a = id;
            var infoHotel = await this._context.Tours.FirstOrDefaultAsync(u => u.ID == id);
            if (infoHotel == null)
            {
                return Json(new { status = "error", msg = "Tours sạn không tồn tại!" });
            }
            if (string.IsNullOrEmpty(rating) || string.IsNullOrEmpty(comment))
            {
                return Json(new { status = "error", msg = "Vui lòng nhập đầy đủ thông tin!" });
            }
            var getList = await this._context.DaTours.Where(u => u.UserID == user.Id && u.paymentStatus == "PAID" && !u.isComment)
                    .ToListAsync();
            if (getList.Any())
            {
                try
                {
                    await this._context.ReviewTours.AddAsync(new ReviewTour
                    {
                        cmt = comment,
                        datecmt = DateTime.Now,
                        UserID = user.Id,
                        TourID = infoHotel.ID,
                        
                    });

                    var tem = getList.FirstOrDefault();
                    tem.isComment = true;
                    this._context.DaTours.Update(tem);
                    await this._context.SaveChangesAsync();
                }
                catch
                {
                    return Json(new { status = "error", msg = "Đã xảy ra lỗi không mong muốn!" });
                }
                return Json(new { status = "success", msg = "Cảm ơn bạn đã đánh giá!" });
            }
            else
            {
                return Json(new { status = "error", msg = "Bạn phải sử dụng dịch vụ mới được đánh giá!" });
            }

        }





    }


}

