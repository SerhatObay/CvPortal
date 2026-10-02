using CvPortal.Domain.Common;

namespace CvPortal.Application.Abstractions;

/// <summary>
/// Veri erişim soyutlaması. Şu an mock (in-memory) implementasyon kullanılıyor;
/// ileride EF Core / Dapper implementasyonu ile değiştirilebilir.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(T entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<int> CountAsync(Func<T, bool>? predicate = null, CancellationToken ct = default);
}
