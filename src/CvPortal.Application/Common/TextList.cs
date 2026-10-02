namespace CvPortal.Application.Common;

/// <summary>
/// Liste alanlarını formlarda tek bir metin kutusu ile düzenleyebilmek için yardımcılar.
/// </summary>
public static class TextList
{
    public static List<string> FromCsv(string? value) => Split(value, [',', ';', '\n']);

    public static List<string> FromLines(string? value) => Split(value, ['\n']);

    public static string ToCsv(IEnumerable<string>? items) => string.Join(", ", items ?? []);

    public static string ToLines(IEnumerable<string>? items) => string.Join(Environment.NewLine, items ?? []);

    private static List<string> Split(string? value, char[] separators)
        => string.IsNullOrWhiteSpace(value)
            ? []
            : value.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                   .Distinct(StringComparer.OrdinalIgnoreCase)
                   .ToList();
}
