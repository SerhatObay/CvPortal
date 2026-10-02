using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CvPortal.Application.Common;
using CvPortal.Domain.Enums;

namespace CvPortal.Application.DTOs;

public class ExperienceDto : IHasId, IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Şirket")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Company { get; set; } = string.Empty;

    [Display(Name = "Şirket Web Sitesi")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? CompanyUrl { get; set; }

    [Display(Name = "Pozisyon")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Position { get; set; } = string.Empty;

    [Display(Name = "Konum")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Çalışma Tipi")]
    public EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;

    [Display(Name = "Çalışma Şekli")]
    public WorkMode WorkMode { get; set; } = WorkMode.Hybrid;

    [Display(Name = "Başlangıç")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Date)]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Bitiş (boş = halen)")]
    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    [Display(Name = "Açıklama")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(2000, ErrorMessage = ValidationMessages.MaxLength)]
    public string Description { get; set; } = string.Empty;

    public List<string> Achievements { get; set; } = [];

    public List<string> Technologies { get; set; } = [];

    [Display(Name = "Sıra")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Sitede göster")]
    public bool IsVisible { get; set; } = true;

    // --- Hesaplanan alanlar ---
    public bool IsCurrent => EndDate is null;
    public string Period => DateRangeFormatter.Period(StartDate, EndDate);
    public string Duration => DateRangeFormatter.Duration(StartDate, EndDate);

    // --- Form yardımcıları (API'de gösterilmez) ---
    [JsonIgnore, Display(Name = "Başarılar (her satıra bir madde)")]
    public string AchievementsText
    {
        get => TextList.ToLines(Achievements);
        set => Achievements = TextList.FromLines(value);
    }

    [JsonIgnore, Display(Name = "Teknolojiler (virgülle ayırın)")]
    public string TechnologiesText
    {
        get => TextList.ToCsv(Technologies);
        set => Technologies = TextList.FromCsv(value);
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate is not null && EndDate < StartDate)
            yield return new ValidationResult("Bitiş tarihi başlangıç tarihinden önce olamaz.", [nameof(EndDate)]);
    }
}
