using CvPortal.Application.Abstractions;
using CvPortal.Domain.Entities;

namespace CvPortal.Application.Services;

public interface IContentResetService
{
    /// <summary>
    /// Tüm CV içeriğini (deneyim, eğitim, yetenek, proje, sertifika, dil, mesaj) siler.
    /// Kurulum sihirbazında "boş başla" seçildiğinde kullanılır. Profil ve yönetici hesabı silinmez.
    /// </summary>
    Task ClearAllContentAsync(CancellationToken ct = default);
}

public sealed class ContentResetService(
    IRepository<Experience> experiences,
    IRepository<Education> educations,
    IRepository<Skill> skills,
    IRepository<Project> projects,
    IRepository<Certificate> certificates,
    IRepository<SpokenLanguage> languages,
    IRepository<ContactMessage> messages) : IContentResetService
{
    public async Task ClearAllContentAsync(CancellationToken ct = default)
    {
        await experiences.DeleteAllAsync(ct);
        await educations.DeleteAllAsync(ct);
        await skills.DeleteAllAsync(ct);
        await projects.DeleteAllAsync(ct);
        await certificates.DeleteAllAsync(ct);
        await languages.DeleteAllAsync(ct);
        await messages.DeleteAllAsync(ct);
    }
}
