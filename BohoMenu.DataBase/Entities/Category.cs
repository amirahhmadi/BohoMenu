namespace BohoMenu.DataBase.Entities;

public class Category : BaseEntity
{
    public string Title { get; set; } = "";
    public List<MenuItem> Items { get; set; } = new();
}