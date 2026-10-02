namespace CvPortal.Application.DTOs;

/// <summary>
/// Herkese açık CV sayfası / API için tüm yayındaki içeriği tek seferde taşıyan model.
/// </summary>
public class CvDto
{
    public required ProfileDto Profile { get; init; }
    public required CvStatsDto Stats { get; init; }
    public IReadOnlyList<ExperienceDto> Experiences { get; init; } = [];
    public IReadOnlyList<EducationDto> Educations { get; init; } = [];
    public IReadOnlyList<SkillGroupDto> SkillGroups { get; init; } = [];
    public IReadOnlyList<ProjectDto> Projects { get; init; } = [];
    public IReadOnlyList<CertificateDto> Certificates { get; init; } = [];
    public IReadOnlyList<LanguageDto> Languages { get; init; } = [];
}

public record SkillGroupDto(string Category, IReadOnlyList<SkillDto> Skills);

public record CvStatsDto(int YearsOfExperience, int CompanyCount, int ProjectCount, int TechnologyCount, int CertificateCount);
