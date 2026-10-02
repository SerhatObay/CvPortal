using System.Security.Cryptography;
using System.Text;
using CvPortal.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CvPortal.Web.Security;

/// <summary>
/// REST API yönetim uç noktalarını "X-Api-Key" başlığı ile korur.
/// Böylece admin panel dışındaki başka bir panel (React, mobil uygulama vb.) de içeriği yönetebilir.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public const string HeaderName = "X-Api-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousApiAttribute>().Any())
            return;

        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            context.Result = Problem(StatusCodes.Status503ServiceUnavailable, "API anahtarı yapılandırılmamış.");
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var provided)
            || !FixedTimeEquals(provided.ToString(), options.ApiKey))
        {
            context.Result = Problem(StatusCodes.Status401Unauthorized, $"Geçerli bir '{HeaderName}' başlığı gönderilmelidir.");
        }
    }

    private static bool FixedTimeEquals(string a, string b)
        => CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));

    private static ObjectResult Problem(int status, string detail) => new(new ProblemDetails
    {
        Status = status,
        Title = status == 401 ? "Yetkisiz erişim" : "Servis kullanılamıyor",
        Detail = detail
    })
    { StatusCode = status };
}

/// <summary>
/// API anahtarı gerektirmeyen (herkese açık) uç noktaları işaretler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowAnonymousApiAttribute : Attribute;
