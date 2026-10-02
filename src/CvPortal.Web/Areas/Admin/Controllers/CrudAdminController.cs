using CvPortal.Application.Common;
using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

/// <summary>
/// Admin panelindeki liste / ekle / düzenle / sil / gizle-göster ekranları için ortak davranış.
/// Her içerik tipi için Index.cshtml ve Form.cshtml görünümleri kullanılır.
/// </summary>
public abstract class CrudAdminController<TDto>(ICrudService<TDto> service) : AdminControllerBase
    where TDto : class, IHasId, new()
{
    /// <summary>Kullanıcıya gösterilecek mesajlardaki içerik adı (ör. "Deneyim").</summary>
    protected abstract string EntityName { get; }

    protected virtual TDto CreateNew() => new();

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await service.GetAllAsync(ct: ct));

    [HttpGet]
    public IActionResult Create() => View("Form", CreateNew());

    [HttpPost]
    public async Task<IActionResult> Create(TDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View("Form", dto);

        await service.CreateAsync(dto, ct);
        Success($"{EntityName} başarıyla eklendi.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
        => await service.GetByIdAsync(id, ct) is { } dto ? View("Form", dto) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Edit(int id, TDto dto, CancellationToken ct)
    {
        dto.Id = id;
        if (!ModelState.IsValid) return View("Form", dto);

        if (!await service.UpdateAsync(id, dto, ct)) return NotFound();

        Success($"{EntityName} güncellendi.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (await service.DeleteAsync(id, ct)) Success($"{EntityName} silindi.");
        else Error($"{EntityName} bulunamadı.");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleVisibility(int id, CancellationToken ct)
    {
        var result = await service.ToggleVisibilityAsync(id, ct);
        if (result is null) Error($"{EntityName} bulunamadı.");
        else Success(result.Value ? $"{EntityName} artık sitede görünüyor." : $"{EntityName} siteden gizlendi.");

        return RedirectToAction(nameof(Index));
    }
}
