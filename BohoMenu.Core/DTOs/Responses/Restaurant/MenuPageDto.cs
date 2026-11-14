using BohoMenu.Core.DTOs.Responses.Category;
using BohoMenu.Core.DTOs.Responses.MenuItem;

namespace BohoMenu.Core.DTOs.Responses.Restaurant;

public class MenuPageDto
{
    public string RestaurantName { get; set; } = "";

    public string Tagline { get; set; } = "";

    public string? LogoUrl { get; set; }

    public List<CategoryDto> Categories { get; set; } = new();

    public int? SelectedCategoryId { get; set; }

    public string? Query { get; set; }

    public List<MenuItemDto> Results { get; set; } = new();
}