using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BohoMenu.Core.DTOs.Requests.MenuItem;

public class UpdateMenuItemRequest
{
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = "";

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(1, 999999999)]
    public decimal Price { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }
    public IFormFile? Image { get; set; }

    public bool IsAvailable { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}