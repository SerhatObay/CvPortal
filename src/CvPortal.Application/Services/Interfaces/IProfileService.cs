using CvPortal.Application.DTOs;

namespace CvPortal.Application.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileDto> GetAsync(CancellationToken ct = default);
    Task UpdateAsync(ProfileDto dto, CancellationToken ct = default);
}
