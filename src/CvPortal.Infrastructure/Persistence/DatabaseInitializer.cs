using CvPortal.Domain.Entities;
using CvPortal.Infrastructure.MockData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CvPortal.Infrastructure.Persistence;

/// <summary>
/// Uygulama açılışında veritabanını hazırlar: migration'ları uygular (veritabanı yoksa oluşturur)
/// ve ilk kurulumda örnek CV içeriğini ekler. "Fişi tak çalıştır" kurulumun kalbi burasıdır.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<DatabaseOptions>();
        if (options.Provider != DatabaseProvider.SqlServer) return;

        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CvPortal.Database");
        var db = scope.ServiceProvider.GetRequiredService<CvPortalDbContext>();

        try
        {
            if (options.AutoMigrate)
            {
                var pending = (await db.Database.GetPendingMigrationsAsync(ct)).ToList();
                if (pending.Count > 0)
                {
                    logger.LogInformation("Veritabanı güncelleniyor: {Migrations}", string.Join(", ", pending));
                    await db.Database.MigrateAsync(ct);
                }
            }

            if (options.SeedDemoData)
                await SeedDemoDataAsync(db, logger, ct);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex,
                "Veritabanına bağlanılamadı. SQL Server'ın çalıştığını ve appsettings.json → ConnectionStrings:DefaultConnection " +
                "değerinin doğru olduğunu kontrol edin.");
            throw;
        }
    }

    /// <summary>
    /// Yalnızca hiç profil yoksa (ilk kurulum) örnek içerik ekler. Kurulum sihirbazında
    /// "demo içeriği temizle" seçilirse profil boşaltılır ama silinmez; böylece demo tekrar eklenmez.
    /// </summary>
    public static async Task SeedDemoDataAsync(CvPortalDbContext db, ILogger logger, CancellationToken ct)
    {
        if (await db.Profiles.AnyAsync(ct)) return;

        var profile = MockDataProvider.Profile();
        profile.Id = 0;

        db.Profiles.Add(profile);
        db.Experiences.AddRange(MockDataProvider.Experiences());
        db.Educations.AddRange(MockDataProvider.Educations());
        db.Skills.AddRange(MockDataProvider.Skills());
        db.Projects.AddRange(MockDataProvider.Projects());
        db.Certificates.AddRange(MockDataProvider.Certificates());
        db.Languages.AddRange(MockDataProvider.Languages());

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Örnek CV içeriği veritabanına eklendi.");
    }
}
