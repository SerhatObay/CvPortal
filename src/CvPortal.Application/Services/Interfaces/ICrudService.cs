using CvPortal.Application.Common;

namespace CvPortal.Application.Services.Interfaces;

/// <summary>
/// Admin panel ve API tarafından kullanılan ortak CRUD sözleşmesi.
/// </summary>
public interface ICrudService<TDto> where TDto : class, IHasId
{
    Task<IReadOnlyList<TDto>> GetAllAsync(bool onlyVisible = false, CancellationToken ct = default);
    Task<TDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, TDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>Kaydın sitede görünürlüğünü tersine çevirir. Kayıt yoksa null döner.</summary>
    Task<bool?> ToggleVisibilityAsync(int id, CancellationToken ct = default);
}
