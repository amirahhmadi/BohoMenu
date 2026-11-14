using BohoMenu.Core.DTOs.Requests.Category;
using BohoMenu.Core.DTOs.Responses.Category;

namespace BohoMenu.Core.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();

    Task<CategoryDto?> GetByIdAsync(int id);

    Task<int?> CreateAsync(CreateCategoryRequest request);

    Task<bool> UpdateAsync(UpdateCategoryRequest request);

    Task<bool> DeleteAsync(int id);
}