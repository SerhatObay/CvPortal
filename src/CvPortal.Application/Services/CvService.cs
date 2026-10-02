using CvPortal.Application.Common;
using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Enums;

namespace CvPortal.Application.Services;

public sealed class CvService(
    IProfileService profileService,
    IExperienceService experienceService,
    IEducationService educationService,
    ISkillService skillService,
    IProjectService projectService,
    ICertificateService certificateService,
    ILanguageService languageService,
    TimeProvider time) : ICvService
{
    public async Task<CvDto> GetCvAsync(CancellationToken ct = default)
    {
        var profile = await profileService.GetAsync(ct);
        var experiences = await experienceService.GetAllAsync(onlyVisible: true, ct);
        var educations = await educationService.GetAllAsync(onlyVisible: true, ct);
        var skills = await skillService.GetAllAsync(onlyVisible: true, ct);
        var skillGroups = await skillService.GetGroupedAsync(onlyVisible: true, ct);
        var projects = await projectService.GetAllAsync(onlyVisible: true, ct);
        var certificates = await certificateService.GetAllAsync(onlyVisible: true, ct);
        var languages = await languageService.GetAllAsync(onlyVisible: true, ct);

        var today = DateOnly.FromDateTime(time.GetLocalNow().DateTime);

        var technologies = skills.Where(s => s.Category != SkillCategory.SoftSkill).Select(s => s.Name)
            .Concat(experiences.SelectMany(e => e.Technologies))
            .Concat(projects.SelectMany(p => p.Technologies))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        var stats = new CvStatsDto(
            YearsOfExperience: DateRangeFormatter.TotalMonths(profile.CareerStartDate, today) / 12,
            CompanyCount: experiences.Select(e => e.Company).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            ProjectCount: projects.Count,
            TechnologyCount: technologies,
            CertificateCount: certificates.Count);

        return new CvDto
        {
            Profile = profile,
            Stats = stats,
            Experiences = experiences,
            Educations = educations,
            SkillGroups = skillGroups,
            Projects = projects,
            Certificates = certificates,
            Languages = languages
        };
    }
}
