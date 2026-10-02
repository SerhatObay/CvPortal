using CvPortal.Application.DTOs;
using CvPortal.Domain.Entities;

namespace CvPortal.Application.Mapping;

/// <summary>
/// Entity &lt;-&gt; DTO dönüşümleri. Bağımlılık eklememek için AutoMapper yerine elle yazıldı.
/// </summary>
public static class MappingExtensions
{
    // ---------- Profile ----------
    public static ProfileDto ToDto(this Profile e) => new()
    {
        FullName = e.FullName,
        Title = e.Title,
        Headline = e.Headline,
        About = e.About,
        Email = e.Email,
        Phone = e.Phone,
        Location = e.Location,
        AvatarUrl = e.AvatarUrl,
        CvFileUrl = e.CvFileUrl,
        GitHubUrl = e.GitHubUrl,
        LinkedInUrl = e.LinkedInUrl,
        WebsiteUrl = e.WebsiteUrl,
        MediumUrl = e.MediumUrl,
        CareerStartDate = e.CareerStartDate,
        IsOpenToWork = e.IsOpenToWork,
        UpdatedAt = e.UpdatedAt
    };

    public static void Apply(this ProfileDto d, Profile e)
    {
        e.FullName = d.FullName.Trim();
        e.Title = d.Title.Trim();
        e.Headline = d.Headline.Trim();
        e.About = d.About.Trim();
        e.Email = d.Email.Trim();
        e.Phone = d.Phone?.Trim();
        e.Location = d.Location.Trim();
        e.AvatarUrl = d.AvatarUrl?.Trim();
        e.CvFileUrl = d.CvFileUrl?.Trim();
        e.GitHubUrl = d.GitHubUrl?.Trim();
        e.LinkedInUrl = d.LinkedInUrl?.Trim();
        e.WebsiteUrl = d.WebsiteUrl?.Trim();
        e.MediumUrl = d.MediumUrl?.Trim();
        e.CareerStartDate = d.CareerStartDate;
        e.IsOpenToWork = d.IsOpenToWork;
    }

    // ---------- Experience ----------
    public static ExperienceDto ToDto(this Experience e) => new()
    {
        Id = e.Id,
        Company = e.Company,
        CompanyUrl = e.CompanyUrl,
        Position = e.Position,
        Location = e.Location,
        EmploymentType = e.EmploymentType,
        WorkMode = e.WorkMode,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        Description = e.Description,
        Achievements = [.. e.Achievements],
        Technologies = [.. e.Technologies],
        DisplayOrder = e.DisplayOrder,
        IsVisible = e.IsVisible
    };

    public static void Apply(this ExperienceDto d, Experience e)
    {
        e.Company = d.Company.Trim();
        e.CompanyUrl = d.CompanyUrl?.Trim();
        e.Position = d.Position.Trim();
        e.Location = d.Location.Trim();
        e.EmploymentType = d.EmploymentType;
        e.WorkMode = d.WorkMode;
        e.StartDate = d.StartDate;
        e.EndDate = d.EndDate;
        e.Description = d.Description.Trim();
        e.Achievements = [.. d.Achievements];
        e.Technologies = [.. d.Technologies];
        e.DisplayOrder = d.DisplayOrder;
        e.IsVisible = d.IsVisible;
    }

    // ---------- Education ----------
    public static EducationDto ToDto(this Education e) => new()
    {
        Id = e.Id,
        School = e.School,
        Degree = e.Degree,
        FieldOfStudy = e.FieldOfStudy,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        Grade = e.Grade,
        Description = e.Description,
        DisplayOrder = e.DisplayOrder,
        IsVisible = e.IsVisible
    };

    public static void Apply(this EducationDto d, Education e)
    {
        e.School = d.School.Trim();
        e.Degree = d.Degree.Trim();
        e.FieldOfStudy = d.FieldOfStudy.Trim();
        e.StartDate = d.StartDate;
        e.EndDate = d.EndDate;
        e.Grade = d.Grade?.Trim();
        e.Description = d.Description?.Trim();
        e.DisplayOrder = d.DisplayOrder;
        e.IsVisible = d.IsVisible;
    }

    // ---------- Skill ----------
    public static SkillDto ToDto(this Skill e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Category = e.Category,
        Proficiency = e.Proficiency,
        IconClass = e.IconClass,
        DisplayOrder = e.DisplayOrder,
        IsVisible = e.IsVisible
    };

    public static void Apply(this SkillDto d, Skill e)
    {
        e.Name = d.Name.Trim();
        e.Category = d.Category;
        e.Proficiency = Math.Clamp(d.Proficiency, 0, 100);
        e.IconClass = d.IconClass?.Trim();
        e.DisplayOrder = d.DisplayOrder;
        e.IsVisible = d.IsVisible;
    }

    // ---------- Project ----------
    public static ProjectDto ToDto(this Project e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Summary = e.Summary,
        Description = e.Description,
        ImageUrl = e.ImageUrl,
        GitHubUrl = e.GitHubUrl,
        LiveUrl = e.LiveUrl,
        Technologies = [.. e.Technologies],
        IsFeatured = e.IsFeatured,
        CompletedAt = e.CompletedAt,
        DisplayOrder = e.DisplayOrder,
        IsVisible = e.IsVisible
    };

    public static void Apply(this ProjectDto d, Project e)
    {
        e.Title = d.Title.Trim();
        e.Summary = d.Summary.Trim();
        e.Description = d.Description?.Trim();
        e.ImageUrl = d.ImageUrl?.Trim();
        e.GitHubUrl = d.GitHubUrl?.Trim();
        e.LiveUrl = d.LiveUrl?.Trim();
        e.Technologies = [.. d.Technologies];
        e.IsFeatured = d.IsFeatured;
        e.CompletedAt = d.CompletedAt;
        e.DisplayOrder = d.DisplayOrder;
        e.IsVisible = d.IsVisible;
    }

    // ---------- Certificate ----------
    public static CertificateDto ToDto(this Certificate e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Issuer = e.Issuer,
        IssueDate = e.IssueDate,
        ExpirationDate = e.ExpirationDate,
        CredentialId = e.CredentialId,
        CredentialUrl = e.CredentialUrl,
        DisplayOrder = e.DisplayOrder,
        IsVisible = e.IsVisible
    };

    public static void Apply(this CertificateDto d, Certificate e)
    {
        e.Name = d.Name.Trim();
        e.Issuer = d.Issuer.Trim();
        e.IssueDate = d.IssueDate;
        e.ExpirationDate = d.ExpirationDate;
        e.CredentialId = d.CredentialId?.Trim();
        e.CredentialUrl = d.CredentialUrl?.Trim();
        e.DisplayOrder = d.DisplayOrder;
        e.IsVisible = d.IsVisible;
    }

    // ---------- Language ----------
    public static LanguageDto ToDto(this SpokenLanguage e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Level = e.Level,
        DisplayOrder = e.DisplayOrder,
        IsVisible = e.IsVisible
    };

    public static void Apply(this LanguageDto d, SpokenLanguage e)
    {
        e.Name = d.Name.Trim();
        e.Level = d.Level;
        e.DisplayOrder = d.DisplayOrder;
        e.IsVisible = d.IsVisible;
    }

    // ---------- Contact Message ----------
    public static ContactMessageDto ToDto(this ContactMessage e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Email = e.Email,
        Subject = e.Subject,
        Message = e.Message,
        IsRead = e.IsRead,
        CreatedAt = e.CreatedAt,
        ReadAt = e.ReadAt
    };
}
