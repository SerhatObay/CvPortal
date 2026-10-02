using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;
using CvPortal.Domain.Enums;

namespace CvPortal.Application.DTOs;

public class LanguageDto : IHasId
{
    public int Id { get; set; }

    [Display(Name = "Dil")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(50, ErrorMessage = ValidationMessages.MaxLength)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Seviye")]
    public LanguageLevel Level { get; set; } = LanguageLevel.B2;

    [Display(Name = "Sıra")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Sitede göster")]
    public bool IsVisible { get; set; } = true;

    public string LevelName => Level.GetDisplayName();

    /// <summary>Görsel gösterim için 0-100 arası yüzde.</summary>
    public int LevelPercent => (int)Math.Round((int)Level / 7d * 100);
}
