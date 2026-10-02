using CvPortal.Domain.Common;

namespace CvPortal.Domain.Entities;

public class Certificate : BaseEntity, IOrderable, IPublishable
{
    public string Name { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public string? CredentialId { get; set; }
    public string? CredentialUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
