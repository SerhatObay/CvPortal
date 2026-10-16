using CvPortal.Application.Services;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Entities;
using CvPortal.Web.Security;

namespace CvPortal.Web.Setup;

public interface ISetupService
{
    /// <summary>Kurulum daha önce tamamlandıysa veya zaten bir yönetici varsa true.</summary>
    Task<bool> IsCompletedAsync(CancellationToken ct = default);

    /// <summary>İlk yöneticiyi oluşturur, profili ve ayarları hazırlar. Kurulum zaten yapıldıysa null döner.</summary>
    Task<AdminUser?> CompleteAsync(SetupViewModel model, CancellationToken ct = default);
}

public sealed class SetupService(
    IAdminAuthService auth,
    ISiteSettingsService settings,
    IProfileService profiles,
    IContentResetService contentReset,
    ILogger<SetupService> logger) : ISetupService
{
    public async Task<bool> IsCompletedAsync(CancellationToken ct = default)
        => await settings.IsSetupCompletedAsync(ct) || await auth.HasAnyAdminAsync(ct);

    public async Task<AdminUser?> CompleteAsync(SetupViewModel model, CancellationToken ct = default)
    {
        if (await IsCompletedAsync(ct)) return null;

        var admin = await auth.CreateAdminAsync(model.Username, model.Email, model.FullName, model.Password, ct);

        if (model.StartEmpty)
        {
            await contentReset.ClearAllContentAsync(ct);
            await profiles.ResetAsync(model.FullName, model.Title, model.Email, ct);
        }
        else
        {
            // Örnek içerik korunur ama kimlik bilgileri kurulumu yapan kişiye ait olur.
            var profile = await profiles.GetAsync(ct);
            profile.FullName = model.FullName.Trim();
            profile.Title = model.Title.Trim();
            profile.Email = model.Email.Trim();
            await profiles.UpdateAsync(profile, ct);
        }

        await settings.SetApiEnabledAsync(model.EnableApi, ct);
        await settings.MarkSetupCompletedAsync(ct);
        SetupGuardMiddleware.MarkCompleted();

        logger.LogInformation("Kurulum tamamlandı. Yönetici: {Username}, boş başlangıç: {StartEmpty}", admin.Username, model.StartEmpty);
        return admin;
    }
}
