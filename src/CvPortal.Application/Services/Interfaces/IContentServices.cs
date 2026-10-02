using CvPortal.Application.DTOs;

namespace CvPortal.Application.Services.Interfaces;

public interface IExperienceService : ICrudService<ExperienceDto>;

public interface IEducationService : ICrudService<EducationDto>;

public interface ISkillService : ICrudService<SkillDto>
{
    Task<IReadOnlyList<SkillGroupDto>> GetGroupedAsync(bool onlyVisible = true, CancellationToken ct = default);
}

public interface IProjectService : ICrudService<ProjectDto>
{
    Task<bool?> ToggleFeaturedAsync(int id, CancellationToken ct = default);
}

public interface ICertificateService : ICrudService<CertificateDto>;

public interface ILanguageService : ICrudService<LanguageDto>;
