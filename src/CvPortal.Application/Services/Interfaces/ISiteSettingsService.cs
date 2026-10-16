using CvPortal.Application.DTOs;

namespace CvPortal.Application.Services.Interfaces;

public interface ISiteSettingsService
{
    /// <summary>Ayarları döner; kayıt yoksa (ilk açılış) API kapalı ve yeni bir anahtarla oluşturur.</summary>
    Task<SiteSettingsDto> GetAsync(CancellationToken ct = default);

    Task<bool> IsSetupCompletedAsync(CancellationToken ct = default);
    Task MarkSetupCompletedAsync(CancellationToken ct = default);

    /// <summary>Yeni bir API anahtarı üretir; eski anahtar hemen geçersiz olur.</summary>
    Task<string> RegenerateApiKeyAsync(CancellationToken ct = default);

    Task SetApiEnabledAsync(bool enabled, CancellationToken ct = default);

    /// <summary>API açık ve anahtar doğruysa true (sabit zamanlı karşılaştırma).</summary>
    Task<ApiKeyValidationResult> ValidateApiKeyAsync(string? providedKey, CancellationToken ct = default);
}

public enum ApiKeyValidationResult
{
    Valid,
    Missing,
    Invalid,
    ApiDisabled
}
