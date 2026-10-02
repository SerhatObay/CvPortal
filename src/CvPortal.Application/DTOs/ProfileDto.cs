using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;

namespace CvPortal.Application.DTOs;

public class ProfileDto
{
    [Display(Name = "Ad Soyad")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Ünvan")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Kısa Tanıtım (Hero)")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(250, ErrorMessage = ValidationMessages.MaxLength)]
    public string Headline { get; set; } = string.Empty;

    [Display(Name = "Hakkımda")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(4000, ErrorMessage = ValidationMessages.MaxLength)]
    public string About { get; set; } = string.Empty;

    [Display(Name = "E-posta")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [EmailAddress(ErrorMessage = ValidationMessages.Email)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Telefon")]
    [Phone(ErrorMessage = ValidationMessages.Phone)]
    public string? Phone { get; set; }

    [Display(Name = "Konum")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Location { get; set; } = string.Empty;

    [Display(Name = "Profil Fotoğrafı URL")]
    public string? AvatarUrl { get; set; }

    [Display(Name = "CV Dosyası (PDF) URL")]
    public string? CvFileUrl { get; set; }

    [Display(Name = "GitHub")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? GitHubUrl { get; set; }

    [Display(Name = "LinkedIn")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? LinkedInUrl { get; set; }

    [Display(Name = "Web Sitesi")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? WebsiteUrl { get; set; }

    [Display(Name = "Medium / Blog")]
    [Url(ErrorMessage = ValidationMessages.Url)]
    public string? MediumUrl { get; set; }

    [Display(Name = "Kariyer Başlangıç Tarihi")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Date)]
    public DateOnly CareerStartDate { get; set; }

    [Display(Name = "Yeni fırsatlara açığım")]
    public bool IsOpenToWork { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
