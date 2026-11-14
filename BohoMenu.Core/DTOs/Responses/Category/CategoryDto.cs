using BohoMenu.Core.DTOs.Responses.MenuItem;

namespace BohoMenu.Core.DTOs.Responses.Category;

public class CategoryDto
{
    public int Id { get; set; }

    public string Title { get; set; } = "";
    public int TotalItems { get; set; }

    public int ActiveItems { get; set; }
    public List<MenuItemDto> Items { get; set; } = new();
}