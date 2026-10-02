namespace CvPortal.Tests;

/// <summary>
/// Testlerde "bugün"ü sabitlemek için basit TimeProvider.
/// </summary>
internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
