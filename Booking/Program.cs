using Booking.Data;
using Booking.Models;
using Booking.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using static Booking.Services.EmailService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register default identity using ThanhMMODbContext for Identity stores
builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddErrorDescriber<VietnameseIdentityErrorDescriber>().AddDefaultTokenProviders(); ;
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddRazorPages();

var mailsettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsettings);
builder.Services.AddTransient<IEmailSender, SendMailService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // Đường dẫn trang đăng nhập
        options.AccessDeniedPath = "/Home/AccessDenied"; // Đường dẫn khi bị từ chối truy cập
        options.LoginPath = "/Home/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Home/Logout"; // Đường dẫn đến trang đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi truy cập bị từ chối
        options.ReturnUrlParameter = "ReturnUrl"; // Xác định tham số query string
        options.ExpireTimeSpan = TimeSpan.FromDays(14); // Thời gian lưu cookie
        options.SlidingExpiration = true; // Làm mới thời gian hết hạn cookie
    })
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Lưu thông tin đăng nhập qua Cookies
    options.CallbackPath = "/signin-google";

    // Hiển thị trang chọn tài khoản
    options.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
        return Task.CompletedTask;
    };

    // Xử lý lỗi khi xác thực
    options.Events.OnRemoteFailure = u =>
    {
        u.Response.Redirect("/Home/Login");
        u.HandleResponse();
        return Task.CompletedTask;
    };
}).AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
    microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
    microsoftOptions.CallbackPath = builder.Configuration["Authentication:Microsoft:CallbackPath"];
    microsoftOptions.Scope.Add("email");
    microsoftOptions.Scope.Add("profile");
    microsoftOptions.Scope.Add("openid");
    microsoftOptions.SaveTokens = true;
});



builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization(options =>
{
    // Chính sách cho Admin
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));

    // Chính sách cho CTV
    options.AddPolicy("CTVPolicy", policy =>
        policy.RequireRole("Admin", "Seller"));

});



builder.Services.Configure<IdentityOptions>(options =>
{

    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    //setting for user
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;

});
// Cấu hình Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login"; // Đường dẫn đến trang đăng nhập
    options.LogoutPath = "/Home/Logout"; // Đường dẫn đến trang đăng xuất
    options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi truy cập bị từ chối
    options.ReturnUrlParameter = "ReturnUrl"; // Xác định tham số query string
});


















var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();


await SeedDataAsync(app);
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
static async Task SeedDataAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        // Create roles if they don't exist
        string[] roles = { "User", "Admin", "Seller" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }


        // Create a default User user if it doesn't exist
        var User = await userManager.FindByEmailAsync("thanhpqce171732@fpt.edu.vn");
        if (User == null)
        {
           User = new AppUser { UserName = "thanhmax14", Email = "thanhpqce171732@fpt.edu.vn" };
            var result = await userManager.CreateAsync(User, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(User, "User");
            }
        }


        
        // Create a default admin user if it doesn't exist
        var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
        if (adminUser == null)
        {
            adminUser = new AppUser { UserName = "admin", Email = "admin@gmail.com" };
            var result = await userManager.CreateAsync(adminUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create a default CTV user if it doesn't exist
        var ctvUser = await userManager.FindByEmailAsync("ctv@gmail.com");
        if (ctvUser == null)
        {
            ctvUser = new AppUser { UserName = "ctv", Email = "ctv@gmail.com" };
            var result = await userManager.CreateAsync(ctvUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(ctvUser, "Seller");
            }
        }
    }
}