using CvPortal.Application.DTOs;

namespace CvPortal.Application.Services.Interfaces;

/// <summary>
/// Herkese açık CV içeriğini (yalnızca yayındaki kayıtlar) bir araya getirir.
/// </summary>
public interface ICvService
{
    Task<CvDto> GetCvAsync(CancellationToken ct = default);
}

/// <summary>
/// Admin panel özet ekranı için istatistikler.
/// </summary>
public interface IDashboardService
{
    Task<DashboardDto> GetSummaryAsync(CancellationToken ct = default);
}
