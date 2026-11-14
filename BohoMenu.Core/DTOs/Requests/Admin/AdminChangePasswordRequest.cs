using System.ComponentModel.DataAnnotations;

namespace BohoMenu.Core.DTOs.Requests.Admin;

public class AdminChangePasswordRequest
{
    [Required(ErrorMessage = "رمز عبور فعلی الزامی است")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = "";

    [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "رمز عبور باید حداقل ۸ کاراکتر باشد")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; } = "";

    [Required(ErrorMessage = "تکرار رمز عبور جدید الزامی است")]
    [Compare(nameof(NewPassword),
        ErrorMessage = "تکرار رمز عبور با رمز جدید مطابقت ندارد")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
}