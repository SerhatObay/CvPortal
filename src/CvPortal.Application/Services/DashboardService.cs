using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;

namespace CvPortal.Application.Services;

public sealed class DashboardService(
    IProfileService profileService,
    IExperienceService experienceService,
    IEducationService educationService,
    ISkillService skillService,
    IProjectService projectService,
    ICertificateService certificateService,
    ILanguageService languageService,
    IContactMessageService messageService) : IDashboardService
{
    public async Task<DashboardDto> GetSummaryAsync(CancellationToken ct = default)
    {
        var profile = await profileService.GetAsync(ct);
        var experiences = await experienceService.GetAllAsync(ct: ct);
        var educations = await educationService.GetAllAsync(ct: ct);
        var skills = await skillService.GetAllAsync(ct: ct);
        var projects = await projectService.GetAllAsync(ct: ct);
        var certificates = await certificateService.GetAllAsync(ct: ct);
        var languages = await languageService.GetAllAsync(ct: ct);
        var messages = await messageService.GetAllAsync(ct: ct);

        var hidden = experiences.Count(x => !x.IsVisible) + educations.Count(x => !x.IsVisible)
                   + skills.Count(x => !x.IsVisible) + projects.Count(x => !x.IsVisible)
                   + certificates.Count(x => !x.IsVisible) + languages.Count(x => !x.IsVisible);

        var (completion, missing) = CalculateProfileCompletion(profile);

        return new DashboardDto
        {
            ExperienceCount = experiences.Count,
            EducationCount = educations.Count,
            SkillCount = skills.Count,
            ProjectCount = projects.Count,
            CertificateCount = certificates.Count,
            LanguageCount = languages.Count,
            MessageCount = messages.Count,
            UnreadMessageCount = messages.Count(m => !m.IsRead),
            HiddenItemCount = hidden,
            ProfileCompletion = completion,
            MissingProfileFields = missing,
            RecentMessages = messages.Take(5).ToList(),
            SkillsByCategory = skills
                .GroupBy(s => s.CategoryName)
                .Select(g => new CategoryCountDto(g.Key, g.Count()))
                .OrderByDescending(x => x.Count)
                .ToList(),
            TopTechnologies = experiences.SelectMany(e => e.Technologies)
                .Concat(projects.SelectMany(p => p.Technologies))
                .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
                .Select(g => new CategoryCountDto(g.Key, g.Count()))
                .OrderByDescending(x => x.Count).ThenBy(x => x.Name)
                .Take(8)
                .ToList()
        };
    }

    private static (int Percent, List<string> Missing) CalculateProfileCompletion(ProfileDto p)
    {
        var checks = new (string Label, bool Ok)[]
        {
            ("Ad Soyad", !string.IsNullOrWhiteSpace(p.FullName)),
            ("Ünvan", !string.IsNullOrWhiteSpace(p.Title)),
            ("Kısa tanıtım", !string.IsNullOrWhiteSpace(p.Headline)),
            ("Hakkımda (en az 150 karakter)", p.About.Length >= 150),
            ("E-posta", !string.IsNullOrWhiteSpace(p.Email)),
            ("Telefon", !string.IsNullOrWhiteSpace(p.Phone)),
            ("Profil fotoğrafı", !string.IsNullOrWhiteSpace(p.AvatarUrl)),
            ("CV dosyası", !string.IsNullOrWhiteSpace(p.CvFileUrl)),
            ("GitHub", !string.IsNullOrWhiteSpace(p.GitHubUrl)),
            ("LinkedIn", !string.IsNullOrWhiteSpace(p.LinkedInUrl))
        };

        var percent = (int)Math.Round(checks.Count(c => c.Ok) * 100d / checks.Length);
        return (percent, checks.Where(c => !c.Ok).Select(c => c.Label).ToList());
    }
}
