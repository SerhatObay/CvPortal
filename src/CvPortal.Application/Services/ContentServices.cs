using CvPortal.Application.Abstractions;
using CvPortal.Application.Common;
using CvPortal.Application.DTOs;
using CvPortal.Application.Mapping;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Entities;

namespace CvPortal.Application.Services;

public sealed class ExperienceService(IRepository<Experience> repository, TimeProvider time)
    : CrudServiceBase<Experience, ExperienceDto>(repository, time), IExperienceService
{
    protected override ExperienceDto ToDto(Experience entity) => entity.ToDto();
    protected override void Apply(ExperienceDto dto, Experience entity) => dto.Apply(entity);

    protected override IEnumerable<Experience> Sort(IEnumerable<Experience> items)
        => items.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.StartDate);
}

public sealed class EducationService(IRepository<Education> repository, TimeProvider time)
    : CrudServiceBase<Education, EducationDto>(repository, time), IEducationService
{
    protected override EducationDto ToDto(Education entity) => entity.ToDto();
    protected override void Apply(EducationDto dto, Education entity) => dto.Apply(entity);

    protected override IEnumerable<Education> Sort(IEnumerable<Education> items)
        => items.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.StartDate);
}

public sealed class SkillService(IRepository<Skill> repository, TimeProvider time)
    : CrudServiceBase<Skill, SkillDto>(repository, time), ISkillService
{
    protected override SkillDto ToDto(Skill entity) => entity.ToDto();
    protected override void Apply(SkillDto dto, Skill entity) => dto.Apply(entity);

    protected override IEnumerable<Skill> Sort(IEnumerable<Skill> items)
        => items.OrderBy(x => x.Category).ThenBy(x => x.DisplayOrder).ThenByDescending(x => x.Proficiency);

    public async Task<IReadOnlyList<SkillGroupDto>> GetGroupedAsync(bool onlyVisible = true, CancellationToken ct = default)
    {
        var skills = await GetAllAsync(onlyVisible, ct);
        return skills
            .GroupBy(s => s.Category)
            .OrderBy(g => g.Key)
            .Select(g => new SkillGroupDto(g.Key.GetDisplayName(), g.ToList()))
            .ToList();
    }
}

public sealed class ProjectService(IRepository<Project> repository, TimeProvider time)
    : CrudServiceBase<Project, ProjectDto>(repository, time), IProjectService
{
    protected override ProjectDto ToDto(Project entity) => entity.ToDto();
    protected override void Apply(ProjectDto dto, Project entity) => dto.Apply(entity);

    protected override IEnumerable<Project> Sort(IEnumerable<Project> items)
        => items.OrderByDescending(x => x.IsFeatured).ThenBy(x => x.DisplayOrder);

    public async Task<bool?> ToggleFeaturedAsync(int id, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        entity.IsFeatured = !entity.IsFeatured;
        entity.UpdatedAt = Time.GetUtcNow().UtcDateTime;
        await Repository.UpdateAsync(entity, ct);
        return entity.IsFeatured;
    }
}

public sealed class CertificateService(IRepository<Certificate> repository, TimeProvider time)
    : CrudServiceBase<Certificate, CertificateDto>(repository, time), ICertificateService
{
    protected override CertificateDto ToDto(Certificate entity) => entity.ToDto();
    protected override void Apply(CertificateDto dto, Certificate entity) => dto.Apply(entity);

    protected override IEnumerable<Certificate> Sort(IEnumerable<Certificate> items)
        => items.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.IssueDate);
}

public sealed class LanguageService(IRepository<SpokenLanguage> repository, TimeProvider time)
    : CrudServiceBase<SpokenLanguage, LanguageDto>(repository, time), ILanguageService
{
    protected override LanguageDto ToDto(SpokenLanguage entity) => entity.ToDto();
    protected override void Apply(LanguageDto dto, SpokenLanguage entity) => dto.Apply(entity);

    protected override IEnumerable<SpokenLanguage> Sort(IEnumerable<SpokenLanguage> items)
        => items.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.Level);
}
