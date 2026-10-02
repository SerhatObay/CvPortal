using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;
using CvPortal.Domain.Enums;

namespace CvPortal.Application.DTOs;

public class SkillDto : IHasId
{
    public int Id { get; set; }

    [Display(Name = "Yetenek")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(50, ErrorMessage = ValidationMessages.MaxLength)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Kategori")]
    public SkillCategory Category { get; set; } = SkillCategory.Backend;

    [Display(Name = "Seviye (%)")]
    [Range(0, 100, ErrorMessage = ValidationMessages.Range)]
    public int Proficiency { get; set; } = 70;

    [Display(Name = "İkon Sınıfı")]
    [StringLength(80, ErrorMessage = ValidationMessages.MaxLength)]
    public string? IconClass { get; set; }

    [Display(Name = "Sıra")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Sitede göster")]
    public bool IsVisible { get; set; } = true;

    public string CategoryName => Category.GetDisplayName();

    public string LevelName => Proficiency switch
    {
        >= 90 => "Uzman",
        >= 75 => "İleri",
        >= 50 => "Orta",
        _ => "Başlangıç"
    };
}
