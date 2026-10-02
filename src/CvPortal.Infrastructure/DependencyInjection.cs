using CvPortal.Application.Abstractions;
using CvPortal.Domain.Entities;
using CvPortal.Infrastructure.MockData;
using CvPortal.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CvPortal.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Mock (in-memory) veri katmanını kaydeder. Repository'ler singleton olduğu için
    /// admin panelden yapılan değişiklikler uygulama çalıştığı sürece korunur.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<Profile>>(_ => new InMemoryRepository<Profile>([MockDataProvider.Profile()]));
        services.AddSingleton<IRepository<Experience>>(_ => new InMemoryRepository<Experience>(MockDataProvider.Experiences()));
        services.AddSingleton<IRepository<Education>>(_ => new InMemoryRepository<Education>(MockDataProvider.Educations()));
        services.AddSingleton<IRepository<Skill>>(_ => new InMemoryRepository<Skill>(MockDataProvider.Skills()));
        services.AddSingleton<IRepository<Project>>(_ => new InMemoryRepository<Project>(MockDataProvider.Projects()));
        services.AddSingleton<IRepository<Certificate>>(_ => new InMemoryRepository<Certificate>(MockDataProvider.Certificates()));
        services.AddSingleton<IRepository<SpokenLanguage>>(_ => new InMemoryRepository<SpokenLanguage>(MockDataProvider.Languages()));
        services.AddSingleton<IRepository<ContactMessage>>(_ => new InMemoryRepository<ContactMessage>(MockDataProvider.Messages()));

        return services;
    }
}
