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
