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
                        temRecBoking.Add(new RecentBookings
                        {
                            ID = getInfoHotel.ID,
                            Name =$"{getInfoHotel.HotelName}"+$"<span class=\"text-gray-5 fw-normal fs-14\">( {getHotsl.FirstOrDefault().RoomName} )</span>",
                            status = item.progress,
                            time = time,
                            Type = "<span class=\"badge badge-soft-info badge-xs rounded-pill mb-1\"><i class=\"isax isax-buildings me-1\"></i>Hotel</span>",
                            date = date,
                            link = $"/home/HotelDetail/{getInfoHotel.ID}",
                            img = getImg.ImagePath
                            
                        });
                    }
                }
            }
            tem.RecentBookings = temRecBoking;






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


        public async Task<IActionResult> Review()
        {
            return View();
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
                    return Json(new { Notfunds = true, message = "Bạn không đủ tiền thanh toán vui lòng nạp!." });
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
                        noidung = $"/account/bookingBill/{orderID}",
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

        [HttpGet]
        public async Task<IActionResult> invoices(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            return View();
        }

        public async Task<IActionResult> DepositBiling()
        {
            return View();
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
                foreach(var item in list)
                {
                    var getHotsl = await this._context.Rooms.Where(u => u.RoomID == item.RoomID).ToListAsync();
                    if (getHotsl.Any())
                    {

                        var getInfoHotel = await this._context.Hotels.FindAsync(getHotsl.FirstOrDefault().HotelID);
                        if (getInfoHotel != null)
                        {
                            var getImg = this._context.Galleries.FirstOrDefault(u => u.HotelID == getInfoHotel.ID && u.IsFeatureImage);
                            tem.Add(new HotelBookingViewodels
                            {
                                Booked = item.BookedOn,
                                date = item.NoOfDate,
                                hotelID = getHotsl.FirstOrDefault().HotelID,
                                Guest = item.Guests,
                                HotelName = getInfoHotel.HotelName,
                                img = getImg.ImagePath,
                                location = $"{getInfoHotel.City},{getInfoHotel.Country}",
                                OrderID = item.OrderID,
                                price = item.totalPaid,
                                RomeName = getHotsl.FirstOrDefault().RoomName,
                                status = item.paymentStatus
                            });
                        }
                    }
                }
            }

            return View(tem);
        }







    }


}

