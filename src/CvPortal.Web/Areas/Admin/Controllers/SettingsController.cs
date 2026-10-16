using CvPortal.Application.Services;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Web.Areas.Admin.Models;
using CvPortal.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

/// <summary>
/// Hesap, şifre, REST API anahtarı ve içerik sıfırlama ayarları.
/// </summary>
public class SettingsController(
    IAdminAuthService auth,
    ISiteSettingsService siteSettings,
    IContentResetService contentReset) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken ct)
        => await BuildViewAsync(account: null, password: null, ct);

    [HttpPost]
    public async Task<IActionResult> UpdateAccount([Bind(Prefix = nameof(SettingsViewModel.Account))] AccountSettingsInput account, CancellationToken ct)
    {
        var user = await auth.GetCurrentAsync(User, ct);
        if (user is null) return Challenge();
        if (!ModelState.IsValid) return await BuildViewAsync(account, null, ct);

        await auth.UpdateAccountAsync(user.Id, account.Email, account.DisplayName, ct);

        // Üst bardaki ad güncellensin diye oturum yeni bilgilerle yenilenir.
        user.DisplayName = account.DisplayName.Trim();
        user.Email = account.Email.Trim();
        await auth.SignInAsync(HttpContext, user);

        Success("Hesap bilgileri güncellendi.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([Bind(Prefix = nameof(SettingsViewModel.Password))] ChangePasswordInput password, CancellationToken ct)
    {
        var user = await auth.GetCurrentAsync(User, ct);
        if (user is null) return Challenge();
        if (!ModelState.IsValid) return await BuildViewAsync(null, password, ct);

        if (!await auth.ChangePasswordAsync(user.Id, password.CurrentPassword, password.NewPassword, ct))
        {
            ModelState.AddModelError($"{nameof(SettingsViewModel.Password)}.{nameof(ChangePasswordInput.CurrentPassword)}", "Mevcut şifre hatalı.");
            return await BuildViewAsync(null, new ChangePasswordInput(), ct);
        }

        Success("Şifreniz değiştirildi.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleApi(bool enabled, CancellationToken ct)
    {
        await siteSettings.SetApiEnabledAsync(enabled, ct);
        Success(enabled ? "REST API etkinleştirildi." : "REST API devre dışı bırakıldı.");
        return RedirectToAction(nameof(Index), null, "api");
    }

    [HttpPost]
    public async Task<IActionResult> RegenerateApiKey(CancellationToken ct)
    {
        await siteSettings.RegenerateApiKeyAsync(ct);
        Success("Yeni API anahtarı oluşturuldu. Eski anahtar artık çalışmaz.");
        return RedirectToAction(nameof(Index), null, "api");
    }

    [HttpPost]
    public async Task<IActionResult> ClearContent(CancellationToken ct)
    {
        await contentReset.ClearAllContentAsync(ct);
        Success("Tüm CV içeriği ve mesajlar silindi. Profil bilgileriniz korundu.");
        return RedirectToAction("Index", "Dashboard");
    }

    private async Task<IActionResult> BuildViewAsync(AccountSettingsInput? account, ChangePasswordInput? password, CancellationToken ct)
    {
        var user = await auth.GetCurrentAsync(User, ct);
        if (user is null) return Challenge();

        return View(nameof(Index), new SettingsViewModel
        {
            Username = user.Username,
            LastLoginAt = user.LastLoginAt,
            Site = await siteSettings.GetAsync(ct),
            Account = account ?? new AccountSettingsInput { DisplayName = user.DisplayName, Email = user.Email },
            Password = password ?? new ChangePasswordInput()
        });
    }
}
