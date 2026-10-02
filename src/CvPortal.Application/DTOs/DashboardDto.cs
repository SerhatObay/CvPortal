namespace CvPortal.Application.DTOs;

/// <summary>
/// Admin panel ana sayfasındaki özet bilgiler.
/// </summary>
public class DashboardDto
{
    public int ExperienceCount { get; init; }
    public int EducationCount { get; init; }
    public int SkillCount { get; init; }
    public int ProjectCount { get; init; }
    public int CertificateCount { get; init; }
    public int LanguageCount { get; init; }
    public int MessageCount { get; init; }
    public int UnreadMessageCount { get; init; }
    public int HiddenItemCount { get; init; }
    public int ProfileCompletion { get; init; }
    public IReadOnlyList<string> MissingProfileFields { get; init; } = [];
    public IReadOnlyList<ContactMessageDto> RecentMessages { get; init; } = [];
    public IReadOnlyList<CategoryCountDto> SkillsByCategory { get; init; } = [];
    public IReadOnlyList<CategoryCountDto> TopTechnologies { get; init; } = [];
}

public record CategoryCountDto(string Name, int Count);
