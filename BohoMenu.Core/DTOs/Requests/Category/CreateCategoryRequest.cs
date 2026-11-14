using System.ComponentModel.DataAnnotations;

namespace BohoMenu.Core.DTOs.Requests.Category;

public class CreateCategoryRequest
{
    [Required(ErrorMessage = "عنوان دسته‌بندی الزامی است.")]
    [StringLength(100, ErrorMessage = "عنوان حداکثر ۱۰۰ کاراکتر است.")]
    public string Title { get; set; } = "";
}