using CvPortal.Web.Security;
using CvPortal.Web.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CvPortal.Web.Controllers;

/// <summary>
/// İlk kurulum sihirbazı (/setup). Yalnızca henüz yönetici hesabı yokken erişilebilir.
/// </summary>
[Route("setup")]
public class SetupController(ISetupService setupService, IAdminAuthService auth) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (await setupService.IsCompletedAsync(ct)) return RedirectToAction("Login", "Account", new { area = "Admin" });
        return View(new SetupViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Index(SetupViewModel model, CancellationToken ct)
    {
        if (await setupService.IsCompletedAsync(ct)) return RedirectToAction("Login", "Account", new { area = "Admin" });
        if (!ModelState.IsValid) return View(model);

        var admin = await setupService.CompleteAsync(model, ct);
        if (admin is null) return RedirectToAction("Login", "Account", new { area = "Admin" });

        await auth.SignInAsync(HttpContext, admin);
        TempData["Success"] = "Kurulum tamamlandı, hoş geldiniz! Profilinizi ve CV içeriğinizi düzenleyerek başlayabilirsiniz.";
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
}
