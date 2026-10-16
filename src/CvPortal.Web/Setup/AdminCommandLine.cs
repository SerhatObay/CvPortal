using System.Text.RegularExpressions;
using CvPortal.Web.Security;

namespace CvPortal.Web.Setup;

/// <summary>
/// Sunucuda çalıştırılan yönetim komutları. Web sitesi başlatılmaz, komut çalışıp çıkar.
///   CvPortal.Web.exe reset-password &lt;kullanıcı&gt; &lt;yeni-şifre&gt;
///   dotnet run --project src/CvPortal.Web -- reset-password admin YeniSifre123
/// </summary>
public static class AdminCommandLine
{
    public static bool IsCommand(string[] args) => args.Length > 0 && args[0] == "reset-password";

    public static async Task<int> RunAsync(IServiceProvider services, string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Kullanım: CvPortal.Web.exe reset-password <kullanıcı> <yeni-şifre>");
            return 1;
        }

        var (username, password) = (args[1], args[2]);
        if (password.Length < 8 || !Regex.IsMatch(password, PasswordRules.Pattern))
        {
            Console.WriteLine($"Şifre en az 8 karakter olmalı. {PasswordRules.Message}");
            return 1;
        }

        using var scope = services.CreateScope();
        var auth = scope.ServiceProvider.GetRequiredService<IAdminAuthService>();

        if (await auth.ResetPasswordAsync(username, password))
        {
            Console.WriteLine($"'{username}' kullanıcısının şifresi güncellendi.");
            return 0;
        }

        var existing = await auth.GetUsernamesAsync();
        Console.WriteLine(existing.Count == 0
            ? "Henüz yönetici hesabı yok. Siteyi açıp /setup adresinden kurulumu tamamlayın."
            : $"'{username}' bulunamadı. Mevcut kullanıcılar: {string.Join(", ", existing)}");
        return 1;
    }
}
