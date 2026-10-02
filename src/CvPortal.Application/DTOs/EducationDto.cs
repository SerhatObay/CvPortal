using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;

namespace CvPortal.Application.DTOs;

public class EducationDto : IHasId, IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Okul")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(150, ErrorMessage = ValidationMessages.MaxLength)]
    public string School { get; set; } = string.Empty;

    [Display(Name = "Derece")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Degree { get; set; } = string.Empty;

    [Display(Name = "Bölüm")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(150, ErrorMessage = ValidationMessages.MaxLength)]
    public string FieldOfStudy { get; set; } = string.Empty;

    [Display(Name = "Başlangıç")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Date)]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Mezuniyet (boş = devam ediyor)")]
    [DataType(DataType.Date)]
    public DateOnly? EndDate { get; set; }

    [Display(Name = "Not Ortalaması")]
    [StringLength(20, ErrorMessage = ValidationMessages.MaxLength)]
    public string? Grade { get; set; }

    [Display(Name = "Açıklama")]
    [StringLength(1000, ErrorMessage = ValidationMessages.MaxLength)]
    public string? Description { get; set; }

    [Display(Name = "Sıra")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Sitede göster")]
    public bool IsVisible { get; set; } = true;

    public string Period => DateRangeFormatter.Period(StartDate, EndDate).Replace("Halen", "Devam ediyor");

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate is not null && EndDate < StartDate)
            yield return new ValidationResult("Mezuniyet tarihi başlangıç tarihinden önce olamaz.", [nameof(EndDate)]);
    }
}
