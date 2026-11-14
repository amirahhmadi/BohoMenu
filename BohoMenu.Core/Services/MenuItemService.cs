
using BohoMenu.Core.DTOs.Requests.MenuItem;
using BohoMenu.Core.DTOs.Responses.MenuItem;
using BohoMenu.Core.DTOs.Responses.Category;
using BohoMenu.Core.Services.Interfaces;
using BohoMenu.DataBase;
using BohoMenu.DataBase.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace BohoMenu.Core.Services;

public class MenuItemService : IMenuItemService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    private const long MaxImageSize = 2 * 1024 * 1024;

    private readonly string[] _allowedExtensions =
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public MenuItemService(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // مسیر ذخیره تصاویر
    private string UploadFolder
    {
        get
        {
            var webRoot = _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(
                    _environment.ContentRootPath,
                    "wwwroot");

            return Path.Combine(
                webRoot,
                "uploads",
                "menu-items");
        }
    }

    // دریافت تمام آیتم‌ها
    public async Task<List<MenuItemDto>> GetAllAsync(int? categoryId = null)
    {
        var query = _context.MenuItems
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        return await query
            .OrderByDescending(x => x.Id)
            .Select(x => new MenuItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                ImageUrl = x.ImageUrl,
                IsAvailable = x.IsAvailable,
                CategoryId = x.CategoryId,
                CategoryTitle = x.Category.Title
            })
            .ToListAsync();
    }

    // دریافت آیتم بر اساس شناسه
    public async Task<MenuItemDto?> GetByIdAsync(int id)
    {
        return await _context.MenuItems
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new MenuItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                ImageUrl = x.ImageUrl,
                IsAvailable = x.IsAvailable,
                CategoryId = x.CategoryId,
                CategoryTitle = x.Category.Title
            })
            .FirstOrDefaultAsync();
    }

    // دریافت دسته‌بندی‌ها
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Title = x.Title
            })
            .ToListAsync();
    }

    // افزودن آیتم جدید
    public async Task<int?> CreateAsync(CreateMenuItemRequest request)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Id == request.CategoryId);

        if (!categoryExists)
            return null;

        string? imageUrl = null;

        if (request.Image != null && request.Image.Length > 0)
            imageUrl = await SaveImageAsync(request.Image);

        var item = new MenuItem
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            ImageUrl = imageUrl,
            IsAvailable = request.IsAvailable,
            CategoryId = request.CategoryId
        };

        try
        {
            await _context.MenuItems.AddAsync(item);
            await _context.SaveChangesAsync();

            return item.Id;
        }
        catch
        {
            if (imageUrl != null)
                DeleteImage(imageUrl);

            throw;
        }
    }

    // ویرایش آیتم
    public async Task<bool> UpdateAsync(UpdateMenuItemRequest request)
    {
        var item = await _context.MenuItems
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item == null)
            return false;

        var categoryExists = await _context.Categories
            .AnyAsync(x => x.Id == request.CategoryId);

        if (!categoryExists)
            return false;

        var oldImageUrl = item.ImageUrl;
        string? newImageUrl = null;

        // فقط در صورت انتخاب تصویر جدید، فایل ذخیره می‌شود
        if (request.Image != null && request.Image.Length > 0)
            newImageUrl = await SaveImageAsync(request.Image);

        item.Name = request.Name.Trim();
        item.Description = request.Description?.Trim();
        item.Price = request.Price;
        item.IsAvailable = request.IsAvailable;
        item.CategoryId = request.CategoryId;

        // اگر تصویر جدید انتخاب شده باشد، آدرس آن جایگزین می‌شود
        if (newImageUrl != null)
            item.ImageUrl = newImageUrl;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch
        {
            if (newImageUrl != null)
                DeleteImage(newImageUrl);

            throw;
        }

        // پس از ذخیره موفق دیتابیس، تصویر قبلی حذف می‌شود
        if (newImageUrl != null && !string.IsNullOrWhiteSpace(oldImageUrl))
            DeleteImage(oldImageUrl);

        return true;
    }

    // حذف آیتم
    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.MenuItems
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return false;

        var imageUrl = item.ImageUrl;

        _context.MenuItems.Remove(item);

        await _context.SaveChangesAsync();

        // بعد از حذف موفق رکورد، تصویر هم حذف می‌شود
        if (!string.IsNullOrWhiteSpace(imageUrl))
            DeleteImage(imageUrl);

        return true;
    }

    // ذخیره تصویر روی سرور
    private async Task<string> SaveImageAsync(
        Microsoft.AspNetCore.Http.IFormFile image)
    {
        var extension = Path.GetExtension(image.FileName)
            .ToLowerInvariant();

        if (!_allowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                "فرمت تصویر باید JPG، PNG یا WEBP باشد.");

        if (image.Length > MaxImageSize)
            throw new InvalidOperationException(
                "حجم تصویر نباید بیشتر از ۲ مگابایت باشد.");

        if (!await IsValidImageAsync(image, extension))
            throw new InvalidOperationException(
                "محتوای فایل انتخاب شده یک تصویر معتبر نیست.");

        Directory.CreateDirectory(UploadFolder);

        var fileName = $"{Guid.NewGuid():N}{extension}";

        var filePath = Path.Combine(UploadFolder, fileName);

        try
        {
            await using var stream = new FileStream(
                filePath,
                FileMode.CreateNew,
                FileAccess.Write);

            await image.CopyToAsync(stream);
        }
        catch
        {
            DeleteImage($"/uploads/menu-items/{fileName}");
            throw;
        }

        return $"/uploads/menu-items/{fileName}";
    }

    // بررسی امضای واقعی فایل تصویر
    private static async Task<bool> IsValidImageAsync(
        Microsoft.AspNetCore.Http.IFormFile image,
        string extension)
    {
        var header = new byte[12];

        await using var stream = image.OpenReadStream();

        var bytesRead = await stream.ReadAsync(header.AsMemory());

        if (extension == ".jpg" || extension == ".jpeg")
        {
            return bytesRead >= 3 &&
                   header[0] == 0xFF &&
                   header[1] == 0xD8 &&
                   header[2] == 0xFF;
        }

        if (extension == ".png")
        {
            byte[] signature =
            {
                0x89, 0x50, 0x4E, 0x47,
                0x0D, 0x0A, 0x1A, 0x0A
            };

            return bytesRead >= 8 &&
                   header.Take(8).SequenceEqual(signature);
        }

        if (extension == ".webp")
        {
            return bytesRead >= 12 &&
                   header[0] == 'R' &&
                   header[1] == 'I' &&
                   header[2] == 'F' &&
                   header[3] == 'F' &&
                   header[8] == 'W' &&
                   header[9] == 'E' &&
                   header[10] == 'B' &&
                   header[11] == 'P';
        }

        return false;
    }

    // حذف فایل تصویر
    private void DeleteImage(string imageUrl)
    {
        const string imagePrefix = "/uploads/menu-items/";

        if (!imageUrl.StartsWith(
                imagePrefix,
                StringComparison.OrdinalIgnoreCase))
            return;

        var fileName = Path.GetFileName(imageUrl);

        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var filePath = Path.Combine(UploadFolder, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}
