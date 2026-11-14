using BohoMenu.Core.DTOs.Responses.Restaurant;

namespace BohoMenu.Core.Services.Interfaces;

public interface IMenuService
{
    Task<MenuPageDto?> GetMenuPageAsync(int? categoryId = null, string? query = null);
}