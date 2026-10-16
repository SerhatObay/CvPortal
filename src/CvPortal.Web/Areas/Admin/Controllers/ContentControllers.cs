using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Web.Uploads;
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

[RequestSizeLimit(MediaUploader.MaxImageBytes + 1024 * 1024)]
public class ProjectsController(IProjectService service, MediaChangeSet media, MediaUploader uploader)
    : CrudAdminController<ProjectDto>(service)
{
    protected override string EntityName => "Proje";

    /// <summary>Formdaki "imageFile" alanından proje görselini yükler veya "removeImage" ile kaldırır.</summary>
    protected override async Task<bool> BeforeSaveAsync(ProjectDto dto, ProjectDto? existing, CancellationToken ct)
    {
        var form = Request.HasFormContentType ? Request.Form : null;
        var file = form?.Files.GetFile("imageFile");
        var remove = form is not null && form["removeImage"].ToString() == "true";

        var result = await media.ApplyAsync(file, remove, existing?.ImageUrl, dto.ImageUrl, UploadKind.Image, "projects", ct);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(ProjectDto.ImageUrl), result.Error!);
            return false;
        }

        dto.ImageUrl = result.Url;
        return true;
    }

    protected override Task AfterSaveAsync(bool succeeded, CancellationToken ct)
        => succeeded ? media.CommitAsync(ct) : media.RollbackAsync(ct);

    protected override Task AfterDeleteAsync(ProjectDto deleted, CancellationToken ct)
        => uploader.DeleteAsync(deleted.ImageUrl, ct);

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
