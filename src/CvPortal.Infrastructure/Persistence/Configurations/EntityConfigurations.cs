using CvPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CvPortal.Infrastructure.Persistence.Configurations;

// Kolon uzunlukları Application katmanındaki DTO doğrulama kurallarıyla aynıdır.
// Enum'lar SSMS'te okunabilir olsun diye metin olarak saklanır.
// List<string> alanları (teknolojiler, başarılar) EF Core tarafından JSON olarak nvarchar(max) kolonda tutulur.

internal sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> b)
    {
        b.ToTable("Profiles");
        b.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Title).HasMaxLength(100).IsRequired();
        b.Property(x => x.Headline).HasMaxLength(250).IsRequired();
        b.Property(x => x.About).HasMaxLength(4000).IsRequired();
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.Location).HasMaxLength(100).IsRequired();
        b.Property(x => x.GitHubUrl).HasMaxLength(300);
        b.Property(x => x.LinkedInUrl).HasMaxLength(300);
        b.Property(x => x.WebsiteUrl).HasMaxLength(300);
        b.Property(x => x.MediumUrl).HasMaxLength(300);
    }
}

internal sealed class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> b)
    {
        b.ToTable("Experiences");
        b.Property(x => x.Company).HasMaxLength(100).IsRequired();
        b.Property(x => x.CompanyUrl).HasMaxLength(300);
        b.Property(x => x.Position).HasMaxLength(100).IsRequired();
        b.Property(x => x.Location).HasMaxLength(100).IsRequired();
        b.Property(x => x.EmploymentType).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.WorkMode).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        b.Property(x => x.Achievements).HasMaxLength(-1);
        b.Property(x => x.Technologies).HasMaxLength(-1);
        b.Ignore(x => x.IsCurrent);
        b.HasIndex(x => x.DisplayOrder);
    }
}

internal sealed class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> b)
    {
        b.ToTable("Educations");
        b.Property(x => x.School).HasMaxLength(150).IsRequired();
        b.Property(x => x.Degree).HasMaxLength(100).IsRequired();
        b.Property(x => x.FieldOfStudy).HasMaxLength(150).IsRequired();
        b.Property(x => x.Grade).HasMaxLength(20);
        b.Property(x => x.Description).HasMaxLength(1000);
        b.HasIndex(x => x.DisplayOrder);
    }
}

internal sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> b)
    {
        b.ToTable("Skills", t => t.HasCheckConstraint("CK_Skills_Proficiency", "[Proficiency] BETWEEN 0 AND 100"));
        b.Property(x => x.Name).HasMaxLength(50).IsRequired();
        b.Property(x => x.Category).HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.IconClass).HasMaxLength(80);
        b.HasIndex(x => new { x.Category, x.DisplayOrder });
    }
}

internal sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> b)
    {
        b.ToTable("Projects");
        b.Property(x => x.Title).HasMaxLength(100).IsRequired();
        b.Property(x => x.Summary).HasMaxLength(300).IsRequired();
        b.Property(x => x.Description).HasMaxLength(3000);
        b.Property(x => x.GitHubUrl).HasMaxLength(300);
        b.Property(x => x.LiveUrl).HasMaxLength(300);
        b.Property(x => x.Technologies).HasMaxLength(-1);
        b.HasIndex(x => x.DisplayOrder);
    }
}

internal sealed class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> b)
    {
        b.ToTable("Certificates");
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Issuer).HasMaxLength(100).IsRequired();
        b.Property(x => x.CredentialId).HasMaxLength(100);
        b.Property(x => x.CredentialUrl).HasMaxLength(300);
        b.HasIndex(x => x.DisplayOrder);
    }
}

internal sealed class SpokenLanguageConfiguration : IEntityTypeConfiguration<SpokenLanguage>
{
    public void Configure(EntityTypeBuilder<SpokenLanguage> b)
    {
        b.ToTable("Languages");
        b.Property(x => x.Name).HasMaxLength(50).IsRequired();
        b.Property(x => x.Level).HasConversion<string>().HasMaxLength(30);
    }
}

internal sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> b)
    {
        b.ToTable("ContactMessages");
        b.Property(x => x.Name).HasMaxLength(80).IsRequired();
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.Subject).HasMaxLength(120).IsRequired();
        b.Property(x => x.Message).HasMaxLength(3000).IsRequired();
        b.HasIndex(x => new { x.IsRead, x.CreatedAt });
    }
}

internal sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> b)
    {
        b.ToTable("AdminUsers");
        b.Property(x => x.Username).HasMaxLength(50).IsRequired();
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
        b.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        b.HasIndex(x => x.Username).IsUnique();
    }
}

internal sealed class SiteSettingsConfiguration : IEntityTypeConfiguration<SiteSettings>
{
    public void Configure(EntityTypeBuilder<SiteSettings> b)
    {
        b.ToTable("SiteSettings");
        b.Property(x => x.ApiKey).HasMaxLength(100).IsRequired();
    }
}
