using CvPortal.Domain.Common;

namespace CvPortal.Domain.Entities;

/// <summary>
/// Ziyaretçilerin iletişim formundan gönderdiği mesajlar.
/// </summary>
public class ContactMessage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
