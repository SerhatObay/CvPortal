using System.ComponentModel.DataAnnotations;
using CvPortal.Application.DTOs;

namespace CvPortal.Web.Models;

public class HomeViewModel
{
    public required CvDto Cv { get; init; }
    public CreateContactMessageDto Contact { get; init; } = new();
}

public class LoginViewModel
{
    [Display(Name = "Kullanıcı adı")]
    [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Şifre")]
    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Beni hatırla")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

/// <summary>
/// Admin listelerindeki satır işlemleri (düzenle / gizle-göster / sil) partial'ı için model.
/// </summary>
public record RowActionsModel(int Id, bool IsVisible, string ItemName);
