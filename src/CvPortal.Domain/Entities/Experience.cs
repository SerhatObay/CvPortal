using CvPortal.Domain.Common;
using CvPortal.Domain.Enums;

namespace CvPortal.Domain.Entities;

public class Experience : BaseEntity, IOrderable, IPublishable
{
    public string Company { get; set; } = string.Empty;
    public string? CompanyUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    public WorkMode WorkMode { get; set; } = WorkMode.Hybrid;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Achievements { get; set; } = [];
    public List<string> Technologies { get; set; } = [];
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;

    public bool IsCurrent => EndDate is null;
}
