namespace BohoMenu.DataBase.Entities;

public class Restaurant : BaseEntity
{
    public string RestaurantName { get; set; } = "";
    public string Tagline { get; set; } = "";
    public string? LogoUrl { get; set; }
}