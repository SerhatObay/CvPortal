using CvPortal.Domain.Common;
using CvPortal.Domain.Enums;

namespace CvPortal.Domain.Entities;

public class Skill : BaseEntity, IOrderable, IPublishable
{
    public string Name { get; set; } = string.Empty;
    public SkillCategory Category { get; set; }
    /// <summary>0-100 arası yetkinlik seviyesi.</summary>
    public int Proficiency { get; set; }
    /// <summary>Bootstrap Icons / Devicon sınıf adı (opsiyonel).</summary>
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
