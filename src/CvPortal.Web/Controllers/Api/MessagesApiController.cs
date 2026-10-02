using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Web.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CvPortal.Web.Controllers.Api;

[ApiController]
[ApiKeyAuthorize]
[EnableCors("ExternalPanels")]
[Route("api/v1/messages"), Tags("Mesajlar")]
[Produces("application/json")]
public class MessagesApiController(IContactMessageService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContactMessageDto>>> GetAll([FromQuery] bool onlyUnread = false, CancellationToken ct = default)
        => Ok(await service.GetAllAsync(onlyUnread, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactMessageDto>> GetById(int id, CancellationToken ct)
        => await service.GetByIdAsync(id, ct) is { } dto ? Ok(dto) : NotFound();

    /// <summary>Ziyaretçi iletişim formu (herkese açık, API anahtarı gerekmez).</summary>
    [HttpPost]
    [AllowAnonymousApi]
    [EnableRateLimiting("contact")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContactMessageDto>> Create(CreateContactMessageDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:int}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetRead(int id, [FromQuery] bool isRead = true, CancellationToken ct = default)
        => await service.SetReadStatusAsync(id, isRead, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
