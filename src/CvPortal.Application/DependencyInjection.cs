using CvPortal.Application.Services;
using CvPortal.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CvPortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IExperienceService, ExperienceService>();
        services.AddScoped<IEducationService, EducationService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IContactMessageService, ContactMessageService>();
        services.AddScoped<ICvService, CvService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();
        services.AddScoped<IContentResetService, ContentResetService>();

        return services;
    }
}
