namespace CvPortal.Web.Setup;

/// <summary>
/// Kurulum tamamlanmadıysa admin paneline gelen istekleri /setup sihirbazına yönlendirir.
/// Kurulum bir kez tamamlandıktan sonra sonuç bellekte tutulur; her istekte veritabanına gidilmez.
/// </summary>
public sealed class SetupGuardMiddleware(RequestDelegate next)
{
    private static volatile bool _setupCompleted;

    public static void MarkCompleted() => _setupCompleted = true;

    public async Task InvokeAsync(HttpContext context, ISetupService setup)
    {
        if (!_setupCompleted && context.Request.Path.StartsWithSegments("/Admin", StringComparison.OrdinalIgnoreCase))
        {
            // SetupService ile aynı kural: ayar kaydı tamamlandı VEYA en az bir yönetici var.
            if (await setup.IsCompletedAsync(context.RequestAborted))
            {
                _setupCompleted = true;
            }
            else
            {
                context.Response.Redirect("/setup");
                return;
            }
        }

        await next(context);
    }
}
