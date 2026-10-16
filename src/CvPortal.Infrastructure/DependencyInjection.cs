using CvPortal.Application.Abstractions;
using CvPortal.Domain.Entities;
using CvPortal.Infrastructure.MockData;
using CvPortal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CvPortal.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Veri katmanını kaydeder. Varsayılan sağlayıcı SQL Server'dır (EF Core);
    /// "InMemory" seçilirse veritabanı gerekmeden mock verilerle çalışır.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, DatabaseOptions options)
    {
        services.AddSingleton(options);

        if (options.Provider == DatabaseProvider.InMemory)
            return services.AddInMemoryRepositories();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new InvalidOperationException(
                "Veritabanı bağlantı cümlesi bulunamadı. appsettings.json → ConnectionStrings:DefaultConnection değerini girin " +
                "veya demo için Database:Provider değerini \"InMemory\" yapın.");

        services.AddDbContext<CvPortalDbContext>(db => db.UseSqlServer(options.ConnectionString,
            sql => sql.EnableRetryOnFailure(maxRetryCount: 3)));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        return services;
    }

    /// <summary>
    /// Bellek içi repository'ler singleton olduğu için değişiklikler uygulama çalıştığı sürece korunur.
    /// </summary>
    private static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<Profile>>(_ => new InMemoryRepository<Profile>([MockDataProvider.Profile()]));
        services.AddSingleton<IRepository<Experience>>(_ => new InMemoryRepository<Experience>(MockDataProvider.Experiences()));
        services.AddSingleton<IRepository<Education>>(_ => new InMemoryRepository<Education>(MockDataProvider.Educations()));
        services.AddSingleton<IRepository<Skill>>(_ => new InMemoryRepository<Skill>(MockDataProvider.Skills()));
        services.AddSingleton<IRepository<Project>>(_ => new InMemoryRepository<Project>(MockDataProvider.Projects()));
        services.AddSingleton<IRepository<Certificate>>(_ => new InMemoryRepository<Certificate>(MockDataProvider.Certificates()));
        services.AddSingleton<IRepository<SpokenLanguage>>(_ => new InMemoryRepository<SpokenLanguage>(MockDataProvider.Languages()));
        services.AddSingleton<IRepository<ContactMessage>>(_ => new InMemoryRepository<ContactMessage>(MockDataProvider.Messages()));
        services.AddSingleton<IRepository<AdminUser>>(_ => new InMemoryRepository<AdminUser>());
        services.AddSingleton<IRepository<SiteSettings>>(_ => new InMemoryRepository<SiteSettings>());
        return services;
    }
}
