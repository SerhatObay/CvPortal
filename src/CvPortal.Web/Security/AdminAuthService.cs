using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CvPortal.Web.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace CvPortal.Web.Security;

public interface IAdminAuthService
{
    Task<bool> SignInAsync(HttpContext httpContext, string username, string password, bool rememberMe);
    Task SignOutAsync(HttpContext httpContext);
}

/// <summary>
/// Mock kullanıcı ile (appsettings) cookie tabanlı admin girişi.
/// </summary>
public sealed class AdminAuthService(IOptionsMonitor<AdminOptions> options) : IAdminAuthService
{
    public const string AdminRole = "Admin";

    public async Task<bool> SignInAsync(HttpContext httpContext, string username, string password, bool rememberMe)
    {
        var admin = options.CurrentValue;

        var valid = SecureEquals(username.Trim(), admin.Username) & SecureEquals(password, admin.Password);
        if (!valid || string.IsNullOrEmpty(admin.Username)) return false;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Username),
            new(ClaimTypes.Name, admin.DisplayName),
            new(ClaimTypes.Role, AdminRole)
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : null
        });

        return true;
    }

    public Task SignOutAsync(HttpContext httpContext)
        => httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    private static bool SecureEquals(string a, string b)
        => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
