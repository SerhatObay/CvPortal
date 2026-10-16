namespace CvPortal.Application.DTOs;

public class SiteSettingsDto
{
    public string ApiKey { get; set; } = string.Empty;
    public bool IsApiEnabled { get; set; }
    public DateTime? SetupCompletedAt { get; set; }

    public bool IsSetupCompleted => SetupCompletedAt is not null;

    /// <summary>Ekranda gösterim için maskelenmiş anahtar: "cvp_ab12••••••••9f".</summary>
    public string MaskedApiKey => ApiKey.Length <= 10 ? "••••••••" : $"{ApiKey[..6]}••••••••{ApiKey[^4..]}";
}
