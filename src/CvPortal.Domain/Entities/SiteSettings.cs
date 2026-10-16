using CvPortal.Domain.Common;

namespace CvPortal.Domain.Entities;

/// <summary>
/// Uygulama genelindeki ayarlar (tekil kayıt). Kurulum sihirbazında oluşturulur, admin panelden yönetilir.
/// </summary>
public class SiteSettings : BaseEntity
{
    /// <summary>REST API yönetim uç noktaları için "X-Api-Key" değeri.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Kapalıysa yönetim API'si anahtar doğru olsa bile 403 döner.</summary>
    public bool IsApiEnabled { get; set; }

    /// <summary>Kurulum sihirbazının tamamlandığı zaman (UTC).</summary>
    public DateTime? SetupCompletedAt { get; set; }
}
