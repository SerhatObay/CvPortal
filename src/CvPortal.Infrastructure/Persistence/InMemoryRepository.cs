using System.Linq.Expressions;
using CvPortal.Application.Abstractions;
using CvPortal.Domain.Common;

namespace CvPortal.Infrastructure.Persistence;

/// <summary>
/// Thread-safe, bellek içi repository. Veritabanı olmadan demo / test için kullanılır
/// (appsettings → Database:Provider = "InMemory"). Uygulama yeniden başlatıldığında veriler mock data'ya döner.
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

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_store.Values.FirstOrDefault(predicate.Compile()));
        }
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(predicate is null ? _store.Count > 0 : _store.Values.Any(predicate.Compile()));
        }
    }

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        lock (_lock)
        {
            return Task.FromResult(predicate is null ? _store.Count : _store.Values.Count(predicate.Compile()));
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

    public Task<int> DeleteAllAsync(CancellationToken ct = default)
    {
        lock (_lock)
        {
            var count = _store.Count;
            _store.Clear();
            return Task.FromResult(count);
        }
    }
}
