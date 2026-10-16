using System.Linq.Expressions;
using CvPortal.Domain.Common;

namespace CvPortal.Application.Abstractions;

/// <summary>
/// Veri erişim soyutlaması. İki implementasyon vardır:
/// SQL Server (EF Core) ve bellek içi (demo / test). Hangisinin kullanılacağı appsettings → Database:Provider ile seçilir.
/// Filtreler Expression olarak alınır; böylece EF Core bunları SQL'e çevirebilir.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(T entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>Tüm kayıtları siler (kurulumda demo içeriği temizlemek için).</summary>
    Task<int> DeleteAllAsync(CancellationToken ct = default);
}
