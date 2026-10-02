using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Web.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Controllers.Api;

[ApiController]
[ApiKeyAuthorize]
[EnableCors("ExternalPanels")]
[Route("api/v1/profile"), Tags("Profil")]
[Produces("application/json")]
public class ProfileApiController(IProfileService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProfileDto>> Get(CancellationToken ct) => Ok(await service.GetAsync(ct));

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(ProfileDto dto, CancellationToken ct)
    {
        await service.UpdateAsync(dto, ct);
        return NoContent();
    }
}

/// <summary>
/// Herkese açık CV verisi — başka bir frontend (React, mobil vb.) bu uç noktayı kullanarak CV'yi gösterebilir.
/// </summary>
[ApiController]
[EnableCors("ExternalPanels")]
[Route("api/v1/cv"), Tags("Herkese Açık CV")]
[Produces("application/json")]
public class CvApiController(ICvService service) : ControllerBase
{
    [HttpGet]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<CvDto>> Get(CancellationToken ct) => Ok(await service.GetCvAsync(ct));
}

[ApiController]
[ApiKeyAuthorize]
[EnableCors("ExternalPanels")]
[Route("api/v1/dashboard"), Tags("Dashboard")]
[Produces("application/json")]
public class DashboardApiController(IDashboardService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get(CancellationToken ct) => Ok(await service.GetSummaryAsync(ct));
}
