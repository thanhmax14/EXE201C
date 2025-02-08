using Booking.Data;
using Booking.Models;
using Booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Booking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public AdminController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var gettoteldepo = this._context.Dongtiens.Where(u => u.method.ToLower().Trim() == "Nap".ToLower().Trim() && u.IsComplete).ToList();


            var today = DateTime.UtcNow.Date;
            var sevenDaysAgo = today.AddDays(-6);
            var commissionRate = 0.03m;
            var tourEarnings = await _context.DaTours
                .Where(dt => dt.paymentStatus.ToLower() == "paid"
                    && dt.DatePayment.HasValue
                    && dt.DatePayment >= sevenDaysAgo
                    && dt.DatePayment < today.AddDays(1))
                .GroupBy(dt => dt.DatePayment.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(dt => dt.totalPaid ?? 0) * commissionRate
                })
                .ToListAsync();

            // Khởi tạo dữ liệu mặc định cho 7 ngày
            var earningsByDay = Enumerable.Range(0, 7)
                .Select(i => sevenDaysAgo.AddDays(i))
                .ToDictionary(date => date, date => 0m);


            foreach (var item in tourEarnings)
            {
                earningsByDay[item.Date] = item.Revenue;
            }

            var earningsList = earningsByDay.Values.Select(e => (int)e).ToList();


            var sevenDaysAgo1 = today.AddDays(-6);
            var commissionRate1 = 0.03m;
            var hotelEarning = await _context.Datphongs
                .Where(dt => dt.paymentStatus.ToLower() == "paid"
                    && dt.DatePayment.HasValue
                    && dt.DatePayment >= sevenDaysAgo
                    && dt.DatePayment < today.AddDays(1))
                .GroupBy(dt => dt.DatePayment.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(dt => dt.totalPaid ?? 0) * commissionRate1
                })
                .ToListAsync();

            // Khởi tạo dữ liệu mặc định cho 7 ngày
            var earningsByDay1 = Enumerable.Range(0, 7)
                .Select(i => sevenDaysAgo1.AddDays(i))
                .ToDictionary(date => date, date => 0m);


            foreach (var item in hotelEarning)
            {
                earningsByDay1[item.Date] = item.Revenue;
            }

            var hotelearning = earningsByDay1.Values.Select(e => (int)e).ToList();


            var data = new
            {
                Revenue = earningsByDay.Values.Sum(), 
                TotalDeposit = gettoteldepo.Sum(u => Math.Abs(u.sotienthaydoi)),
                Reports = new List<object>
        {
            new { Category = "Tour", Data = earningsList },
            new { Category = "Hotels", Data = hotelearning }
        }
            };

            return Json(data);
        }




        public async Task<IActionResult> ManageSellers()
        {
            var users = await _userManager.Users.Where(x => x.isUpdateProfile == true).ToListAsync(); // Get users first

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Await async call properly
                if (roles.Contains("Seller"))
                {
                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        FirstName = user.firstName,
                        LastName = user.lastName,
                        PhoneNumber = user.PhoneNumber,
                        ProvinceCity = user.Province,
                        District = user.District,
                        Ward = user.Ward,
                        Address = user.address,
                        ZipCode = user.ZipCode,
                        DateJoined = user.joinin,
                        Roles = roles.ToList()
                    });
                }
            }
            return View("manage-sellers", userViewModels);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users.ToListAsync(); // Get users first

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Await async call properly
                if (!roles.Any(r => r == "Admin" || r == "Seller"))
                {
                    if(roles.Count == 0)
                    {
                        roles.Add("Unverified");
                    }
                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        FirstName = user.firstName,
                        LastName = user.lastName,
                        PhoneNumber = user.PhoneNumber,
                        ProvinceCity = user.Province,
                        District = user.District,
                        Ward = user.Ward,
                        Address = user.address,
                        ZipCode = user.ZipCode,
                        DateJoined = user.joinin,
                        Roles = roles.ToList()
                    });
                }
            }

            return View("manage-users", userViewModels);
        }

        public async Task<IActionResult> SalesRegistration()
        {
            var users = await _userManager.Users.Where(x => x.RequestSeller != null).ToListAsync(); // Get users first

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Await async call properly
                if (!roles.Any(r => r == "Admin" || r == "Seller"))
                {
                    if (roles.Count == 0)
                    {
                        roles.Add("Unverified");
                    }
                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        FirstName = user.firstName,
                        LastName = user.lastName,
                        PhoneNumber = user.PhoneNumber,
                        ProvinceCity = user.Province,
                        District = user.District,
                        Ward = user.Ward,
                        Address = user.address,
                        ZipCode = user.ZipCode,
                        DateJoined = user.joinin,
                        Roles = roles.ToList()
                    });
                }
            }

            return View("sales-registration", userViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null) 
            { 
                return NotFound("User not found."); 
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0)
            {
                return Ok("User has no roles assigned.");
            }

            var result1 = await _userManager.RemoveFromRolesAsync(user, roles);
            if (result1.Succeeded)
            {
                var result2 = await _userManager.AddToRoleAsync(user, "Seller");
                if (result2.Succeeded)
                {
                    user.RequestSeller = null;
                    var result = await _userManager.UpdateAsync(user);
                    return RedirectToAction("RedirectToSalesRegistration");
                }
                return RedirectToAction("Erro404", "Home");
            }
            return RedirectToAction("Erro404", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RequestDenied(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            user.RequestSeller = null;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return View("sales-registration");
            }
            return RedirectToAction("Erro404", "Home");
        }

        public IActionResult RedirectToManageSellers()
        {
            return RedirectToAction("ManageSellers");
        }

        public IActionResult RedirectToManageUsers()
        {
            return RedirectToAction("ManageUsers");
        }
        public IActionResult RedirectToSalesRegistration()
        {
            return RedirectToAction("SalesRegistration");
        }
    }
}
