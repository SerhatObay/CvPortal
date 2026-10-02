using CvPortal.Application.Common;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Web.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Controllers.Api;

/// <summary>
/// Harici paneller için ortak REST CRUD uç noktaları.
/// Türetilen controller yalnızca route ve servis tipini belirtir.
/// </summary>
[ApiController]
[ApiKeyAuthorize]
[EnableCors("ExternalPanels")]
[Produces("application/json")]
public abstract class CrudApiController<TDto>(ICrudService<TDto> service) : ControllerBase
    where TDto : class, IHasId
{
    /// <summary>Tüm kayıtları (gizliler dahil) listeler.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TDto>>> GetAll([FromQuery] bool onlyVisible = false, CancellationToken ct = default)
        => Ok(await service.GetAllAsync(onlyVisible, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TDto>> GetById(int id, CancellationToken ct)
        => await service.GetByIdAsync(id, ct) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TDto>> Create(TDto dto, CancellationToken ct)
    {
        var created = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, TDto dto, CancellationToken ct)
        => await service.UpdateAsync(id, dto, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();

    /// <summary>Kaydın sitede görünürlüğünü değiştirir.</summary>
    [HttpPatch("{id:int}/visibility")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> ToggleVisibility(int id, CancellationToken ct)
        => await service.ToggleVisibilityAsync(id, ct) is { } isVisible ? Ok(new { id, isVisible }) : NotFound();
}
