using CvPortal.Domain.Common;

namespace CvPortal.Domain.Entities;

public class Education : BaseEntity, IOrderable, IPublishable
{
    public string School { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Grade { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
