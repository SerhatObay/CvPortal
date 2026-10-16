using System.Security.Cryptography;
using System.Text;
using CvPortal.Application.Abstractions;
using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Entities;

namespace CvPortal.Application.Services;

public sealed class SiteSettingsService(IRepository<SiteSettings> repository, TimeProvider time) : ISiteSettingsService
{
    private const string KeyPrefix = "cvp_";

    public async Task<SiteSettingsDto> GetAsync(CancellationToken ct = default)
        => ToDto(await GetOrCreateAsync(ct));

    public async Task<bool> IsSetupCompletedAsync(CancellationToken ct = default)
        => await repository.AnyAsync(s => s.SetupCompletedAt != null, ct);

    public async Task MarkSetupCompletedAsync(CancellationToken ct = default)
    {
        var settings = await GetOrCreateAsync(ct);
        settings.SetupCompletedAt = time.GetUtcNow().UtcDateTime;
        await SaveAsync(settings, ct);
    }

    public async Task<string> RegenerateApiKeyAsync(CancellationToken ct = default)
    {
        var settings = await GetOrCreateAsync(ct);
        settings.ApiKey = GenerateApiKey();
        await SaveAsync(settings, ct);
        return settings.ApiKey;
    }

    public async Task SetApiEnabledAsync(bool enabled, CancellationToken ct = default)
    {
        var settings = await GetOrCreateAsync(ct);
        settings.IsApiEnabled = enabled;
        await SaveAsync(settings, ct);
    }

    public async Task<ApiKeyValidationResult> ValidateApiKeyAsync(string? providedKey, CancellationToken ct = default)
    {
        var settings = await repository.FirstOrDefaultAsync(_ => true, ct);
        if (settings is null || !settings.IsApiEnabled) return ApiKeyValidationResult.ApiDisabled;
        if (string.IsNullOrWhiteSpace(providedKey)) return ApiKeyValidationResult.Missing;

        var equal = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(providedKey.Trim()), Encoding.UTF8.GetBytes(settings.ApiKey));

        return equal ? ApiKeyValidationResult.Valid : ApiKeyValidationResult.Invalid;
    }

    /// <summary>"cvp_" + 40 karakter, URL güvenli rastgele anahtar.</summary>
    public static string GenerateApiKey()
        => KeyPrefix + Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(20));

    private async Task<SiteSettings> GetOrCreateAsync(CancellationToken ct)
    {
        var settings = await repository.FirstOrDefaultAsync(_ => true, ct);
        if (settings is not null) return settings;

        return await repository.AddAsync(new SiteSettings
        {
            ApiKey = GenerateApiKey(),
            IsApiEnabled = false,
            CreatedAt = time.GetUtcNow().UtcDateTime
        }, ct);
    }

    private Task SaveAsync(SiteSettings settings, CancellationToken ct)
    {
        settings.UpdatedAt = time.GetUtcNow().UtcDateTime;
        return repository.UpdateAsync(settings, ct);
    }

    private static SiteSettingsDto ToDto(SiteSettings s) => new()
    {
        ApiKey = s.ApiKey,
        IsApiEnabled = s.IsApiEnabled,
        SetupCompletedAt = s.SetupCompletedAt
    };
}
