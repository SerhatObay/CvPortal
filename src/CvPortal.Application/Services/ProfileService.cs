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

    /// <summary>
    /// Profil tekil bir kayıttır; yoksa boş bir profil oluşturulur.
    /// </summary>
    private async Task<Profile> GetOrCreateAsync(CancellationToken ct)
    {
        var profile = (await repository.GetAllAsync(ct)).FirstOrDefault();
        if (profile is not null) return profile;

        return await repository.AddAsync(new Profile
        {
            FullName = "Ad Soyad",
            Title = "Yazılım Mühendisi",
            CareerStartDate = DateOnly.FromDateTime(time.GetLocalNow().DateTime)
        }, ct);
    }
}
