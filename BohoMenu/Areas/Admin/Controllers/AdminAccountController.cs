
using BohoMenu.Core.DTOs.Requests.Admin;
using BohoMenu.DataBase;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BohoMenu.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Account")]
public class AdminAccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<string> _passwordHasher = new();

    public AdminAccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Categories", new { area = "Admin" });

        ViewData["ReturnUrl"] = returnUrl;
        return View("Login");
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        AdminLoginRequest request,
        string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("Login", request);
        }

        var username = request.Username.Trim();

        var admin = await _context.AdminAccounts
            .SingleOrDefaultAsync(x => x.Username == username);

        if (admin == null)
        {
            ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است.");
            ViewData["ReturnUrl"] = returnUrl;
            return View("Login", request);
        }

        var verification = _passwordHasher.VerifyHashedPassword(
            admin.Username,
            admin.PasswordHash,
            request.Password
        );

        if (verification == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است.");
            ViewData["ReturnUrl"] = returnUrl;
            return View("Login", request);
        }

        // ارتقای خودکار هش در صورت نیاز
        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            admin.PasswordHash = _passwordHasher.HashPassword(
                admin.Username,
                request.Password
            );

            await _context.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, admin.Username),
            new(ClaimTypes.Role, "Admin"),
            new("SecurityStamp", admin.SecurityStamp)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = request.RememberMe,
            AllowRefresh = true
        };

        if (request.RememberMe)
            authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProperties
        );

        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Categories", new { area = "Admin" });
    }

    [HttpGet("ChangePassword")]
    [Authorize(Roles = "Admin")]
    public IActionResult ChangePassword()
    {
        return View("ChangePassword");
    }

    [HttpPost("ChangePassword")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        AdminChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return View("ChangePassword", request);

        var username = User.Identity?.Name;

        var admin = await _context.AdminAccounts
            .SingleOrDefaultAsync(x => x.Username == username);

        if (admin == null)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login");
        }

        var verification = _passwordHasher.VerifyHashedPassword(
            admin.Username,
            admin.PasswordHash,
            request.CurrentPassword
        );

        if (verification == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(
                nameof(request.CurrentPassword),
                "رمز عبور فعلی صحیح نیست."
            );

            return View("ChangePassword", request);
        }

        admin.PasswordHash = _passwordHasher.HashPassword(
            admin.Username,
            request.NewPassword
        );

        // بی‌اعتبار کردن تمام نشست‌های قبلی
        admin.SecurityStamp = Guid.NewGuid().ToString("N");

        await _context.SaveChangesAsync();

        // خروج از نشست فعلی؛ ورود مجدد با رمز جدید لازم است
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        TempData["SuccessMessage"] = "رمز عبور با موفقیت تغییر کرد. لطفاً دوباره وارد شوید.";

        return RedirectToAction("Login");
    }

    [HttpPost("Logout")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction("Login");
    }
}
