using BohoMenu.Core.DTOs.Requests.Category;
using BohoMenu.Core.DTOs.Responses.Category;
using BohoMenu.Core.DTOs.Responses.MenuItem;
using BohoMenu.Core.Services.Interfaces;
using BohoMenu.DataBase;
using BohoMenu.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace BohoMenu.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    // دریافت تمام دسته‌بندی‌ها
    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Title = x.Title,

                // تعداد کل آیتم‌ها
                TotalItems = x.Items.Count(),

                // تعداد آیتم‌های فعال
                ActiveItems = x.Items.Count(item => item.IsAvailable),

                // لیست آیتم‌ها
                Items = x.Items.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                    IsAvailable = item.IsAvailable
                }).ToList()
            })
            .ToListAsync();
    }

    // دریافت یک دسته‌بندی
    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Title = x.Title,

                Items = x.Items.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ImageUrl = item.ImageUrl,
                    IsAvailable = item.IsAvailable
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    // افزودن دسته‌بندی
    public async Task<int?> CreateAsync(CreateCategoryRequest request)
    {
        var title = request.Title.Trim();

        // جلوگیری از ثبت دسته‌بندی تکراری
        var exists = await _context.Categories
            .AnyAsync(x => x.Title == title);

        if (exists)
            return null;

        var category = new Category
        {
            Title = title
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return category.Id;
    }

    // ویرایش دسته‌بندی
    public async Task<bool> UpdateAsync(UpdateCategoryRequest request)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (category == null)
            return false;

        var title = request.Title.Trim();

        // بررسی تکراری نبودن عنوان در دسته‌بندی‌های دیگر
        var exists = await _context.Categories
            .AnyAsync(x => x.Id != request.Id && x.Title == title);

        if (exists)
            return false;

        category.Title = title;

        await _context.SaveChangesAsync();

        return true;
    }

    // حذف دسته‌بندی
    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
            return false;

        // اگر دسته‌بندی محصول داشته باشد، اجازه حذف نمی‌دهیم
        var hasItems = await _context.MenuItems
            .AnyAsync(x => x.CategoryId == id);

        if (hasItems)
            return false;

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return true;
    }
}
