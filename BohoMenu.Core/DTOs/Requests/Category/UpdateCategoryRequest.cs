using System.ComponentModel.DataAnnotations;

namespace BohoMenu.Core.DTOs.Requests.Category;

public class UpdateCategoryRequest
{
    [Required(ErrorMessage = "شناسه دسته‌بندی الزامی است.")]
    [Range(1, int.MaxValue)]
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان دسته‌بندی الزامی است.")]
    [StringLength(100, ErrorMessage = "عنوان حداکثر ۱۰۰ کاراکتر است.")]
    public string Title { get; set; } = "";
}