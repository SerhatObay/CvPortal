using CvPortal.Domain.Common;
using CvPortal.Domain.Enums;

namespace CvPortal.Domain.Entities;

public class SpokenLanguage : BaseEntity, IOrderable, IPublishable
{
    public string Name { get; set; } = string.Empty;
    public LanguageLevel Level { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
