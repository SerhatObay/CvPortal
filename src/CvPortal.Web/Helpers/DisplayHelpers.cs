using System.Globalization;

namespace CvPortal.Web.Helpers;

public static class DisplayHelpers
{
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    /// <summary>UTC zamanı "5 dk önce", "2 gün önce" gibi göreli metne çevirir.</summary>
    public static string Relative(DateTime utc)
    {
        var diff = DateTime.UtcNow - utc;
        return diff.TotalSeconds switch
        {
            < 60 => "az önce",
            < 3600 => $"{(int)diff.TotalMinutes} dk önce",
            < 86400 => $"{(int)diff.TotalHours} saat önce",
            < 604800 => $"{(int)diff.TotalDays} gün önce",
            _ => utc.ToLocalTime().ToString("dd MMM yyyy", Turkish)
        };
    }

    public static string LocalDateTime(DateTime utc) => utc.ToLocalTime().ToString("dd MMMM yyyy, HH:mm", Turkish);

    public static string Initials(string name)
        => string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => char.ToUpper(p[0], Turkish)));

    public static string Truncate(string? text, int length)
        => string.IsNullOrEmpty(text) || text.Length <= length ? text ?? string.Empty : text[..length].TrimEnd() + "…";
}
