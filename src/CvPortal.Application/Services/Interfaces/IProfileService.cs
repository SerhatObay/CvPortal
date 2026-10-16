using CvPortal.Application.DTOs;

namespace CvPortal.Application.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileDto> GetAsync(CancellationToken ct = default);
    Task UpdateAsync(ProfileDto dto, CancellationToken ct = default);

    /// <summary>Profili verilen ad/ünvan/e-posta ile boş bir şablona döndürür (kurulumda "boş başla").</summary>
    Task ResetAsync(string fullName, string title, string email, CancellationToken ct = default);
}
