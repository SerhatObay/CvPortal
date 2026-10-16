using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.RateLimiting;
using CvPortal.Application;
using CvPortal.Infrastructure;
using CvPortal.Infrastructure.Persistence;
using CvPortal.Web.OpenApi;
using CvPortal.Web.Options;
using CvPortal.Web.Security;
using CvPortal.Web.Setup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.WebEncoders;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Veritabanı ----------
var databaseOptions = builder.Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
databaseOptions.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

// ---------- Katmanlar ----------
builder.Services
    .AddApplication()                     // İş servisleri
    .AddInfrastructure(databaseOptions);  // SQL Server (EF Core) veya InMemory

// ---------- Ayarlar ----------
builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(ApiOptions.SectionName));
var apiOptions = builder.Configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>() ?? new ApiOptions();

// Oturum çerezlerini şifreleyen anahtarlar uygulama klasöründe saklanır; böylece uygulama yeniden
// başladığında (IIS geri dönüşümü vb.) yöneticiler oturumdan atılmaz.
var dataProtection = builder.Services.AddDataProtection()
    .SetApplicationName("CvPortal")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys")));
if (OperatingSystem.IsWindows()) dataProtection.ProtectKeysWithDpapi();

// ---------- Kimlik doğrulama (Admin panel: Cookie) ----------
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<ISetupService, SetupService>();
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
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
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

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // İletişim formu spam koruması: IP başına dakikada 5 mesaj
    options.AddPolicy("contact", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));

    // Kaba kuvvet (brute force) koruması: IP başına 5 dakikada 10 giriş / kurulum denemesi
    options.AddPolicy("login", httpContext => RateLimitPartition.GetFixedWindowLimiter(
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(5) }));
});

// Sağlık kontrolü (/health): izleme araçları ve hosting panelleri için
var healthChecks = builder.Services.AddHealthChecks();
if (databaseOptions.Provider == DatabaseProvider.SqlServer)
    healthChecks.AddDbContextCheck<CvPortalDbContext>("database");

var app = builder.Build();

// ---------- Veritabanını hazırla (tablolar + ilk kurulum verisi) ----------
await app.Services.InitializeDatabaseAsync();

// Komut satırı yönetim araçları (ör. şifre sıfırlama) — site başlatılmadan çalışıp çıkar.
if (AdminCommandLine.IsCommand(args))
{
    Environment.ExitCode = await AdminCommandLine.RunAsync(app.Services, args);
    return;
}

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
app.UseMiddleware<SetupGuardMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapHealthChecks("/health");

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
