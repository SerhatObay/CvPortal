using CvPortal.Web.Security;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CvPortal.Web.OpenApi;

/// <summary>
/// OpenAPI dokümanına API bilgilerini ve "X-Api-Key" güvenlik şemasını ekler.
/// </summary>
internal sealed class ApiKeySecurityTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeName = "ApiKey";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "CvPortal API",
            Version = "v1",
            Description = "CV içeriğini harici panellerden yönetmek için REST API. " +
                          $"Yönetim uç noktaları '{ApiKeyAuthorizeAttribute.HeaderName}' başlığı gerektirir; " +
                          "GET /api/v1/cv ve POST /api/v1/messages herkese açıktır."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyAuthorizeAttribute.HeaderName,
            Description = "appsettings.json → Api:ApiKey değeri"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, document)] = []
        });

        return Task.CompletedTask;
    }
}
