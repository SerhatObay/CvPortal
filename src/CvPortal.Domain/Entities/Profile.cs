using CvPortal.Domain.Common;

namespace CvPortal.Domain.Entities;

/// <summary>
/// CV sahibinin temel profil bilgileri (tekil kayıt).
/// </summary>
public class Profile : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? CvFileUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? MediumUrl { get; set; }
    public DateOnly CareerStartDate { get; set; }
    public bool IsOpenToWork { get; set; }
}
