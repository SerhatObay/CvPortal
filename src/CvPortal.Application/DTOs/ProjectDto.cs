using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CvPortal.Application.Common;

namespace CvPortal.Application.DTOs;

public class ProjectDto : IHasId
{
    public int Id { get; set; }

    [Display(Name = "Proje Adı")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Kısa Özet")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(300, ErrorMessage = ValidationMessages.MaxLength)]
    public string Summary { get; set; } = string.Empty;

    [Display(Name = "Detaylı Açıklama")]
    [StringLength(3000, ErrorMessage = ValidationMessages.MaxLength)]
    public string? Description { get; set; }

    [Display(Name = "Görsel URL")]
    public string? ImageUrl { get; set; }

    [Display(Name = "GitHub")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? GitHubUrl { get; set; }

    [Display(Name = "Canlı Demo")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? LiveUrl { get; set; }

    public List<string> Technologies { get; set; } = [];

    [Display(Name = "Öne çıkan proje")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Tamamlanma Tarihi")]
    [DataType(DataType.Date)]
    public DateOnly? CompletedAt { get; set; }

    [Display(Name = "Sıra")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Sitede göster")]
    public bool IsVisible { get; set; } = true;

    [JsonIgnore, Display(Name = "Teknolojiler (virgülle ayırın)")]
    public string TechnologiesText
    {
        get => TextList.ToCsv(Technologies);
        set => Technologies = TextList.FromCsv(value);
    }
}
