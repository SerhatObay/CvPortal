using System.Globalization;

namespace CvPortal.Application.Common;

/// <summary>
/// Tarih aralıklarını CV'de gösterilecek biçime çevirir: "Oca 2022 - Halen", "2 yıl 3 ay".
/// </summary>
public static class DateRangeFormatter
{
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    public static string Period(DateOnly start, DateOnly? end)
        => $"{MonthYear(start)} - {(end is null ? "Halen" : MonthYear(end.Value))}";

    public static string MonthYear(DateOnly date) => date.ToString("MMM yyyy", Turkish);

    public static string Duration(DateOnly start, DateOnly? end, DateOnly? today = null)
    {
        var finish = end ?? today ?? DateOnly.FromDateTime(DateTime.Today);
        var totalMonths = TotalMonths(start, finish);

        var years = totalMonths / 12;
        var months = totalMonths % 12;

        return (years, months) switch
        {
            (0, _) => $"{months} ay",
            (_, 0) => $"{years} yıl",
            _ => $"{years} yıl {months} ay"
        };
    }

    /// <summary>
    /// İki tarih arasındaki ay sayısı (başlangıç ayı dahil, en az 1).
    /// </summary>
    public static int TotalMonths(DateOnly start, DateOnly end)
    {
        var months = (end.Year - start.Year) * 12 + end.Month - start.Month + 1;
        return Math.Max(1, months);
    }
}
