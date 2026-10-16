using System.Linq.Expressions;
using CvPortal.Application.Abstractions;
using CvPortal.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CvPortal.Infrastructure.Persistence;

/// <summary>
/// SQL Server (EF Core) tabanlı repository. Her işlem kendi SaveChanges çağrısıyla kalıcı hâle gelir.
/// Servisler entity'yi okuyup değiştirdikten sonra UpdateAsync çağırır; entity zaten izlendiği için değişiklikler kaydedilir.
/// </summary>
public class EfRepository<T>(CvPortalDbContext db) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _set = db.Set<T>();

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.ToListAsync(ct);

    public Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => _set.FirstOrDefaultAsync(x => x.Id == id, ct);

    // Tekil kayıtlarda (profil, ayarlar) sonucun her zaman aynı olması için Id'ye göre sıralanır.
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => _set.OrderBy(x => x.Id).FirstOrDefaultAsync(predicate, ct);

    public Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null ? _set.AnyAsync(ct) : _set.AnyAsync(predicate, ct);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null ? _set.CountAsync(ct) : _set.CountAsync(predicate, ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        _set.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (db.Entry(entity).State == EntityState.Detached)
        {
            if (!await _set.AnyAsync(x => x.Id == entity.Id, ct)) return false;
            _set.Update(entity);
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => await _set.Where(x => x.Id == id).ExecuteDeleteAsync(ct) > 0;

    public Task<int> DeleteAllAsync(CancellationToken ct = default)
        => _set.ExecuteDeleteAsync(ct);
}
