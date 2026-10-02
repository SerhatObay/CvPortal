using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

public class ExperiencesController(IExperienceService service) : CrudAdminController<ExperienceDto>(service)
{
    protected override string EntityName => "Deneyim";
}

public class EducationsController(IEducationService service) : CrudAdminController<EducationDto>(service)
{
    protected override string EntityName => "Eğitim";
}

public class SkillsController(ISkillService service) : CrudAdminController<SkillDto>(service)
{
    protected override string EntityName => "Yetenek";
}

public class ProjectsController(IProjectService service) : CrudAdminController<ProjectDto>(service)
{
    protected override string EntityName => "Proje";

    [HttpPost]
    public async Task<IActionResult> ToggleFeatured(int id, CancellationToken ct)
    {
        var result = await service.ToggleFeaturedAsync(id, ct);
        if (result is null) Error("Proje bulunamadı.");
        else Success(result.Value ? "Proje öne çıkarıldı." : "Proje öne çıkanlardan kaldırıldı.");

        return RedirectToAction(nameof(Index));
    }
}

public class CertificatesController(ICertificateService service) : CrudAdminController<CertificateDto>(service)
{
    protected override string EntityName => "Sertifika";
}

public class LanguagesController(ILanguageService service) : CrudAdminController<LanguageDto>(service)
{
    protected override string EntityName => "Dil";
}
