using BohoMenu.Core.DTOs.Responses.Category;

namespace BohoMenu.Core.DTOs.Responses.Restaurant;

public class RestaurantDto
{
    public int Id { get; set; }

    public string RestaurantName { get; set; } = "";

    public string Tagline { get; set; } = "";

    public string? LogoUrl { get; set; }
}