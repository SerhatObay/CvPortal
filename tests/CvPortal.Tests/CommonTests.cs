using CvPortal.Application.Common;

namespace CvPortal.Tests;

public class DateRangeFormatterTests
{
    [Theory]
    [InlineData(2023, 3, 2023, 3, "1 ay")]
    [InlineData(2023, 1, 2023, 12, "1 yıl")]
    [InlineData(2021, 1, 2023, 2, "2 yıl 2 ay")]
    public void Duration_ReturnsHumanReadableText(int sy, int sm, int ey, int em, string expected)
    {
        var result = DateRangeFormatter.Duration(new DateOnly(sy, sm, 1), new DateOnly(ey, em, 1));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Duration_UsesTodayWhenEndDateIsNull()
    {
        var result = DateRangeFormatter.Duration(new DateOnly(2025, 1, 1), null, today: new DateOnly(2025, 6, 15));
        Assert.Equal("6 ay", result);
    }

    [Fact]
    public void Period_ShowsHalenForCurrentPosition()
    {
        var result = DateRangeFormatter.Period(new DateOnly(2023, 3, 1), null);
        Assert.EndsWith("- Halen", result);
    }
}

public class TextListTests
{
    [Fact]
    public void FromCsv_TrimsAndRemovesDuplicates()
    {
        var result = TextList.FromCsv(" C#, .NET ,c#,, Docker ");
        Assert.Equal(["C#", ".NET", "Docker"], result);
    }

    [Fact]
    public void FromLines_SplitsByNewLine()
    {
        var result = TextList.FromLines("Birinci madde\r\n\r\nİkinci madde\n");
        Assert.Equal(["Birinci madde", "İkinci madde"], result);
    }

    [Fact]
    public void FromCsv_ReturnsEmptyListForBlankInput()
        => Assert.Empty(TextList.FromCsv("   "));
}
