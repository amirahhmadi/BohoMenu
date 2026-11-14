using BohoMenu.Core.Services;
using BohoMenu.Core.Services.Interfaces;
using BohoMenu.DataBase;
using BohoMenu.DataBase.Entities;
using BohoMenu.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Application Services
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();

// Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.LogoutPath = "/Admin/Account/Logout";

        options.Cookie.Name = "BohoMenu.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;

        options.Events.OnValidatePrincipal = async context =>
        {
            var db = context.HttpContext.RequestServices
                .GetRequiredService<ApplicationDbContext>();

            var username = context.Principal?.Identity?.Name;

            var cookieStamp = context.Principal?
                .FindFirst("SecurityStamp")?.Value;

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(cookieStamp))
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                return;
            }

            var admin = await db.AdminAccounts
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username == username);

            if (admin == null || admin.SecurityStamp != cookieStamp)
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Initialize Admin Account
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    var username = app.Configuration["AdminAccount:Username"]?.Trim();
    var passwordHash = app.Configuration["AdminAccount:PasswordHash"];

    // اگر اطلاعات حساب در تنظیمات موجود باشد
    // و هنوز هیچ حساب مدیری در دیتابیس نباشد
    if (!string.IsNullOrWhiteSpace(username) &&
        !string.IsNullOrWhiteSpace(passwordHash))
    {
        var adminExists = await db.AdminAccounts.AnyAsync();

        if (!adminExists)
        {
            var admin = new AdminAccount
            {
                Username = username,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString("N")
            };

            db.AdminAccounts.Add(admin);

            await db.SaveChangesAsync();
        }
    }
}

// HTTP Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Categories}/{action=Index}/{id?}"
);

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Menu}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();