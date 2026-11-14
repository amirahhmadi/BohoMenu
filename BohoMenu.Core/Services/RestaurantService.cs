using BohoMenu.Core.DTOs.Requests.Restaurant;
using BohoMenu.Core.DTOs.Responses.Restaurant;
using BohoMenu.Core.Services.Interfaces;
using BohoMenu.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace BohoMenu.Core.Services;

public class RestaurantService : IRestaurantService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public RestaurantService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<RestaurantDto?> GetAsync()
    {
        return await _context.Restaurants
            .AsNoTracking()
            .Select(x => new RestaurantDto
            {
                Id = x.Id,
                RestaurantName = x.RestaurantName,
                Tagline = x.Tagline,
                LogoUrl = x.LogoUrl
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(UpdateRestaurantRequest request)
    {
        var restaurant = await _context.Restaurants.FirstOrDefaultAsync();

        if (restaurant == null)
            return false;

        restaurant.RestaurantName = request.RestaurantName.Trim();
        restaurant.Tagline = request.Tagline?.Trim() ?? "";

        string? newLogoUrl = null;

        if (request.Logo != null && request.Logo.Length > 0)
        {
            var extension = Path.GetExtension(request.Logo.FileName).ToLowerInvariant();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("فرمت تصویر لوگو معتبر نیست.");

            if (request.Logo.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException("حجم لوگو نباید بیشتر از ۲ مگابایت باشد.");

            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "restaurant");

            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Logo.CopyToAsync(stream);
            }

            newLogoUrl = $"/uploads/restaurant/{fileName}";
        }

        var oldLogoUrl = restaurant.LogoUrl;

        if (newLogoUrl != null)
            restaurant.LogoUrl = newLogoUrl;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            if (newLogoUrl != null)
                DeleteLogo(newLogoUrl);

            throw;
        }

        if (newLogoUrl != null && !string.IsNullOrWhiteSpace(oldLogoUrl))
            DeleteLogo(oldLogoUrl);

        return true;
    }

    private void DeleteLogo(string logoUrl)
    {
        var fileName = Path.GetFileName(logoUrl);

        var filePath = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "restaurant",
            fileName);

        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
    }
}