using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CvPortal.Web.Security;

/// <summary>
/// REST API yönetim uç noktalarını "X-Api-Key" başlığı ile korur.
/// Anahtar veritabanında (SiteSettings) saklanır; admin panel → Ayarlar ekranından açılıp kapatılır ve yenilenir.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public const string HeaderName = "X-Api-Key";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousApiAttribute>().Any())
            return;

        var settings = context.HttpContext.RequestServices.GetRequiredService<ISiteSettingsService>();
        var provided = context.HttpContext.Request.Headers[HeaderName].ToString();

        context.Result = await settings.ValidateApiKeyAsync(provided, context.HttpContext.RequestAborted) switch
        {
            ApiKeyValidationResult.Valid => null,
            ApiKeyValidationResult.ApiDisabled => Problem(StatusCodes.Status403Forbidden, "API kapalı",
                "Yönetim API'si devre dışı. Admin panel → Ayarlar ekranından etkinleştirin."),
            _ => Problem(StatusCodes.Status401Unauthorized, "Yetkisiz erişim",
                $"Geçerli bir '{HeaderName}' başlığı gönderilmelidir.")
        };
    }

    private static ObjectResult Problem(int status, string title, string detail) => new(new ProblemDetails
    {
        Status = status,
        Title = title,
        Detail = detail
    })
    { StatusCode = status };
}

/// <summary>
/// API anahtarı gerektirmeyen (herkese açık) uç noktaları işaretler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowAnonymousApiAttribute : Attribute;
