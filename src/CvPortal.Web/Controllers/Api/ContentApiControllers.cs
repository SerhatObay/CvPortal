using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Controllers.Api;

[Route("api/v1/experiences"), Tags("Deneyimler")]
public class ExperiencesApiController(IExperienceService service) : CrudApiController<ExperienceDto>(service);

[Route("api/v1/educations"), Tags("Eğitim")]
public class EducationsApiController(IEducationService service) : CrudApiController<EducationDto>(service);

[Route("api/v1/skills"), Tags("Yetenekler")]
public class SkillsApiController(ISkillService service) : CrudApiController<SkillDto>(service)
{
    /// <summary>Yetenekleri kategoriye göre gruplar.</summary>
    [HttpGet("grouped")]
    public async Task<ActionResult<IReadOnlyList<SkillGroupDto>>> GetGrouped([FromQuery] bool onlyVisible = true, CancellationToken ct = default)
        => Ok(await service.GetGroupedAsync(onlyVisible, ct));
}

[Route("api/v1/projects"), Tags("Projeler")]
public class ProjectsApiController(IProjectService service) : CrudApiController<ProjectDto>(service)
{
    /// <summary>Projenin "öne çıkan" durumunu değiştirir.</summary>
    [HttpPatch("{id:int}/featured")]
    public async Task<ActionResult<object>> ToggleFeatured(int id, CancellationToken ct)
        => await service.ToggleFeaturedAsync(id, ct) is { } isFeatured ? Ok(new { id, isFeatured }) : NotFound();
}

[Route("api/v1/certificates"), Tags("Sertifikalar")]
public class CertificatesApiController(ICertificateService service) : CrudApiController<CertificateDto>(service);

[Route("api/v1/languages"), Tags("Diller")]
public class LanguagesApiController(ILanguageService service) : CrudApiController<LanguageDto>(service);
