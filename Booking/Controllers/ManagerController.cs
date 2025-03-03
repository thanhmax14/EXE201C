﻿using Booking.Data;
using Booking.Models;
using Booking.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Booking.Controllers
{
    [Authorize(Roles = "Seller")]
    public class ManagerController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public ManagerController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var tem = 0;
            var getMyHotel = await this._context.Hotels.Where(u => u.UserID == user.Id).ToListAsync();

            if (getMyHotel.Any())
            {
                foreach (var item in getMyHotel)
                {
                    var getRoom = await this._context.Rooms.Where(u => u.HotelID == item.ID).ToListAsync();

                    if (getRoom.Any())
                    {
                        foreach (var itemRom in getRoom)
                        {
                            var getInfoBook = await this._context.Datphongs.Where(u => u.RoomID == itemRom.RoomID).OrderByDescending(q => q.BookedOn).ToListAsync();
                            if (getInfoBook.Any())
                            {
                                foreach (var itemDatPhong in getInfoBook)
                                {
                                    var getInfoUser = await this._userManager.FindByIdAsync(itemDatPhong.UserID);

                                    if (getInfoUser != null)
                                    {
                                        tem++;
                                    }

                                }
                            }

                        }
                    }

                }
            }

            var counttou = 0;
            var getMyHotel1 = await this._context.Tours.Where(u => u.UserID == user.Id).ToListAsync();

            if (getMyHotel1.Any())
            {
                foreach (var item in getMyHotel1)
                {
                    var getInfoBook1 = await this._context.DaTours.Where(u => u.TourID == item.ID).ToListAsync();
                    if (getInfoBook1.Any())
                    {
                        foreach (var itemDatPhong in getInfoBook1)
                        {
                            var getInfoUser1 = await this._userManager.FindByIdAsync(itemDatPhong.UserID);

                            if (getInfoUser1 != null)
                            {
                                counttou++;
                            }

                        }
                    }
                }
            }
            var temla = new viewModelDash();
            temla.totelBoking = counttou + tem;
            var countRevHo = 0;
            var countHotelden = 0;
            var countHotel = this._context.Hotels.Where(u => u.UserID == user.Id).ToList();
            if (countHotel.Any())
            {
                foreach(var item in countHotel)
                {
                    countHotelden++;
                    var check = this._context.ReviewHotels.Where(u => u.HotelID == item.ID).ToList();
                    if (check.Any())
                    {
                      foreach(var itemcoun in check)
                        {
                            countRevHo++;
                        }
                    }
                }
            }

            var countReviwT = 0;
            var countTo = 0;
            var counttour= this._context.Tours.Where(u => u.UserID == user.Id).ToList();
            if (counttour.Any())
            {
              
                foreach (var item in counttour)
                {
                    countTo++;
                    var check1 = this._context.ReviewTours.Where(u => u.TourID == item.ID).ToList();
                    if (check1.Any())
                    {
                        foreach (var itemc in check1)
                        {
                            countReviwT++;
                        }
                    }
                }
            }

            temla.totekList = countTo + countHotelden;
            temla.totelReviw = countRevHo + countReviwT;


            return View(temla);
        }
         public async Task<IActionResult> Listing()
        {
            var user1 = await _userManager.GetUserAsync(User);
            if (user1 == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var list = new List<ListProduct>();
            var infoHotel = new List<ListHotels>();
            var getHotel = await this._context.Hotels
            .Where(h =>h.UserID ==user1.Id)
            .OrderBy(h => h.Established)
            .ToListAsync();
            var farevo = "";
            foreach (var item in getHotel)
            {
                var user = await this._userManager.FindByIdAsync(item.UserID);
                var temImg = new List<GalleriesImg>();
                var getImg = await this._context.Galleries.Where(u => u.HotelID == item.ID)
             .OrderByDescending(h => h.IsFeatureImage)
             .ToListAsync();
                if (getImg.Any())
                {
                    foreach (var itemImg in getImg)
                    {
                        if (!string.IsNullOrWhiteSpace(itemImg.ImagePath))
                        {
                            temImg.Add(new GalleriesImg { img = itemImg.ImagePath });
                        }
                    }
                }
                var flagUser = await _userManager.GetUserAsync(User);
                if (flagUser != null && await this._context.WishlistHotels.AnyAsync(u => u.UserID == flagUser.Id && u.HotelID == item.ID))
                {
                    farevo = "text-danger";
                }
                else
                {
                    farevo = "";
                }
                infoHotel.Add(new ListHotels
                {
                    HotelID = item.ID,
                    HotelName = item.HotelName,
                    img = temImg,
                    Location = $"{item.City},{item.Country}",
                    NameSeller = user.UserName,
                    NumberReview = 200,
                    price = 502,
                    farovite = farevo

                });
            }

            var InfoTour = new List<ListTour>();
            var getTour = await this._context.Tours
            .Where(h => h.UserID == user1.Id)
            .ToListAsync();
            var farevo1 = "";
            foreach (var item in getTour)
            {
                var user = await this._userManager.FindByIdAsync(item.UserID);
                var temImg = new List<GalleriesImg>();
                var getImg = await this._context.GalleryTours.Where(u => u.TourID == item.ID)
             .OrderByDescending(h => h.IsFeatureImage)
             .ToListAsync();
                if (getImg.Any())
                {
                    foreach (var itemImg in getImg)
                    {
                        if (!string.IsNullOrWhiteSpace(itemImg.ImagePath))
                        {
                            temImg.Add(new GalleriesImg { img = itemImg.ImagePath });
                        }
                    }
                }
                var flagUser = await _userManager.GetUserAsync(User);
                if (flagUser != null && await this._context.WishlistTours.AnyAsync(u => u.UserID == flagUser.Id && u.TourID == item.ID))
                {
                    farevo1 = "text-danger";
                }
                else
                {
                    farevo1 = "";
                }
                InfoTour.Add(new ListTour
                {
                    TourID = item.ID,
                    TourName = item.TourName,
                    img = temImg,
                    Location = $"{item.City},{item.Country}",
                    NameSeller = user.UserName,
                    NumberReview = 200,
                    price = 502,
                    farovite = farevo1

                });
            }

            list.Add(new ListProduct
            {
                Hotels = infoHotel,
                Tour = InfoTour,
            });
            return View(list);
        }

        public async Task<IActionResult> CreateHotel()
        {
            return View();
        }

        [HttpPost]
        
        public async Task<IActionResult> CreateHotel(CreateHotelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["MessseErro"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường nhập.";
                return View(model);
            }

            try
            {
                var user1 = await _userManager.GetUserAsync(User);
                if (user1 == null)
                {
                    TempData["MessseErro"] = "Không tìm thấy người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Erro404", "Home");
                }

                var HotelID = Guid.NewGuid();
                var hotel = new Hotel
                {
                    ID = HotelID,
                    HotelName = model.Name,
                    Category = model.Category,
                    StarRatings = model.StarRating,
                    TotalRooms = model.TotalRooms,
                    MaxCapacity = model.MaxCapacity,
                    Country = model.Country,
                    City = model.City,
                    linkLocation = ExtractUrlFromIframe(model.linkLocation),
                    State = model.State,
                    ZipCode = model.ZipCode,
                    Address = model.Address,
                    Description = model.dess,
                    Address1 = model.Address,
                    UserID = user1.Id

                };
                await this._context.Hotels.AddAsync(hotel);
                await this._context.SaveChangesAsync();
                var TienNghi = new List<string>();
                if (model.Accessibility.Any())
                {
                    foreach (var accessibility in model.Accessibility.Select((value, index) => new { value, index }))
                    {
                        switch (accessibility.index)
                        {
                            case 0:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-wind-2 fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Hồ bơi</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 1:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-coffee fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Cà phê</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 2:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-shopping-bag fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Tiện ích giặt là</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 3:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-finger-scan fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Két an toàn trong phòng</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 4:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-airplane fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Chuyển sân bay</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 5:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-diamonds fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Quầy bar</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 6:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-health fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Phòng tập thể dục</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 7:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-weight fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Phòng gym</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 8:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-headphone fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Lễ tân 24/7</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 9:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-reserve fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Bữa sáng miễn phí</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 10:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-buildings-2 fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Phòng kết nối</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 11:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-car fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Đỗ xe miễn phí</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 12:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-mirroring-screen fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Ti vi</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 13:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-airpod fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>Điều hòa</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            case 14:
                                if (accessibility.value)
                                {
                                    await this._context.Amenities.AddAsync(new Amenity
                                    {
                                        AmenityName = " <div class=\"d-flex align-items-center mb-3\">\r\n" +
                                                      "   <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n" +
                                                      "       <i class=\"isax isax-lovely fs-16\"></i>\r\n" +
                                                      "   </span>\r\n" +
                                                      "   <p>SPA</p>\r\n" +
                                                      "</div>",
                                        HotelID = HotelID,
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }


                if (model.IsRoomService24h.Any())
                {
                    foreach (var service in model.IsRoomService24h.Select((value, index) => new { value, index }))
                    {
                        switch (service.index)
                        {
                            case 0:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ phòng 24h</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 1:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ ăn uống trong phòng</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 2:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ concierge</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 3:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dọn phòng hàng ngày</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 4:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ quầy lễ tân</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 5:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Nhà hàng tại chỗ</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 6:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Hỗ trợ nhận/trả phòng</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 7:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Gửi hành lý miễn phí</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 8:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ giặt là và là ủi</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 9:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ giặt khô</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 10:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ làm tóc và làm đẹp</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 11:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Điều trị spa trong phòng</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 12:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Valet parking</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 13:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ giữ trẻ</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 14:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ gọi đánh thức</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 15:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ thông dịch</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 16:
                                if (service.value)
                                {
                                    await this._context.Services.AddAsync(new Service
                                    {
                                        HotelID = HotelID,
                                        ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ đổi ngoại tệ</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (model.RoomTypes.Any())
                {
                    foreach (var room in model.RoomTypes.Select((value, index) => new { value, index }))
                    {
                        switch (room.index)
                        {
                            case 0:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng đơn</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 1:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng đôi</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 2:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng giường đơn</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 3:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng Deluxe</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 4:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng Suite</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 5:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng Junior Suite</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 6:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng gia đình</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 7:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng kết nối</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 8:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng accessible</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 9:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng Studio</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 10:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng Penthouse</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 11:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Biệt thự</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 12:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng hạng kinh tế</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 13:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng có view thành phố</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 14:
                                if (room.value)
                                {
                                    await this._context.RoomTypes.AddAsync(new RoomType
                                    {
                                        HotelID = HotelID,
                                        RoomTypeName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-send-sqaure-2 fs-16\"></i>\r\n    </span>\r\n    <p>Phòng có view biển</p>\r\n</div>"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }


                if (model.Highlights != null && model.Highlights.Any())
                {
                    var highlights = new List<Highlight>();

                    foreach (var item in model.Highlights)
                    {
                        highlights.Add(new Highlight
                        {
                            HotelID = HotelID,
                            HighlightText = item
                        });
                    }

                    await this._context.Highlights.AddRangeAsync(highlights);
                    await this._context.SaveChangesAsync();
                }


                {

                }
                if (model.Images != null && model.Images.Any())
                {

                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "hotel_images");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    foreach (var file in model.Images)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        await this._context.Galleries.AddAsync(new Gallery
                        {
                            HotelID = HotelID,
                            ImagePath = "/" + Path.Combine("uploads", "hotel_images", fileName),
                            IsFeatureImage =true
                        });
                    }
                    await this._context.SaveChangesAsync();
                }

                TempData["Messse"] = "Khách sạn đã được tạo thành công, vui lòng tạo phòng!";
                return View(model);
            }
            catch (Exception ex)
            {
              
                TempData["MessseErro"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(model);
            }
        }

       public static string ExtractUrlFromIframe(string input)
        {
            try
            {
              
                string pattern = @"https:\/\/www\.google\.com\/maps\/embed\?pb=[^""]+";
                Match match = Regex.Match(input, pattern);

                // Kiểm tra nếu có kết quả
                if (match.Success)
                {
                    return match.Value;
                }
                else
                {
                    Console.WriteLine("No URL matched the pattern.");
                    return "";
                }
            }
            catch (Exception ex)
            {
             
                Console.WriteLine($"An error occurred: {ex.Message}");
                return "";
            }
        }
        public async Task<IActionResult> CreateRom()
        {
            var hotels = await _context.Hotels
                               .Select(h => new { h.ID, h.HotelName })
                               .ToListAsync();

            ViewBag.Hotels = hotels.Any()
         ? new SelectList(hotels, "ID", "HotelName")
         : new SelectList(Enumerable.Empty<SelectListItem>());
            return View();
        }
        [HttpPost]
       public async Task<IActionResult> CreateRom(CreateRoomView model)
        {
            var hotels = await _context.Hotels
                                    .Select(h => new { h.ID, h.HotelName })
                                    .ToListAsync();

            ViewBag.Hotels = hotels.Any()
         ? new SelectList(hotels, "ID", "HotelName")
         : new SelectList(Enumerable.Empty<SelectListItem>());


            if (!ModelState.IsValid)
            {
                TempData["MessseErro"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường nhập.";
                return View(model);
            }

            try
            {
                var romID = Guid.NewGuid();
                var tem = new Room
                {
                    RoomID = romID,
                    RoomName = model.RoomName,
                    BedType = model.BedType,
                    Description = model.dess,
                    HotelID = model.HotelID,
                    RoomSize = model.RoomSize,
                    View = model.View,
                    PricePerNight = model.PricePerNight,
                    Sleeps = model.Sleeps,
                    MaximumOccupancy = model.MaximumOccupancy,
                    RoomType = model.RoomType,
                };
                await this._context.Rooms.AddAsync(tem);
                await this._context.SaveChangesAsync();


                if (model.Services.Any())
                {
                    foreach (var service in model.Services.Select((value, index) => new { value, index }))
                    {
                        switch (service.index)
                        {
                            case 0:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ phòng 24 giờ"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 1:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ ăn uống trong phòng"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 2:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ lễ tân"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 3:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dọn phòng hàng ngày"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 4:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Hỗ trợ nhận/trả phòng"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 5:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Gửi hành lý miễn phí"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 6:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ giặt là và ủi đồ"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 7:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ giặt khô"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 8:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ đỗ xe"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 9:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ trông trẻ"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 10:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = "Dịch vụ gọi đánh thức"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 11:
                                if (service.value)
                                {
                                    await this._context.ServiceRooms.AddAsync(new ServiceRoom
                                    {
                                        RoomID = romID,
                                        ServiceName = ""
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;                          
                            default:
                                break;
                        }
                    }
                }


                if (model.Accessibility.Any())
                {
                    foreach (var service in model.Accessibility.Select((value, index) => new { value, index }))
                    {
                        switch (service.index)
                        {
                            case 0:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Khả năng tiếp cận cho người sử dụng xe lăn"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 1:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Cảnh báo hình ảnh trong hành lang"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 2:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Thang máy"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 3:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Biển báo chữ nổi/Braille"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 4:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Phòng gym có thể tiếp cận cho người sử dụng xe lăn"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 5:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Trung tâm kinh doanh có thể tiếp cận cho người sử dụng xe lăn"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 6:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Phòng chờ có thể tiếp cận cho người sử dụng xe lăn"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 7:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Quầy lễ tân có thể tiếp cận cho người sử dụng xe lăn"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 8:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Lối vào không có bậc hoặc có ramp"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 9:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Cửa tự động"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 10:
                                if (service.value)
                                {
                                    await this._context.AccessibilityRooms.AddAsync(new AccessibilityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Thanh vịn trong phòng tắm"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;                       
                            default:
                                break;
                        }
                    }
                }



                if (model.Amenities.Any())
                {
                    foreach (var service in model.Amenities.Select((value, index) => new { value, index }))
                    {
                        switch (service.index)
                        {
                            case 0:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Wi-Fi miễn phí"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 1:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Điều hòa không khí"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 2:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Máy sưởi"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 3:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Két sắt"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 4:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Bàn là/Ủi"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 5:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Máy sấy tóc"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 6:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Loa Bluetooth"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 7:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Sofa/Khu vực ngồi"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;
                            case 8:
                                if (service.value)
                                {
                                    await this._context.AmenityRooms.AddAsync(new AmenityRoom
                                    {
                                        RoomID = romID,
                                        AmenityName = "Gối và chăn extra"
                                    });
                                    await this._context.SaveChangesAsync();
                                }
                                break;                         
                            default:
                                break;
                        }
                    }
                }
                if (model.GalleryImages != null && model.GalleryImages.Any())
                {

                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "roomImage");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    foreach (var file in model.GalleryImages)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        await this._context.GalleryRooms.AddAsync(new GalleryRoom
                        {
                            RoomID = romID,
                            ImagePath = "/" + Path.Combine("uploads", "roomImage", fileName)
                        });
                    }
                    await this._context.SaveChangesAsync();
                }
                TempData["Messse"] = "Khách sạn đã được tạo thành công, vui lòng tạo phòng!";
                return View(model);


            }
            catch(Exception ex)
            {
                TempData["MessseErro"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(model);
            }
        }

        public async Task<IActionResult> HotelEdit(Guid id)
        {
            var infoHotel = await this._context.Hotels.FirstOrDefaultAsync(u => u.ID == id);
            var tem = new EditViewModels();

            if (infoHotel == null)
            {
                return RedirectToAction("Erro404");
            }
            else
            {
                try
                {
                    tem.ID = infoHotel.ID;
                    tem.State = infoHotel.State;
                    tem.StarRating = infoHotel.StarRatings;
                    tem.Address = infoHotel.Address;
                    tem.linkLocation = infoHotel.linkLocation;
                    tem.Category = infoHotel.Category;
                    tem.dess = infoHotel.Description;
                    tem.ZipCode = infoHotel.ZipCode;
                    tem.City = infoHotel.City;
                    tem.TotalRooms = infoHotel.TotalRooms;
                    tem.MaxCapacity = infoHotel.MaxCapacity;
                    tem.Country = infoHotel.Country;
                    tem.Name = infoHotel.HotelName;

                    var getServices = await this._context.Services.Where(u => u.HotelID == infoHotel.ID).ToListAsync();
                    var serveccs = new List<bool>(new bool[18]);

                    if (getServices.Any())
                    {
                        foreach (var service in getServices)
                        {
                            if (service.ServiceName.Contains("Dịch vụ phòng 24h"))
                            {
                                serveccs[0] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ ăn uống trong phòng"))
                            {
                                serveccs[1] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ concierge"))
                            {
                                serveccs[2] = true;
                            }
                            else if (service.ServiceName.Contains("Dọn phòng hàng ngày"))
                            {
                                serveccs[3] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ quầy lễ tân"))
                            {
                                serveccs[4] = true;
                            }
                            else if (service.ServiceName.Contains("Nhà hàng tại chỗ"))
                            {
                                serveccs[5] = true;
                            }
                            else if (service.ServiceName.Contains("Hỗ trợ nhận/trả phòng"))
                            {
                                serveccs[6] = true;
                            }
                            else if (service.ServiceName.Contains("Gửi hành lý miễn phí"))
                            {
                                serveccs[7] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ giặt là và là ủi"))
                            {
                                serveccs[8] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ giặt khô"))
                            {
                                serveccs[9] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ làm tóc và làm đẹp"))
                            {
                                serveccs[10] = true;
                            }
                            else if (service.ServiceName.Contains("Điều trị spa trong phòng"))
                            {
                                serveccs[11] = true;
                            }
                            else if (service.ServiceName.Contains("Valet parking"))
                            {
                                serveccs[12] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ giữ trẻ"))
                            {
                                serveccs[13] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ gọi đánh thức"))
                            {
                                serveccs[14] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ thông dịch"))
                            {
                                serveccs[15] = true;
                            }
                            else if (service.ServiceName.Contains("Dịch vụ đổi ngoại tệ"))
                            {
                                serveccs[16] = true;
                            }
                        }
                    }
                    else
                    {
                        serveccs = Enumerable.Repeat(false, 18).ToList();
                    }


                    var getSAccessibility = await this._context.Amenities.Where(u => u.HotelID == infoHotel.ID).ToListAsync();
                    var Accessibility = new List<bool>(new bool[18]);

                    // Danh sách các dịch vụ cần kiểm tra
                    var amenitiesList = new[]
                    {
    "Hồ bơi", "Cà phê", "Tiện ích giặt là", "Két an toàn trong phòng",
    "Chuyển sân bay", "Quầy bar", "Phòng tập thể dục", "Phòng gym",
    "Lễ tân 24/7", "Bữa sáng miễn phí", "Phòng kết nối", "Đỗ xe miễn phí",
    "Ti vi", "Điều hòa", "SPA"
};

                    if (getSAccessibility.Any())
                    {
                        foreach (var service in getSAccessibility)
                        {
                            for (int i = 0; i < amenitiesList.Length; i++)
                            {
                                if (service.AmenityName.Contains(amenitiesList[i]))
                                {
                                    Accessibility[i] = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        Accessibility = Enumerable.Repeat(false, 18).ToList();
                    }

                    var getRoomType = await this._context.RoomTypes.Where(u => u.HotelID == infoHotel.ID).ToListAsync();
                    var RoomTypes = new List<bool>(new bool[15]);

                    // Danh sách các tên phòng cần kiểm tra
                    var roomTypeNames = new[]
                    {
    "Phòng đơn", "Phòng đôi", "Phòng giường đơn", "Phòng Deluxe", "Phòng Suite",
    "Phòng Junior Suite", "Phòng gia đình", "Phòng kết nối", "Phòng accessible",
    "Phòng Studio", "Phòng Penthouse", "Biệt thự", "Phòng hạng kinh tế",
    "Phòng có view thành phố", "Phòng có view biển"
};

                    if (getRoomType.Any())
                    {
                        foreach (var room in getRoomType)
                        {
                            for (int i = 0; i < roomTypeNames.Length; i++)
                            {
                                if (room.RoomTypeName.Contains(roomTypeNames[i]))
                                {
                                    RoomTypes[i] = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        RoomTypes = Enumerable.Repeat(false, 15).ToList();
                    }

                    tem.Accessibility = Accessibility;
                    tem.RoomTypes = RoomTypes;
                    tem.IsRoomService24h = serveccs;

                    var Highlights = await this._context.Highlights.Where(u => u.HotelID == infoHotel.ID).Select(h => h.HighlightText).ToListAsync();
                    tem.Highlights.AddRange(Highlights);
                    var imgExit = await this._context.Galleries.Where(u => u.HotelID == infoHotel.ID).OrderByDescending(h => h.IsFeatureImage).Select(h => h.ImagePath).ToListAsync();

                    tem.ExistingImages = imgExit;
                    return View(tem);
                }
                catch(Exception ex)
                {
                    TempData["MessseErro"] = $"Có lỗi xảy ra: {ex.Message}";
                    return NotFound();
                }
            }

        }
        [HttpPost]
        public async Task<IActionResult> HotelEdit(EditViewModels model)
        {
            if (!ModelState.IsValid)
            {
                TempData["MessseErro"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường nhập.";
                return View(model);
            }

            try
            {
                var infoHotel = await this._context.Hotels.FirstOrDefaultAsync(u => u.ID == model.ID);
                if(infoHotel == null)
                {
                    TempData["MessseErro"] = "Không tìm thấy khách sạn";
                    return View(model);
                }

                var user1 = await _userManager.GetUserAsync(User);
                if (user1 == null)
                {
                    TempData["MessseErro"] = "Không tìm thấy người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Erro404", "Home");
                }

                infoHotel.HotelName = model.Name;
                infoHotel.Category = model.Category;
                infoHotel.StarRatings = model.StarRating;
                infoHotel.TotalRooms = model.TotalRooms;
                infoHotel.MaxCapacity = model.MaxCapacity;
                infoHotel.Country = model.Country;
                infoHotel.City = model.City;
                infoHotel.linkLocation = ExtractUrlFromIframe(model.linkLocation);
                 infoHotel.State = model.State;
                infoHotel.ZipCode = model.ZipCode;
                infoHotel.Address = model.Address;
                infoHotel.Description = model.dess;
                infoHotel.Address1 = model.Address; ;
                infoHotel.UserID = user1.Id;

              
               this._context.Hotels.Update(infoHotel);
                await this._context.SaveChangesAsync();
                var TienNghi = new List<string>();
                if (model.Accessibility.Any())
                {
                    foreach (var accessibility in model.Accessibility.Select((value, index) => new { value, index }))
                    {
                        switch (accessibility.index)
                        {
                            case 0:
                                if (!accessibility.value) // Kiểm tra nếu giá trị là false
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Hồ bơi") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 1:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Cà phê") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 2:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Tiện ích giặt là") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 3:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Két an toàn trong phòng") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 4:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Chuyển sân bay") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 5:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Quầy bar") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 6:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Phòng tập thể dục") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 7:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Phòng gym") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 8:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Lễ tân 24/7") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 9:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Bữa sáng miễn phí") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 10:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Phòng kết nối") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 11:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Đỗ xe miễn phí") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 12:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Ti vi") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 13:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("Điều hòa") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            case 14:
                                if (!accessibility.value)
                                {
                                    var amenityToDelete = await this._context.Amenities
                                        .Where(a => a.AmenityName.Contains("SPA") && a.HotelID == model.ID)
                                        .FirstOrDefaultAsync();

                                    if (amenityToDelete != null)
                                    {
                                        this._context.Amenities.Remove(amenityToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }


                if (model.RoomTypes.Any())
                {
                    var roomTypeNames = new[]
{
    "Phòng đơn", "Phòng đôi", "Phòng giường đơn", "Phòng Deluxe", "Phòng Suite",
    "Phòng Junior Suite", "Phòng gia đình", "Phòng kết nối", "Phòng accessible",
    "Phòng Studio", "Phòng Penthouse", "Biệt thự", "Phòng hạng kinh tế",
    "Phòng có view thành phố", "Phòng có view biển"
};
                    foreach (var service in model.RoomTypes.Select((value, index) => new { value, index }))
                    {
                        var roomTypeName = roomTypeNames[service.index];
                        switch (service.index)
                        {
                            case 0:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                        .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                        .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 1:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                       .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 2:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                        .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                        .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 3:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                         .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 4:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                        .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                        .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 5:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                        .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                        .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 6:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                       .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 7:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                      .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 8:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                      .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 9:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                       .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 10:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                      .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 11:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                       .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 12:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                      .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 13:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                      .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 14:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                       .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 15:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                      .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 16:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.RoomTypes
                       .Where(a => a.RoomTypeName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.RoomTypes.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Danh sách các dịch vụ bạn muốn kiểm tra
                var serviceNames = new[]
                {
    "Dịch vụ phòng 24h", "Dịch vụ ăn uống trong phòng", "Dịch vụ concierge", "Dọn phòng hàng ngày",
    "Dịch vụ quầy lễ tân", "Nhà hàng tại chỗ", "Hỗ trợ nhận/trả phòng", "Gửi hành lý miễn phí",
    "Dịch vụ giặt là và là ủi", "Dịch vụ giặt khô", "Dịch vụ làm tóc và làm đẹp", "Điều trị spa trong phòng",
    "Valet parking", "Dịch vụ giữ trẻ", "Dịch vụ gọi đánh thức", "Dịch vụ thông dịch", "Dịch vụ đổi ngoại tệ",""
};

                if (model.IsRoomService24h.Any())
                {
                  
                    foreach (var service in model.IsRoomService24h.Select((value, index) => new { value, index }))
                    {
                        var roomTypeName = serviceNames[service.index];
                        switch (service.index)
                        {
                            case 0:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ phòng 24h</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 1:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                      .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ ăn uống trong phòng</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 2:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                      .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ concierge</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 3:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                      .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dọn phòng hàng ngày</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 4:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                      .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ quầy lễ tân</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 5:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Nhà hàng tại chỗ</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 6:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                        .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                        .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Hỗ trợ nhận/trả phòng</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 7:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Gửi hành lý miễn phí</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 8:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                      .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ giặt là và là ủi</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 9:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ giặt khô</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 10:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                      .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ làm tóc và làm đẹp</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            case 11:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Điều trị spa trong phòng</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                           
                                 case 12:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Valet parking</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            
                                 case 13:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ giữ trẻ</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            
                                 case 14:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ gọi đánh thức</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                           
                                 case 15:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ thông dịch</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                           
                                 case 16:
                                if (!service.value)
                                {
                                    var RoomTypeToDelete = await this._context.Services
                       .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                       .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete != null)
                                    {
                                        this._context.Services.Remove(RoomTypeToDelete);
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var RoomTypeToDelete = await this._context.Services
                                                         .Where(a => a.ServiceName.Contains(roomTypeName) && a.HotelID == model.ID)
                                                         .FirstOrDefaultAsync();
                                    if (RoomTypeToDelete == null)
                                    {
                                        await this._context.Services.AddAsync(new Service
                                        {
                                            HotelID = model.ID,
                                            ServiceName = "<div class=\"d-flex align-items-center mb-3\">\r\n    <span class=\"avatar avatar-md bg-primary-transparent rounded-circle me-2\">\r\n        <i class=\"isax isax-verify fs-16\"></i>\r\n    </span>\r\n    <p>Dịch vụ ăn uống trong phòng</p>\r\n</div>"
                                        });
                                        await this._context.SaveChangesAsync();
                                    }
                                }
                                break;
                            


                        }
                    }
                }


                if (model.Highlights != null && model.Highlights.Any())
                {
                    var highlights = new List<Highlight>();

                    foreach (var item in model.Highlights)
                    {
                        var RoomTypeToDelete = await this._context.Highlights
                      .Where(a => a.HighlightText.Contains(item) && a.HotelID == model.ID)
                      .FirstOrDefaultAsync();
                        if (RoomTypeToDelete != null)
                        {
                            this._context.Highlights.Remove(RoomTypeToDelete);
                            await this._context.SaveChangesAsync();
                        }
                    }
                    await this._context.Highlights.AddRangeAsync(highlights);
                    await this._context.SaveChangesAsync();
                }

                if (model.Images != null && model.Images.Any())
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "hotel_images");

  
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }


                    var oldGalleries = await this._context.Galleries
                                                           .Where(g => g.HotelID == model.ID)
                                                           .ToListAsync();

                    if (oldGalleries.Any())
                    {
                        // Xóa ảnh cũ khỏi thư mục
                      
                        // Xóa tất cả ảnh cũ trong cơ sở dữ liệu
                        this._context.Galleries.RemoveRange(oldGalleries);
                        await this._context.SaveChangesAsync();
                    }

                    // Thêm ảnh mới vào thư mục và cơ sở dữ liệu
                    foreach (var file in model.Images)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Tạo tên file duy nhất
                        var filePath = Path.Combine(uploadPath, fileName);

                        // Lưu ảnh vào thư mục
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Thêm thông tin ảnh vào cơ sở dữ liệu
                        await this._context.Galleries.AddAsync(new Gallery
                        {
                            HotelID = model.ID,
                            ImagePath = "/" + Path.Combine("uploads", "hotel_images", fileName)
                        });
                    }

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    await this._context.SaveChangesAsync();
                }

              
                var imgExit = await this._context.Galleries.Where(u => u.HotelID == infoHotel.ID).OrderByDescending(h => h.IsFeatureImage).Select(h => h.ImagePath).ToListAsync();

                model.ExistingImages = imgExit;
                TempData["Messse"] = "Khách sạn đã được câp nhật thông tin thành công!!";
                return View(model);
            }
            catch (Exception ex)
            {

                TempData["MessseErro"] = $"Có lỗi xảy ra: {ex.Message}";
                return View(model);
            }




        }



        public async Task<IActionResult> CreateTour()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTour(CreateTourViewModeks model)
        {
            if (!ModelState.IsValid)
            {

                if (!ModelState.IsValid)
                {
                    TempData["MessseErro"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường nhập.";
                    return View(model);
                }

            }

            var user1 = await _userManager.GetUserAsync(User);
            if (user1 == null)
            {
                TempData["MessseErro"] = "Không tìm thấy người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Erro404", "Home");
            }
            try
            {
                var tourID = Guid.NewGuid();
                var tem = new tour
                {
                    ID = tourID,
                    Category = model.Category,
                    City = model.City,
                    Country = model.Country,
                    Address = model.Address,
                    ZipCode = model.ZipCode,
                    Description = model.dess,
                    Destination = model.Destination,
                    EndDATE = model.endDate,
                    startDate = model.startDate,
                    DurationDay = "",
                    totalPreoPle = model.totalProple,
                    linkLocation = ExtractUrlFromIframe(model.linkLocation),
                    UserID = user1.Id,
                    DurationNight = "",
                    price = model.Pricing,
                    minAge = model.mindAge,
                    State = model.State,
                    TourName = model.Name
                };
                await this._context.Tours.AddAsync(tem);
                await this._context.SaveChangesAsync();

                if (model.Images != null && model.Images.Any())
                {

                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tour_image");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    foreach (var file in model.Images)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        await this._context.GalleryTours.AddAsync(new GalleryTour
                        {
                            TourID = tourID,
                            ImagePath = "/" + Path.Combine("uploads", "tour_image", fileName),
                            IsFeatureImage =true
                        });
                    }
                    await this._context.SaveChangesAsync();
                    
                }
                TempData["Messse"] = "Khách sạn đã được câp nhật thông tin thành công!!";
                return View(model);
            }
            catch(Exception e)
            {
                TempData["MessseErro"] = $"Có lỗi xảy ra: {e.Message}";
                return View(model);
            }
        }




        public async Task<IActionResult> EditTour(Guid id)
        {
            var model = await this._context.Tours.FindAsync(id);
            if(model == null)
            {
                return RedirectToAction("Erro404");
            }
            else
            {
                var tem = new EditTourViewModels
                {
                    ID = model.ID,
                    Category = model.Category,
                    City = model.City,
                    Country = model.Country,
                    Address = model.Address,
                    ZipCode = model.ZipCode,
                    dess = model.Description,
                    Destination = model.Destination,
                    endDate = model.EndDATE,
                    startDate = model.startDate,
                    totalProple = model.totalPreoPle,
                    linkLocation = model.linkLocation,
                    Pricing = model.price,
                    mindAge = model.minAge,
                    State = model.State,
                    Name = model.TourName
                };
                var imgExit = await this._context.GalleryTours.Where(u => u.TourID == model.ID).OrderByDescending(h => h.IsFeatureImage).Select(h => h.ImagePath).ToListAsync();

                tem.ExistingImages = imgExit;


                return View(tem);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditTour(EditTourViewModels model)
        {

            if (!ModelState.IsValid)
            {

                if (!ModelState.IsValid)
                {
                    TempData["MessseErro"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường nhập.";
                    return View(model);
                }

            }

            var user1 = await _userManager.GetUserAsync(User);
            if (user1 == null)
            {
                TempData["MessseErro"] = "Không tìm thấy người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Erro404", "Home");
            }
            try
            {
                var getInfo = await this._context.Tours.FindAsync(model.ID);

                if (getInfo == null)
                {
                    TempData["MessseErro"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại các trường nhập.";
                    return View(model);
                }


                getInfo.Category = model.Category;
                getInfo.City = model.City;
                getInfo.Country = model.Country;
                getInfo.Address = model.Address;
                getInfo.ZipCode = model.ZipCode;
                getInfo.Description = model.dess;
                getInfo.Destination = model.Destination;
                getInfo.EndDATE = model.endDate;
                getInfo.startDate = model.startDate;
                getInfo.DurationDay = "";
                getInfo.totalPreoPle = model.totalProple;
                getInfo.linkLocation = ExtractUrlFromIframe(model.linkLocation);
                getInfo.UserID = user1.Id;
                getInfo.DurationNight = "";
                getInfo.price = model.Pricing;
                getInfo.minAge = model.mindAge;
                getInfo.State = model.State;
                getInfo.TourName = model.Name;

                this._context.Tours.Update(getInfo);
                await this._context.SaveChangesAsync();


                if (model.Images != null && model.Images.Any())
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "hotel_images");


                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }


                    var oldGalleries = await this._context.GalleryTours
                                                           .Where(g => g.TourID == model.ID)
                                                           .ToListAsync();

                    if (oldGalleries.Any())
                    {
                        this._context.GalleryTours.RemoveRange(oldGalleries);
                        await this._context.SaveChangesAsync();
                    }

                    foreach (var file in model.Images)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        await this._context.GalleryTours.AddAsync(new GalleryTour
                        {
                            TourID = model.ID,
                            ImagePath = "/" + Path.Combine("uploads", "hotel_images", fileName),
                            IsFeatureImage = true
                        });
                    }
                    await this._context.SaveChangesAsync();
                }




                var imgExit = await this._context.GalleryTours.Where(u => u.TourID == getInfo.ID).OrderByDescending(h => h.IsFeatureImage).Select(h => h.ImagePath).ToListAsync();

                model.ExistingImages = imgExit;
                TempData["Messse"] = "Khách sạn đã được câp nhật thông tin thành công!!";
                return View(model);

            }
            catch (Exception e)
            {
                TempData["MessseErro"] = $"Có lỗi xảy ra: {e.Message}";
                return View(model);
            }

        }


        public async Task<IActionResult> HotelsBooking()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var tem = new List<ManaHotelBoking>();
            var getMyHotel = await this._context.Hotels.Where(u => u.UserID == user.Id).ToListAsync();

            if (getMyHotel.Any())
            {
                foreach(var item in getMyHotel)
                {
                    var getRoom = await this._context.Rooms.Where(u => u.HotelID == item.ID).ToListAsync();

                    if (getRoom.Any())
                    {
                        foreach(var itemRom in getRoom)
                        {
                            var getInfoBook = await this._context.Datphongs.Where(u => u.RoomID == itemRom.RoomID).OrderByDescending(q => q.BookedOn).ToListAsync();
                            if (getInfoBook.Any())
                            {
                                 foreach(var itemDatPhong in getInfoBook)
                                {
                                    var getInfoUser = await this._userManager.FindByIdAsync(itemDatPhong.UserID);

                                    if (getInfoUser != null)
                                    {

                                        tem.Add(new ManaHotelBoking
                                        {
                                            orderCode = itemDatPhong.OrderID,
                                            addreddHotel = $"{item.Country}",
                                            hotemName = item.HotelName,
                                            user = getInfoUser.firstName +" "+ getInfoUser.lastName,
                                            addresUser = getInfoUser.Province,
                                           roomNAME = itemRom.RoomName,
                                           guest = itemDatPhong.Guests,
                                           date = itemDatPhong.NoOfDate,
                                           price = itemDatPhong.totalPaid,
                                           booked = itemDatPhong.BookedOn+"",
                                           DatphongID = itemDatPhong.ID,
                                           status = itemDatPhong.progress

                                        }); 
                                    }

                                }    
                            }

                        }
                    }

                }
            }

            return View(tem);
        }


        public async Task<IActionResult> ToursBooking()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var tem = new List<ManaHotelBoking>();
            var getMyHotel = await this._context.Tours.Where(u => u.UserID == user.Id).ToListAsync();

            if (getMyHotel.Any())
            {
                foreach (var item in getMyHotel)
                {                 
                            var getInfoBook = await this._context.DaTours.Where(u => u.TourID == item.ID).ToListAsync();
                            if (getInfoBook.Any())
                            {
                                foreach (var itemDatPhong in getInfoBook)
                                {
                                    var getInfoUser = await this._userManager.FindByIdAsync(itemDatPhong.UserID);

                                    if (getInfoUser != null)
                                    {

                                        tem.Add(new ManaHotelBoking
                                        {
                                            orderCode = itemDatPhong.OrderID,
                                            addreddHotel = $"{item.Country}",
                                            hotemName = item.TourName,
                                            user = getInfoUser.firstName + " " + getInfoUser.lastName,
                                            addresUser = getInfoUser.Province,
                                            guest = itemDatPhong.Guests,
                                            date = itemDatPhong.NoOfDate,
                                            price = itemDatPhong.totalPaid,
                                            booked = itemDatPhong.BookedOn + "",
                                            DatphongID = itemDatPhong.ID,
                                            status = itemDatPhong.progress

                                        });
                                    }

                                }
                            }
                }
            }

            return View(tem);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingStatistics()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }
            var tem = 0;
            var getMyHotel = await this._context.Hotels.Where(u => u.UserID == user.Id).ToListAsync();

            if (getMyHotel.Any())
            {
                foreach (var item in getMyHotel)
                {
                    var getRoom = await this._context.Rooms.Where(u => u.HotelID == item.ID).ToListAsync();

                    if (getRoom.Any())
                    {
                        foreach (var itemRom in getRoom)
                        {
                            var getInfoBook = await this._context.Datphongs.Where(u => u.RoomID == itemRom.RoomID).OrderByDescending(q => q.BookedOn).ToListAsync();
                            if (getInfoBook.Any())
                            {
                                foreach (var itemDatPhong in getInfoBook)
                                {
                                    var getInfoUser = await this._userManager.FindByIdAsync(itemDatPhong.UserID);

                                    if (getInfoUser != null)
                                    {
                                        tem++;
                                    }

                                }
                            }

                        }
                    }

                }
            }

            var counttou = 0;
            var getMyHotel1 = await this._context.Tours.Where(u => u.UserID == user.Id).ToListAsync();

            if (getMyHotel1.Any())
            {
                foreach (var item in getMyHotel1)
                {
                    var getInfoBook1 = await this._context.DaTours.Where(u => u.TourID == item.ID).ToListAsync();
                    if (getInfoBook1.Any())
                    {
                        foreach (var itemDatPhong in getInfoBook1)
                        {
                            var getInfoUser1 = await this._userManager.FindByIdAsync(itemDatPhong.UserID);

                            if (getInfoUser1 != null)
                            {
                                counttou++;
                            }

                        }
                    }
                }
            }
            var hotelBookings = tem;
            var tourBookings = counttou;

            return Json(new { hotels = hotelBookings, tours = tourBookings });
        }



        [HttpGet]
        public async Task<IActionResult> GetEarningsData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var hotelEarnings = await _context.Datphongs
                .Where(dp => _context.Rooms
                    .Where(r => _context.Hotels
                        .Where(h => h.UserID == user.Id)
                        .Select(h => h.ID)
                        .Contains(r.HotelID))
                    .Select(r => r.RoomID)
                    .Contains(dp.RoomID))
                .GroupBy(dp => dp.DatePayment.Value.Month)
                .Select(g => new { Month = g.Key, Revenue = g.Sum(dp => dp.totalPaid ?? 0) })
                .ToListAsync();

            var tourEarnings = await _context.DaTours
                .Where(dt => _context.Tours
                    .Where(t => t.UserID == user.Id)
                    .Select(t => t.ID)
                    .Contains(dt.TourID))
                .GroupBy(dt => dt.DatePayment.Value.Month)
                .Select(g => new { Month = g.Key, Revenue = g.Sum(dt => dt.totalPaid ?? 0) })
                .ToListAsync();

            var earningsByMonth = Enumerable.Range(1, 12).Select(month => new
            {
                Month = month,
                HotelRevenue = hotelEarnings.FirstOrDefault(e => e.Month == month)?.Revenue ?? 0,
                TourRevenue = tourEarnings.FirstOrDefault(e => e.Month == month)?.Revenue ?? 0
            }).ToList();

            return Json(earningsByMonth);
        }

        public async Task<IActionResult> Review()
        {
            var tem = new List<readcmt>();


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Erro404", "Home");
            }

            var getMyHotel = await this._context.Hotels.Where(u => u.UserID == user.Id).ToListAsync();

            if (getMyHotel.Any())
            {
                foreach (var item in getMyHotel)
                {
                    var getHotel = await this._context.ReviewHotels.Where(u => u.HotelID == item.ID).OrderByDescending(q => q.datecmt).ToListAsync();

                    if (getHotel.Any())
                    {
                        foreach (var itemRom in getHotel)
                        {
                            var infoUser = await this._userManager.FindByIdAsync(itemRom.UserID);
                            var infoSeller = await this._userManager.FindByIdAsync(item.UserID);
                            tem.Add(new readcmt
                            {
                                ID = itemRom.ID,
                                datecmt = itemRom.datecmt,
                                daterelay = itemRom.dateRelay,
                                imgSeller = infoSeller.img,
                                imgUser = infoUser.img,
                                rating = itemRom.rating + ".0",
                                relay = itemRom.relay ?? "",
                                SellerName = $"{infoSeller.firstName} {infoSeller.lastName}",
                                UserName = $"{infoUser.firstName} {infoUser.lastName}",
                                cmt = itemRom.cmt,
                                nameSever = item.HotelName

                            });
                        }
                    }

                }
            }

            var getRoom = await this._context.Tours.Where(u => u.UserID == user.Id).ToListAsync();

            if (getRoom.Any())
            {
                foreach (var item in getRoom)
                {
                    var getHotel = await this._context.ReviewTours.Where(u => u.TourID == item.ID).OrderByDescending(q => q.datecmt).ToListAsync();

                    if (getHotel.Any())
                    {

                        foreach (var itemRom in getHotel)
                        {
                            var infoUser = await this._userManager.FindByIdAsync(itemRom.UserID);
                            var infoSeller = await this._userManager.FindByIdAsync(item.UserID);
                            tem.Add(new readcmt
                            {
                                ID = itemRom.ID,
                                datecmt = itemRom.datecmt,
                                daterelay = itemRom.dateRelay,
                                imgSeller = infoSeller.img,
                                imgUser = infoUser.img,
                                rating = itemRom.rating + ".0",
                                relay = itemRom.relay ?? "",
                                SellerName = $"{infoSeller.firstName} {infoSeller.lastName}",
                                UserName = $"{infoUser.firstName} {infoUser.lastName}",
                                cmt = itemRom.cmt,
                                nameSever = item.TourName

                            });
                        }
                    }

                }
            }
            return View(tem);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(Guid id, string comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { status = "error", message = "Bạn phải đăng nhập thể thực hiện hành động này!" });
            }
            if (string.IsNullOrEmpty(comment))
            {
                return Ok(new { status = "error", msg="Bạn phải điền câu trả lời." });;
            }
            var getInfoCme = await this._context.ReviewHotels.FindAsync(id);
            if (getInfoCme != null)
            {
                getInfoCme.relay = comment;
                getInfoCme.dateRelay = DateTime.Now;
                this._context.ReviewHotels.Update(getInfoCme);
                this._context.SaveChanges();
                return Ok(new { status = "success", msg = "Trả lời bình luận thành công" }); ;
            }
            var getinfoTour = await this._context.ReviewTours.FindAsync(id);
            if (getinfoTour != null)
            {
                getInfoCme.relay = comment;
                getInfoCme.dateRelay = DateTime.Now;
                this._context.ReviewTours.Update(getinfoTour);
                this._context.SaveChanges();
                return Ok(new { status = "success", msg = "Trả lời bình luận thành công" }); ;
            }


            return Ok(new { status = "error", msg = "Bình luận không tồn tại.!" }); ;
        }

    }
}
