using CvPortal.Domain.Common;

namespace CvPortal.Domain.Entities;

/// <summary>
/// Admin paneline giriş yapabilen kullanıcı. Şifre yalnızca hash olarak saklanır.
/// </summary>
public class AdminUser : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
}
