namespace CvPortal.Web.Options;

/// <summary>
/// Admin panel giriş bilgileri (appsettings.json → "Admin").
/// Demo amaçlıdır; gerçek kullanımda User Secrets / ortam değişkeni ve hash'lenmiş şifre kullanın.
/// </summary>
public class AdminOptions
{
    public const string SectionName = "Admin";

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = "Yönetici";
}

/// <summary>
/// Harici paneller / istemciler için REST API ayarları (appsettings.json → "Api").
/// </summary>
public class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>Yönetim uç noktaları için "X-Api-Key" başlığında gönderilmesi gereken anahtar.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>API'yi tarayıcıdan çağırabilecek harici panel adresleri (CORS).</summary>
    public string[] AllowedOrigins { get; set; } = [];
}
