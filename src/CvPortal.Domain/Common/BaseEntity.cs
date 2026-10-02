namespace CvPortal.Domain.Common;

/// <summary>
/// Tüm entity'lerin ortak alanları.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Sıralanabilir (admin panelden sırası değiştirilebilir) entity'ler için.
/// </summary>
public interface IOrderable
{
    int DisplayOrder { get; set; }
}

/// <summary>
/// Yayında olup olmadığı yönetilebilen entity'ler için.
/// </summary>
public interface IPublishable
{
    bool IsVisible { get; set; }
}
