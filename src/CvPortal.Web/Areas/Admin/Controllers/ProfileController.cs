using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Web.Uploads;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

public class ProfileController(IProfileService profileService, MediaChangeSet media) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await profileService.GetAsync(ct));

    [HttpPost]
    [RequestSizeLimit(MediaUploader.MaxImageBytes + MediaUploader.MaxPdfBytes + 1024 * 1024)]
    public async Task<IActionResult> Index(ProfileDto dto, IFormFile? avatarFile, IFormFile? cvFile,
        bool removeAvatar, bool removeCv, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);

        var current = await profileService.GetAsync(ct);

        var avatar = await media.ApplyAsync(avatarFile, removeAvatar, current.AvatarUrl, dto.AvatarUrl, UploadKind.Image, "avatar", ct);
        var cv = await media.ApplyAsync(cvFile, removeCv, current.CvFileUrl, dto.CvFileUrl, UploadKind.Pdf, "cv", ct);

        if (!avatar.Succeeded || !cv.Succeeded)
        {
            if (!avatar.Succeeded) ModelState.AddModelError(nameof(ProfileDto.AvatarUrl), avatar.Error!);
            if (!cv.Succeeded) ModelState.AddModelError(nameof(ProfileDto.CvFileUrl), cv.Error!);
            await media.RollbackAsync(ct);
            return View(dto);
        }

        dto.AvatarUrl = avatar.Url;
        dto.CvFileUrl = cv.Url;

        try
        {
            await profileService.UpdateAsync(dto, ct);
        }
        catch
        {
            await media.RollbackAsync(ct);
            throw;
        }

        await media.CommitAsync(ct);
        Success("Profil bilgileri güncellendi.");
        return RedirectToAction(nameof(Index));
    }
}
