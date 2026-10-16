using CvPortal.Application.Common;
using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

/// <summary>
/// Admin panelindeki liste / ekle / düzenle / sil / gizle-göster ekranları için ortak davranış.
/// Her içerik tipi için Index.cshtml ve Form.cshtml görünümleri kullanılır.
/// Dosya yükleme gibi ek işler için BeforeSaveAsync / AfterSaveAsync / AfterDeleteAsync ezilebilir.
/// </summary>
public abstract class CrudAdminController<TDto>(ICrudService<TDto> service) : AdminControllerBase
    where TDto : class, IHasId, new()
{
    /// <summary>Kullanıcıya gösterilecek mesajlardaki içerik adı (ör. "Deneyim").</summary>
    protected abstract string EntityName { get; }

    protected virtual TDto CreateNew() => new();

    /// <summary>Kaydetmeden hemen önce çağrılır. false dönerse form (ModelState hatalarıyla) tekrar gösterilir.</summary>
    protected virtual Task<bool> BeforeSaveAsync(TDto dto, TDto? existing, CancellationToken ct) => Task.FromResult(true);

    /// <summary>Kayıt denemesinden sonra çağrılır (başarılı / başarısız).</summary>
    protected virtual Task AfterSaveAsync(bool succeeded, CancellationToken ct) => Task.CompletedTask;

    /// <summary>Kayıt silindikten sonra çağrılır (ör. ilişkili dosyaları temizlemek için).</summary>
    protected virtual Task AfterDeleteAsync(TDto deleted, CancellationToken ct) => Task.CompletedTask;

    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await service.GetAllAsync(ct: ct));

    [HttpGet]
    public IActionResult Create() => View("Form", CreateNew());

    [HttpPost]
    public async Task<IActionResult> Create(TDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid || !await BeforeSaveAsync(dto, null, ct))
        {
            await AfterSaveAsync(false, ct);
            return View("Form", dto);
        }

        try
        {
            await service.CreateAsync(dto, ct);
        }
        catch
        {
            await AfterSaveAsync(false, ct);
            throw;
        }

        await AfterSaveAsync(true, ct);
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

        var existing = await service.GetByIdAsync(id, ct);
        if (existing is null) return NotFound();

        if (!await BeforeSaveAsync(dto, existing, ct))
        {
            await AfterSaveAsync(false, ct);
            return View("Form", dto);
        }

        bool updated;
        try
        {
            updated = await service.UpdateAsync(id, dto, ct);
        }
        catch
        {
            await AfterSaveAsync(false, ct);
            throw;
        }

        await AfterSaveAsync(updated, ct);
        if (!updated) return NotFound();

        Success($"{EntityName} güncellendi.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var existing = await service.GetByIdAsync(id, ct);
        if (existing is not null && await service.DeleteAsync(id, ct))
        {
            await AfterDeleteAsync(existing, ct);
            Success($"{EntityName} silindi.");
        }
        else
        {
            Error($"{EntityName} bulunamadı.");
        }

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
