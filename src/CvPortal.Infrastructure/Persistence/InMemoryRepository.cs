using CvPortal.Application.Abstractions;
using CvPortal.Domain.Common;

namespace CvPortal.Infrastructure.Persistence;

/// <summary>
/// Thread-safe, bellek içi repository. Uygulama yeniden başlatıldığında veriler mock data'ya döner.
/// Gerçek veritabanına geçerken yalnızca bu sınıfın yerine EF Core tabanlı bir implementasyon yazmak yeterlidir.
/// </summary>
public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly Dictionary<int, T> _store = [];
    private readonly Lock _lock = new();
    private int _lastId;

    public InMemoryRepository(IEnumerable<T>? seed = null)
    {
        foreach (var item in seed ?? [])
        {
            if (item.Id <= 0) item.Id = ++_lastId;
            _lastId = Math.Max(_lastId, item.Id);
            _store[item.Id] = item;
        }
    }

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IReadOnlyList<T>>(_store.Values.ToList());
        }
    }

    public Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_store.GetValueOrDefault(id));
        }
    }

    public Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        lock (_lock)
        {
            entity.Id = ++_lastId;
            _store[entity.Id] = entity;
            return Task.FromResult(entity);
        }
    }

    public Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (!_store.ContainsKey(entity.Id)) return Task.FromResult(false);
            _store[entity.Id] = entity;
            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_store.Remove(id));
        }
    }

    public Task<int> CountAsync(Func<T, bool>? predicate = null, CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(predicate is null ? _store.Count : _store.Values.Count(predicate));
        }
    }
}
