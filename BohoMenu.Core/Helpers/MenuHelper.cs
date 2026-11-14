using System.Globalization;

namespace BohoMenu.Core.Helpers;

public static class MenuHelper
{
    // 145000 -> ۱۴۵٬۰۰۰
    public static string Price(decimal price) =>
        ToPersianDigits(price.ToString("N0", CultureInfo.InvariantCulture)).Replace(",", "٬");

    public static string TagLabel(string tag) => tag switch
    {
        "vegan" => "گیاهی",
        "spicy" => "تند",
        "new"   => "جدید",
        _       => tag
    };

    private static string ToPersianDigits(string s) =>
        string.Concat(s.Select(c => c is >= '0' and <= '9' ? (char)('۰' + (c - '0')) : c));
}
