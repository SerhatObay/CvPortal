namespace CvPortal.Web.Options;

/// <summary>
/// Harici paneller / istemciler için REST API ayarları (appsettings.json → "Api").
/// API anahtarı veritabanında saklanır ve admin panel → Ayarlar ekranından yönetilir.
/// </summary>
public class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>API'yi tarayıcıdan çağırabilecek harici panel adresleri (CORS).</summary>
    public string[] AllowedOrigins { get; set; } = [];
}
