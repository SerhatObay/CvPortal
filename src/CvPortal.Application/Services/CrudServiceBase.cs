using CvPortal.Application.Abstractions;
using CvPortal.Application.Common;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Common;

namespace CvPortal.Application.Services;

/// <summary>
/// Sıralanabilir ve yayınlanabilir içerikler için ortak CRUD davranışı.
/// Alt sınıflar yalnızca eşleme (mapping) ve gerekirse sıralama kuralını belirtir.
/// </summary>
public abstract class CrudServiceBase<TEntity, TDto>(IRepository<TEntity> repository, TimeProvider timeProvider)
    : ICrudService<TDto>
    where TEntity : BaseEntity, IOrderable, IPublishable, new()
    where TDto : class, IHasId
{
    protected IRepository<TEntity> Repository { get; } = repository;
    protected TimeProvider Time { get; } = timeProvider;

    protected abstract TDto ToDto(TEntity entity);
    protected abstract void Apply(TDto dto, TEntity entity);

    protected virtual IEnumerable<TEntity> Sort(IEnumerable<TEntity> items)
        => items.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id);

    public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(bool onlyVisible = false, CancellationToken ct = default)
    {
        var items = await Repository.GetAllAsync(ct);
        var query = onlyVisible ? items.Where(x => x.IsVisible) : items;
        return Sort(query).Select(ToDto).ToList();
    }

    public virtual async Task<TDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public virtual async Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default)
    {
        var entity = new TEntity();
        Apply(dto, entity);
        entity.CreatedAt = Time.GetUtcNow().UtcDateTime;

        // Sıra verilmediyse listenin sonuna ekle.
        if (entity.DisplayOrder <= 0)
        {
            var all = await Repository.GetAllAsync(ct);
            entity.DisplayOrder = all.Count == 0 ? 1 : all.Max(x => x.DisplayOrder) + 1;
        }

        var created = await Repository.AddAsync(entity, ct);
        return ToDto(created);
    }

    public virtual async Task<bool> UpdateAsync(int id, TDto dto, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return false;

        Apply(dto, entity);
        entity.UpdatedAt = Time.GetUtcNow().UtcDateTime;
        return await Repository.UpdateAsync(entity, ct);
    }

    public virtual Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => Repository.DeleteAsync(id, ct);

    public virtual async Task<bool?> ToggleVisibilityAsync(int id, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        if (entity is null) return null;

        entity.IsVisible = !entity.IsVisible;
        entity.UpdatedAt = Time.GetUtcNow().UtcDateTime;
        await Repository.UpdateAsync(entity, ct);
        return entity.IsVisible;
    }
}
