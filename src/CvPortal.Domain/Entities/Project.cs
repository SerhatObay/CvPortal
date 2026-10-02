using CvPortal.Domain.Common;

namespace CvPortal.Domain.Entities;

public class Project : BaseEntity, IOrderable, IPublishable
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LiveUrl { get; set; }
    public List<string> Technologies { get; set; } = [];
    public bool IsFeatured { get; set; }
    public DateOnly? CompletedAt { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
