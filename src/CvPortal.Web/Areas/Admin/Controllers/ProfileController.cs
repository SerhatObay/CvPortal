using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

public class ProfileController(IProfileService profileService) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await profileService.GetAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Index(ProfileDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);

        await profileService.UpdateAsync(dto, ct);
        Success("Profil bilgileri güncellendi.");
        return RedirectToAction(nameof(Index));
    }
}
