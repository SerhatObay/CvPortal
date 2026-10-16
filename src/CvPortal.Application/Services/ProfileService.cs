using CvPortal.Application.Abstractions;
using CvPortal.Application.DTOs;
using CvPortal.Application.Mapping;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Entities;

namespace CvPortal.Application.Services;

public sealed class ProfileService(IRepository<Profile> repository, TimeProvider time) : IProfileService
{
    public async Task<ProfileDto> GetAsync(CancellationToken ct = default)
        => (await GetOrCreateAsync(ct)).ToDto();

    public async Task UpdateAsync(ProfileDto dto, CancellationToken ct = default)
    {
        var profile = await GetOrCreateAsync(ct);
        dto.Apply(profile);
        profile.UpdatedAt = time.GetUtcNow().UtcDateTime;
        await repository.UpdateAsync(profile, ct);
    }

    public async Task ResetAsync(string fullName, string title, string email, CancellationToken ct = default)
    {
        var profile = await GetOrCreateAsync(ct);
        var blank = CreateBlank();

        // Yeni bir nesne yerine mevcut kaydın alanları sıfırlanır (aynı Id korunur).
        new ProfileDto
        {
            FullName = fullName,
            Title = title,
            Email = email,
            Headline = $"Merhaba, ben {fullName}. {title}.",
            About = "Kendinizi anlatan birkaç paragrafı admin panel → Profil ekranından ekleyin.",
            Location = blank.Location,
            AvatarUrl = "/images/avatar.svg",
            CareerStartDate = blank.CareerStartDate,
            IsOpenToWork = true
        }.Apply(profile);

        profile.UpdatedAt = time.GetUtcNow().UtcDateTime;
        await repository.UpdateAsync(profile, ct);
    }

    /// <summary>
    /// Profil tekil bir kayıttır; yoksa boş bir profil oluşturulur.
    /// </summary>
    private async Task<Profile> GetOrCreateAsync(CancellationToken ct)
        => await repository.FirstOrDefaultAsync(_ => true, ct)
           ?? await repository.AddAsync(CreateBlank(), ct);

    private Profile CreateBlank() => new()
    {
        FullName = "Ad Soyad",
        Title = "Yazılım Mühendisi",
        Location = "Türkiye",
        CareerStartDate = DateOnly.FromDateTime(time.GetLocalNow().DateTime),
        CreatedAt = time.GetUtcNow().UtcDateTime
    };
}
