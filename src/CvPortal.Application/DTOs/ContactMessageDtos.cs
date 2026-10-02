using System.ComponentModel.DataAnnotations;
using CvPortal.Application.Common;

namespace CvPortal.Application.DTOs;

public class ContactMessageDto : IHasId
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

/// <summary>
/// Ziyaretçinin iletişim formundan gönderdiği veri.
/// </summary>
public class CreateContactMessageDto
{
    [Display(Name = "Adınız")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(80, MinimumLength = 2, ErrorMessage = ValidationMessages.Length)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "E-posta")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [EmailAddress(ErrorMessage = ValidationMessages.Email)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Konu")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(120, MinimumLength = 3, ErrorMessage = ValidationMessages.Length)]
    public string Subject { get; set; } = string.Empty;

    [Display(Name = "Mesaj")]
    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(3000, MinimumLength = 10, ErrorMessage = ValidationMessages.Length)]
    public string Message { get; set; } = string.Empty;
}
