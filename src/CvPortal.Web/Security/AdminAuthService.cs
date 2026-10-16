using System.Security.Claims;
using CvPortal.Application.Abstractions;
using CvPortal.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace CvPortal.Web.Security;

public interface IAdminAuthService
{
    Task<bool> HasAnyAdminAsync(CancellationToken ct = default);
    Task<AdminUser> CreateAdminAsync(string username, string email, string displayName, string password, CancellationToken ct = default);
    Task<bool> SignInAsync(HttpContext httpContext, string username, string password, bool rememberMe);
    Task SignInAsync(HttpContext httpContext, AdminUser user, bool rememberMe = false);
    Task SignOutAsync(HttpContext httpContext);
    Task<AdminUser?> GetCurrentAsync(ClaimsPrincipal principal, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default);
    Task UpdateAccountAsync(int userId, string email, string displayName, CancellationToken ct = default);

    /// <summary>Mevcut şifre sorulmadan sıfırlar (yalnızca sunucudaki komut satırı aracı kullanır).</summary>
    Task<bool> ResetPasswordAsync(string username, string newPassword, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetUsernamesAsync(CancellationToken ct = default);
}

/// <summary>
/// Veritabanındaki yönetici hesaplarıyla cookie tabanlı giriş.
/// Şifreler ASP.NET Core Identity'nin PasswordHasher'ı (PBKDF2) ile hash'lenir; düz metin asla saklanmaz.
/// </summary>
public sealed class AdminAuthService(IRepository<AdminUser> users, TimeProvider time) : IAdminAuthService
{
    public const string AdminRole = "Admin";
    private static readonly PasswordHasher<AdminUser> Hasher = new();

    public Task<bool> HasAnyAdminAsync(CancellationToken ct = default) => users.AnyAsync(ct: ct);

    public async Task<AdminUser> CreateAdminAsync(string username, string email, string displayName, string password, CancellationToken ct = default)
    {
        var user = new AdminUser
        {
            Username = username.Trim(),
            Email = email.Trim(),
            DisplayName = displayName.Trim(),
            CreatedAt = time.GetUtcNow().UtcDateTime
        };
        user.PasswordHash = Hasher.HashPassword(user, password);
        return await users.AddAsync(user, ct);
    }

    public async Task<bool> SignInAsync(HttpContext httpContext, string username, string password, bool rememberMe)
    {
        var normalized = username.Trim();
        var user = await users.FirstOrDefaultAsync(u => u.Username == normalized, httpContext.RequestAborted);
        if (user is null)
        {
            // Kullanıcı yokken de hash hesaplanır; böylece yanıt süresinden kullanıcı adı tahmin edilemez.
            Hasher.HashPassword(new AdminUser(), password);
            return false;
        }

        var result = Hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return false;

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = Hasher.HashPassword(user, password);

        user.LastLoginAt = time.GetUtcNow().UtcDateTime;
        await users.UpdateAsync(user, httpContext.RequestAborted);

        await SignInAsync(httpContext, user, rememberMe);
        return true;
    }

    public Task SignInAsync(HttpContext httpContext, AdminUser user, bool rememberMe = false)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, AdminRole)
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        return httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe ? time.GetUtcNow().AddDays(7) : null
        });
    }

    public Task SignOutAsync(HttpContext httpContext)
        => httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public async Task<AdminUser?> GetCurrentAsync(ClaimsPrincipal principal, CancellationToken ct = default)
        => int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? await users.GetByIdAsync(id, ct)
            : null;

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var user = await users.GetByIdAsync(userId, ct);
        if (user is null || Hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword) == PasswordVerificationResult.Failed)
            return false;

        user.PasswordHash = Hasher.HashPassword(user, newPassword);
        user.UpdatedAt = time.GetUtcNow().UtcDateTime;
        await users.UpdateAsync(user, ct);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string username, string newPassword, CancellationToken ct = default)
    {
        var normalized = username.Trim();
        var user = await users.FirstOrDefaultAsync(u => u.Username == normalized, ct);
        if (user is null) return false;

        user.PasswordHash = Hasher.HashPassword(user, newPassword);
        user.UpdatedAt = time.GetUtcNow().UtcDateTime;
        await users.UpdateAsync(user, ct);
        return true;
    }

    public async Task<IReadOnlyList<string>> GetUsernamesAsync(CancellationToken ct = default)
        => (await users.GetAllAsync(ct)).Select(u => u.Username).Order().ToList();

    public async Task UpdateAccountAsync(int userId, string email, string displayName, CancellationToken ct = default)
    {
        var user = await users.GetByIdAsync(userId, ct) ?? throw new InvalidOperationException("Kullanıcı bulunamadı.");
        user.Email = email.Trim();
        user.DisplayName = displayName.Trim();
        user.UpdatedAt = time.GetUtcNow().UtcDateTime;
        await users.UpdateAsync(user, ct);
    }
}
