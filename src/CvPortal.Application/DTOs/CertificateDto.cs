using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;

namespace CvPortal.Application.DTOs;

public class CertificateDto : IHasId, IValidatableObject
{
    public int Id { get; set; }

    [Display(Name = "Sertifika Adı")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(150, ErrorMessage = ValidationMessages.MaxLength)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Veren Kurum")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Issuer { get; set; } = string.Empty;

    [Display(Name = "Veriliş Tarihi")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Date)]
    public DateOnly IssueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Geçerlilik Bitişi")]
    [DataType(DataType.Date)]
    public DateOnly? ExpirationDate { get; set; }

    [Display(Name = "Sertifika No")]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string? CredentialId { get; set; }

    [Display(Name = "Doğrulama Linki")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? CredentialUrl { get; set; }

    [Display(Name = "Sıra")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Sitede göster")]
    public bool IsVisible { get; set; } = true;

    public bool IsExpired => ExpirationDate is not null && ExpirationDate < DateOnly.FromDateTime(DateTime.Today);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpirationDate is not null && ExpirationDate < IssueDate)
            yield return new ValidationResult("Geçerlilik bitişi veriliş tarihinden önce olamaz.", [nameof(ExpirationDate)]);
    }
}
