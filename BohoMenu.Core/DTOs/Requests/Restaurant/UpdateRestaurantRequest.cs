using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BohoMenu.Core.DTOs.Requests.Restaurant;

public class UpdateRestaurantRequest
{
    [Required(ErrorMessage = "نام کافه الزامی است")]
    [StringLength(150)]
    public string RestaurantName { get; set; } = "";

    [StringLength(300, ErrorMessage = "شعار کافه نباید بیشتر از ۳۰۰ کاراکتر باشد")]
    public string? Tagline { get; set; }

    public IFormFile? Logo { get; set; }

    // آدرس لوگوی فعلی برای نمایش در فرم
    public string? LogoUrl { get; set; }
}