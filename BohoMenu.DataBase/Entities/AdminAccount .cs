namespace BohoMenu.DataBase.Entities;

public class AdminAccount : BaseEntity
{
    public string Username { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    // برای بی‌اعتبار کردن نشست‌های ورود قبلی
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
}