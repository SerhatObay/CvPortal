using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.RateLimiting;
using CvPortal.Application;
using CvPortal.Infrastructure;
using CvPortal.Web.OpenApi;
using CvPortal.Web.Options;
using CvPortal.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.WebEncoders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Katmanlar ----------
builder.Services
    .AddApplication()      // İş servisleri
    .AddInfrastructure();  // Mock (in-memory) veri katmanı

// ---------- Ayarlar ----------
builder.Services.Configure<AdminOptions>(builder.Configuration.GetSection(AdminOptions.SectionName));
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.SectionName));
var apiOptions = builder.Configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>() ?? new ApiOptions();

// ---------- Kimlik doğrulama (Admin panel: Cookie) ----------
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.LogoutPath = "/Admin/Account/Logout";
        options.AccessDeniedPath = "/Admin/Account/Login";
        options.Cookie.Name = "CvPortal.Admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

// ---------- MVC + API ----------
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Türkçe karakterlerin HTML çıktısında &#x131; gibi encode edilmemesi için
builder.Services.Configure<WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<ApiKeySecurityTransformer>());

// Harici yönetim panelleri için CORS
builder.Services.AddCors(options => options.AddPolicy("ExternalPanels", policy =>
    policy.WithOrigins(apiOptions.AllowedOrigins)
          .AllowAnyHeader()
          .AllowAnyMethod()));

// İletişim formu spam koruması: IP başına dakikada 5 mesaj
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("contact", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));
});

var app = builder.Build();

// ---------- Pipeline ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

var turkish = new CultureInfo("tr-TR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(turkish),
    SupportedCultures = [turkish],
    SupportedUICultures = [turkish]
});

app.UseHttpsRedirection();
// HTML hata sayfaları yalnızca site için; API istemcileri ham durum kodunu / ProblemDetails'i alır.
app.UseWhen(ctx => !ctx.Request.Path.StartsWithSegments("/api"),
    branch => branch.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}"));
app.UseRouting();

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// API dokümantasyonu: /openapi/v1.json ve /scalar/v1
app.MapOpenApi();
app.MapScalarApiReference(options => options
    .WithTitle("CvPortal API")
    .AddPreferredSecuritySchemes(ApiKeySecurityTransformer.SchemeName));

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

// Entegrasyon testleri için (WebApplicationFactory<Program>)
public partial class Program;
