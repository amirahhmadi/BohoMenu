
using BohoMenu.Core.DTOs.Responses.Category;
using BohoMenu.Core.DTOs.Responses.MenuItem;
using BohoMenu.Core.DTOs.Responses.Restaurant;
using BohoMenu.Core.Services.Interfaces;
using BohoMenu.DataBase;
using Microsoft.EntityFrameworkCore;

namespace BohoMenu.Core.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;

    public MenuService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuPageDto?> GetMenuPageAsync(
        int? categoryId = null,
        string? query = null)
    {
        // دریافت اطلاعات رستوران
        var restaurant = await _context.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (restaurant == null)
            return null;

        // دریافت دسته‌بندی‌ها و فقط آیتم‌های موجود
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Title = category.Title,

                Items = category.Items
                    .Where(item => item.IsAvailable)
                    .OrderBy(item => item.Id)
                    .Select(item => new MenuItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Description = item.Description,
                        Price = item.Price,
                        ImageUrl = item.ImageUrl,
                        IsAvailable = item.IsAvailable
                    })
                    .ToList()
            })
            .ToListAsync();

        // بررسی دسته‌بندی انتخاب شده
        var selectedCategoryId = categoryId.HasValue &&
            categories.Any(x => x.Id == categoryId.Value)
                ? categoryId
                : null;

        // دریافت آیتم‌های موجود برای نمایش
        var itemsQuery = _context.MenuItems
            .AsNoTracking()
            .Where(x => x.IsAvailable);

        // فیلتر دسته‌بندی
        if (selectedCategoryId.HasValue)
        {
            itemsQuery = itemsQuery.Where(
                x => x.CategoryId == selectedCategoryId.Value
            );
        }

        // جستجو
        if (!string.IsNullOrWhiteSpace(query))
        {
            var search = query.Trim();

            itemsQuery = itemsQuery.Where(x =>
                x.Name.Contains(search) ||
                (x.Description != null &&
                 x.Description.Contains(search))
            );
        }

        // دریافت نتایج
        var results = await itemsQuery
            .OrderBy(x => x.Id)
            .Select(item => new MenuItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                IsAvailable = item.IsAvailable
            })
            .ToListAsync();

        // ساخت خروجی نهایی
        return new MenuPageDto
        {
            RestaurantName = restaurant.RestaurantName,
            Tagline = restaurant.Tagline,
            LogoUrl = restaurant.LogoUrl,
            Categories = categories,
            SelectedCategoryId = selectedCategoryId,
            Query = query,
            Results = results
        };
    }
}
