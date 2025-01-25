using Booking.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Booking.ViewModels;
using Booking.Services;
using Booking.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Controllers
{
    public class HomeController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public HomeController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender = null, ApplicationDbContext context = null)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _context = context;
        }

        //  private readonly IEmailSender _emailSender;

        public async Task<IActionResult> Index()
        {
        var list = new List<ListProduct>();
        var infoHotel = new List<ListHotels>();
        var getHotel = await this._context.Hotels
        .Where(h => h.HotelName!="") 
        .OrderBy(h => h.Established) 
        .ToListAsync();        
           foreach(var item in getHotel)
            {
                var user = await this._userManager.FindByIdAsync(item.UserID);
                var temImg = new List<GalleriesImg>();
                var getImg = await this._context.Galleries.Where(u =>u.HotelID == item.HotelID)
             .OrderByDescending(h => h.IsFeatureImage)
             .ToListAsync();
                if (getImg.Any())
                {
                    foreach (var itemImg in getImg)
                {
                    if (!string.IsNullOrWhiteSpace(itemImg.ImagePath))
                    {
                        temImg.Add(new GalleriesImg { img= itemImg.ImagePath });
                    }
                }
                }
                infoHotel.Add(new ListHotels
                {
                    HotelID = item.HotelID,
                    HotelName = item.HotelName,
                    img = temImg,
                    Location = $"{item.City},{item.Country}",
                    NameSeller =user.UserName,
                    NumberReview =200,
                    price = 502
                    
                }); 
            }
            list.Add(new ListProduct
            {
                Hotels = infoHotel
            });
            ViewBag.List = list;
            return View();
        }
        public async Task<IActionResult> HotelDetail(Guid id)
        {
            var infoHotel = await this._context.Hotels.FirstOrDefaultAsync(u => u.HotelID == id);
            var tem = new HotelsDetail();
            if (infoHotel == null)
            {
                return RedirectToAction("Erro404");
            }
            else
            {
               
                tem.HotelName = infoHotel.HotelName;
                tem.HotelTye = infoHotel.Category;
                tem.Locations = $"{infoHotel.City},{infoHotel.Country}";
                tem.Descriptions = infoHotel.Description;
                var Highlights = await this._context.Highlights.Where(u => u.HotelID == infoHotel.HotelID).Select(h => h.HighlightText).ToListAsync();
                tem.Highlights.AddRange(Highlights);
                var Amenities = await this._context.Amenities.Where(u => u.HotelID == infoHotel.HotelID).Select(h => h.AmenityName).ToListAsync();
                tem.Amenities.AddRange(Amenities);
                var romType = await this._context.RoomTypes.Where(u => u.HotelID == infoHotel.HotelID).Select(h => h.RoomTypeName).ToListAsync();
                tem.Roomtypes.AddRange(romType); 
                var services = await this._context.Services.Where(u => u.HotelID == infoHotel.HotelID).Select(h => h.ServiceName).ToListAsync();
                tem.Services.AddRange(services);

                var faq1 = await this._context.FAQs.Where(u => u.HotelID == infoHotel.HotelID).ToListAsync();
                tem.faq = faq1.ToDictionary(faq => faq.Question, faq => faq.Answer);

            }

            return View(tem);
        }
        public IActionResult Erro404()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Login(string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password,bool? rememberMe, string ReturnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return Json(new { status = "error", msg = "Tên Người Dùng Không Được Để Trống" });
            }
            if (string.IsNullOrEmpty(password))
            {
                return Json(new { status = "error", msg = "Mật Khẩu Không Được Để Trống" });
            }
            // Xử lý ReturnUrl tương tự GET
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                return Json(new { status = "error", msg = "Tài khoản không tồn tại" });
            }
           /* if (user.IsLockedByAdmin)
            {
                return Json(new { status = "error", msg = "Tài khoản của bạn đã bị khóa bởi quản trị viên." });
            }*/
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Json(new { status = "comfirm", msg = "Bạn phải xác thực email trước khi đăng nhập." });
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                await _userManager.AccessFailedAsync(user);
                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    return Json(new { status = "error", msg = "Tài khoản của bạn đã bị khóa do quá nhiều lần đăng nhập thất bại." });
                }
                else
                {
                    return Json(new { status = "error", msg = $"Sai mật khẩu! Bạn còn {5 - failedAttempts} lần thử." });
                }
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: true);
            if (result.IsLockedOut)
            {
                return Json(new { status = "error", msg = "Tài khoản của bạn đã bị khóa." });
            }
            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (isTwoFactorEnabled)
            {
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Authenticator");
                return Json(new { status = "verify", msg = "Nhập mã xác minh từ ứng dụng xác thực.", token = token });
            }
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: rememberMe ?? false); // Thêm dòng này
                return Json(new
                    {
                        status = "success",
                        msg = "Đăng nhập thành công",
                        redirectUrl = ViewData["ReturnUrl"]?.ToString()
                    });
     
            }
            return Json(new { status = "error", msg = "Thông tin đăng nhập không chính xác!" });
        }

        [HttpPost]
        public async Task<IActionResult> ResendConfirmationEmail(string username)
        {
            var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                return Json(new { status = "error", msg = "Tài khoản không tồn tại" });
            }

            // Generate a new email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create the confirmation link
            var confirmationLink = Url.Action("ConfirmEmail", "Home", new { userId = user.Id, token = token }, Request.Scheme);

            // Send the confirmation email
            await _emailSender.SendEmailAsync(user.Email, "Email Verification",
                $"Please click the following link to verify your email: {confirmationLink}");

            return Ok(new { message = "A new verification email has been sent." });
        }

        [AllowAnonymous]
        public IActionResult LoginWithProdider(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Home");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            // Xử lý chọn provider (Google hoặc Microsoft)
            if (provider == "Google")
            {
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            else if (provider == "Microsoft")
            {
                return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
            }

            // Nếu không xác định provider, trả về trang đăng nhập mặc định
            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Kiểm tra nếu user đã tồn tại trong hệ thống
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email
                };

                var createUserResult = await _userManager.CreateAsync(user);

                if (!createUserResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Error creating user.");
                    return RedirectToAction("Login");
                }

                if (!await _roleManager.RoleExistsAsync("user"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("user"));
                }

                await _userManager.AddToRoleAsync(user, "user");
            }

            await _signInManager.SignInAsync(user, isPersistent: true);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register(string ReturnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.Identity.Name;
                return RedirectToAction("Index", "Home");
            }
            // Xử lý ReturnUrl tương tự GET
            if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl) && ReturnUrl != Url.Action("Login", "Home")
                && ReturnUrl != Url.Action("Register", "Home") && ReturnUrl != Url.Action("Forgot", "Home")
                && ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (ModelState.ContainsKey("Username") && ModelState["Username"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Username"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("Email") && ModelState["Email"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Email"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("Password") && ModelState["Password"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["Password"].Errors[0].ErrorMessage });
                }

                if (ModelState.ContainsKey("repassword") && ModelState["repassword"].Errors.Any())
                {
                    return Json(new { status = "error", msg = "" + ModelState["repassword"].Errors[0].ErrorMessage });
                }

                return Json(new { status = "error", msg = "Dữ liệu không hợp lệ" });
            }

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl != Url.Action("Login", "Home")
                && model.ReturnUrl != Url.Action("Register", "Home") && model.ReturnUrl != Url.Action("Forgot", "Home")
                && model.ReturnUrl != Url.Action("ResetPassword", "Home")
                )
            {
                ViewData["ReturnUrl"] = model.ReturnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
            {
                return Json(new { status = "error", msg = "Tên người dùng đã tồn tại. Vui lòng chọn tên khác." });
            }
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                return Json(new { status = "error", msg = "Email đã được đăng ký. Vui lòng sử dụng email khác." });
            }
            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                 
                var confirmationLink = Url.Action("ConfirmEmail", "Home",
                    new { userId = user.Id, token = token }, Request.Scheme);

                // Gửi email xác minh
                await _emailSender.SendEmailAsync(user.Email, "Xác nhận email",
                    $"Vui lòng nhấp vào liên kết để xác nhận email: {confirmationLink}");
                await _signInManager.SignInAsync(user, isPersistent: true);


                return Json(new { status = "success", msg = "Đăng ký thành công.", redirectUrl = ViewData["ReturnUrl"].ToString() });

            }
            var firstResultError = result.Errors.FirstOrDefault()?.Description;
            return Json(new { status = "error", msg = firstResultError ?? "Đăng ký thất bại." });
        }

        // Xác nhận email khi người dùng nhấp vào liên kết trong email
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Yêu cầu không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Xác nhận email thành công.");
            }

            return BadRequest("Xác nhận email không thành công.");
        }

        public async Task<IActionResult> LogoutAsync()
        {

            if (User.Identity.IsAuthenticated)
            {
               /* var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var getUser = await this._userManager.FindByIdAsync(userId);
                getUser.lastAssces = DateTime.Now;
                await this._userManager.UpdateAsync(getUser);*/
            }
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
