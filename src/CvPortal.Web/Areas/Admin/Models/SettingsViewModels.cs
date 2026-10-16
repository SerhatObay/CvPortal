using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;
using CvPortal.Application.DTOs;
using CvPortal.Web.Setup;

namespace CvPortal.Web.Areas.Admin.Models;

public class SettingsViewModel
{
    public string Username { get; init; } = string.Empty;
    public DateTime? LastLoginAt { get; init; }
    public required SiteSettingsDto Site { get; init; }
    public AccountSettingsInput Account { get; init; } = new();
    public ChangePasswordInput Password { get; init; } = new();
}

public class AccountSettingsInput
{
    [Display(Name = "Görünen ad")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string DisplayName { get; set; } = string.Empty;

    [Display(Name = "E-posta")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [EmailAddress(ErrorMessage = ValidationMessages.Email)]
    [StringLength(256, ErrorMessage = ValidationMessages.MaxLength)]
    public string Email { get; set; } = string.Empty;
}

public class ChangePasswordInput
{
    [Display(Name = "Mevcut şifre")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Display(Name = "Yeni şifre")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = ValidationMessages.Length)]
    [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
    public string NewPassword { get; set; } = string.Empty;

    [Display(Name = "Yeni şifre (tekrar)")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
