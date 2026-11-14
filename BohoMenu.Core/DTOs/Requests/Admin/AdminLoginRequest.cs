using System.ComponentModel.DataAnnotations;

namespace BohoMenu.Core.DTOs.Requests.Admin;

public class AdminLoginRequest
{
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "رمز عبور الزامی است")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }
}