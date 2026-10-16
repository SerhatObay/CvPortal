using CvPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CvPortal.Infrastructure.Persistence;

public class CvPortalDbContext(DbContextOptions<CvPortalDbContext> options) : DbContext(options)
{
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<SpokenLanguage> Languages => Set<SpokenLanguage>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(CvPortalDbContext).Assembly);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Sınırı belirtilmeyen metin kolonları nvarchar(max) yerine makul bir uzunlukla oluşturulsun.
        configurationBuilder.Properties<string>().HaveMaxLength(500);
    }
}
