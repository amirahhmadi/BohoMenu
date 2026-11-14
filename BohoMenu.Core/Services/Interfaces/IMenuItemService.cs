using BohoMenu.Core.DTOs.Requests.MenuItem;
using BohoMenu.Core.DTOs.Responses.Category;
using BohoMenu.Core.DTOs.Responses.MenuItem;

namespace BohoMenu.Core.Services.Interfaces;

public interface IMenuItemService
{
    Task<List<MenuItemDto>> GetAllAsync(int? categoryId = null);
    Task<MenuItemDto?> GetByIdAsync(int id);

        Task<List<CategoryDto>> GetCategoriesAsync();

        Task<int?> CreateAsync(CreateMenuItemRequest request);

        Task<bool> UpdateAsync(UpdateMenuItemRequest request);

        Task<bool> DeleteAsync(int id);
}