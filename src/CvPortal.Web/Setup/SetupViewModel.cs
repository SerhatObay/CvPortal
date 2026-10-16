using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;

namespace CvPortal.Web.Setup;

public class SetupViewModel
{
    [Display(Name = "Ad Soyad")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Ünvan")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(100, ErrorMessage = ValidationMessages.MaxLength)]
    public string Title { get; set; } = "Yazılım Mühendisi";

    [Display(Name = "E-posta")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [EmailAddress(ErrorMessage = ValidationMessages.Email)]
    [StringLength(256, ErrorMessage = ValidationMessages.MaxLength)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Kullanıcı adı")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(50, MinimumLength = 3, ErrorMessage = ValidationMessages.Length)]
    [RegularExpression("^[a-zA-Z0-9._-]+$", ErrorMessage = "Kullanıcı adı yalnızca harf, rakam, nokta, tire ve alt çizgi içerebilir.")]
    public string Username { get; set; } = "admin";

    [Display(Name = "Şifre")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = ValidationMessages.Length)]
    [RegularExpression(PasswordRules.Pattern, ErrorMessage = PasswordRules.Message)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Şifre (tekrar)")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Örnek içeriği sil, boş bir CV ile başla")]
    public bool StartEmpty { get; set; }

    [Display(Name = "Harici paneller için REST API'yi etkinleştir")]
    public bool EnableApi { get; set; }
}

public static class PasswordRules
{
    /// <summary>En az bir harf ve bir rakam.</summary>
    public const string Pattern = @"^(?=.*[A-Za-zçğıöşüÇĞİÖŞÜ])(?=.*\d).+$";
    public const string Message = "Şifre en az bir harf ve bir rakam içermelidir.";
}
