using CvPortal.Domain.Entities;
using CvPortal.Domain.Enums;

namespace CvPortal.Infrastructure.MockData;

/// <summary>
/// Uygulama ilk açıldığında kullanılan örnek (kurgusal) CV verileri.
/// Tüm içerik admin panelden veya API üzerinden değiştirilebilir.
/// </summary>
public static class MockDataProvider
{
    private static DateOnly D(int year, int month) => new(year, month, 1);

    public static Profile Profile() => new()
    {
        Id = 1,
        FullName = "Deniz Yıldırım",
        Title = "Senior Software Engineer · .NET",
        Headline = "Ölçeklenebilir, test edilebilir ve sade backend sistemleri tasarlıyorum. .NET, bulut ve dağıtık mimariler üzerine 7+ yıllık deneyim.",
        About = """
                Merhaba! Ben Deniz. Yaklaşık yedi yıldır finans, lojistik ve e-ticaret alanlarında yüksek trafikli sistemler geliştiriyorum.
                Odak noktam; temiz mimari, domain-driven design ve ölçülebilir performans iyileştirmeleri.

                Ekip içinde kod incelemelerine, mentorluğa ve teknik dokümantasyona önem veririm. Monolitik yapıları adım adım
                mikroservislere taşıma, CI/CD süreçlerini kurma ve gözlemlenebilirlik (observability) altyapısı oluşturma konularında
                uçtan uca sorumluluk aldım. Boş zamanlarımda açık kaynak projelere katkı veriyor ve teknik blog yazıları yazıyorum.
                """,
        Email = "deniz.yildirim@example.com",
        Phone = "+90 555 000 00 00",
        Location = "İstanbul, Türkiye",
        AvatarUrl = "/images/avatar.svg",
        CvFileUrl = null,
        GitHubUrl = "https://github.com/example",
        LinkedInUrl = "https://www.linkedin.com/in/example",
        WebsiteUrl = "https://example.com",
        MediumUrl = "https://medium.com/@example",
        CareerStartDate = D(2019, 7),
        IsOpenToWork = true
    };

    public static List<Experience> Experiences() =>
    [
        new()
        {
            Company = "Nova Fintech",
            CompanyUrl = "https://example.com/nova",
            Position = "Senior Software Engineer",
            Location = "İstanbul",
            EmploymentType = EmploymentType.FullTime,
            WorkMode = WorkMode.Hybrid,
            StartDate = D(2023, 3),
            Description = "Ödeme ve cüzdan altyapısını geliştiren 8 kişilik platform ekibinde teknik liderlik.",
            Achievements =
            [
                "Monolitik ödeme servisini 6 mikroservise ayırarak deploy süresini 45 dk'dan 8 dk'ya indirdim.",
                "Outbox pattern ve RabbitMQ ile günde 2M+ işlemde mesaj kaybını sıfırladım.",
                "OpenTelemetry + Grafana ile uçtan uca izlenebilirlik altyapısı kurdum.",
                "3 junior geliştiriciye mentorluk yaptım; kod inceleme standartlarını yazdım."
            ],
            Technologies = [".NET 10", "ASP.NET Core", "PostgreSQL", "RabbitMQ", "Redis", "Kubernetes", "OpenTelemetry"],
            DisplayOrder = 1
        },
        new()
        {
            Company = "Atlas Lojistik Teknolojileri",
            Position = "Software Engineer",
            Location = "İstanbul",
            EmploymentType = EmploymentType.FullTime,
            WorkMode = WorkMode.OnSite,
            StartDate = D(2021, 1),
            EndDate = D(2023, 2),
            Description = "Filo takip ve rota optimizasyon platformunun backend geliştirmesi.",
            Achievements =
            [
                "SignalR ile 5.000+ aracın gerçek zamanlı konum takibini sağlayan servisi geliştirdim.",
                "EF Core sorgu optimizasyonlarıyla rapor ekranlarında %70 hız artışı sağladım.",
                "Azure DevOps üzerinde CI/CD pipeline'larını kurdum, test kapsamını %35'ten %80'e çıkardım."
            ],
            Technologies = ["C#", "ASP.NET Core", "SignalR", "SQL Server", "EF Core", "Azure", "Docker"],
            DisplayOrder = 2
        },
        new()
        {
            Company = "Pixel Yazılım",
            Position = "Junior .NET Developer",
            Location = "Ankara",
            EmploymentType = EmploymentType.FullTime,
            WorkMode = WorkMode.OnSite,
            StartDate = D(2019, 7),
            EndDate = D(2020, 12),
            Description = "E-ticaret müşterileri için B2B sipariş yönetim sistemleri ve entegrasyonlar.",
            Achievements =
            [
                "Pazaryeri entegrasyonları (REST/SOAP) için ortak bir adaptör kütüphanesi geliştirdim.",
                "Legacy WebForms modüllerini ASP.NET Core MVC'ye taşıyan ekipte yer aldım."
            ],
            Technologies = ["C#", "ASP.NET MVC", "SQL Server", "jQuery", "Bootstrap"],
            DisplayOrder = 3
        },
        new()
        {
            Company = "Kuzey Bilişim",
            Position = "Yazılım Stajyeri",
            Location = "Ankara",
            EmploymentType = EmploymentType.Internship,
            WorkMode = WorkMode.OnSite,
            StartDate = D(2018, 7),
            EndDate = D(2018, 9),
            Description = "İç kullanım için envanter takip uygulaması geliştirme.",
            Achievements = ["Ekipman zimmet takibi için küçük bir WinForms uygulaması geliştirdim."],
            Technologies = ["C#", "WinForms", "SQLite"],
            DisplayOrder = 4,
            IsVisible = false
        }
    ];

    public static List<Education> Educations() =>
    [
        new()
        {
            School = "Orta Doğu Teknik Üniversitesi",
            Degree = "Lisans",
            FieldOfStudy = "Bilgisayar Mühendisliği",
            StartDate = D(2015, 9),
            EndDate = D(2019, 6),
            Grade = "3.42 / 4.00",
            Description = "Bitirme projesi: Dağıtık sistemlerde tutarlılık modelleri üzerine simülasyon aracı.",
            DisplayOrder = 1
        },
        new()
        {
            School = "İstanbul Teknik Üniversitesi",
            Degree = "Yüksek Lisans",
            FieldOfStudy = "Bilgisayar Mühendisliği (Tezli)",
            StartDate = D(2024, 9),
            Grade = null,
            Description = "Araştırma alanı: Mikroservis mimarilerinde gözlemlenebilirlik.",
            DisplayOrder = 0
        }
    ];

    public static List<Skill> Skills() =>
    [
        // Backend
        new() { Name = "C#", Category = SkillCategory.Backend, Proficiency = 95, IconClass = "devicon-csharp-plain", DisplayOrder = 1 },
        new() { Name = "ASP.NET Core", Category = SkillCategory.Backend, Proficiency = 93, IconClass = "devicon-dotnetcore-plain", DisplayOrder = 2 },
        new() { Name = "Entity Framework Core", Category = SkillCategory.Backend, Proficiency = 88, IconClass = "bi bi-diagram-3", DisplayOrder = 3 },
        new() { Name = "gRPC", Category = SkillCategory.Backend, Proficiency = 72, IconClass = "bi bi-lightning-charge", DisplayOrder = 4 },
        new() { Name = "Python", Category = SkillCategory.Backend, Proficiency = 60, IconClass = "devicon-python-plain", DisplayOrder = 5 },
        // Frontend
        new() { Name = "TypeScript", Category = SkillCategory.Frontend, Proficiency = 75, IconClass = "devicon-typescript-plain", DisplayOrder = 1 },
        new() { Name = "React", Category = SkillCategory.Frontend, Proficiency = 70, IconClass = "devicon-react-original", DisplayOrder = 2 },
        new() { Name = "Blazor", Category = SkillCategory.Frontend, Proficiency = 78, IconClass = "devicon-blazor-original", DisplayOrder = 3 },
        // Database
        new() { Name = "PostgreSQL", Category = SkillCategory.Database, Proficiency = 85, IconClass = "devicon-postgresql-plain", DisplayOrder = 1 },
        new() { Name = "SQL Server", Category = SkillCategory.Database, Proficiency = 88, IconClass = "devicon-microsoftsqlserver-plain", DisplayOrder = 2 },
        new() { Name = "Redis", Category = SkillCategory.Database, Proficiency = 78, IconClass = "devicon-redis-plain", DisplayOrder = 3 },
        new() { Name = "MongoDB", Category = SkillCategory.Database, Proficiency = 62, IconClass = "devicon-mongodb-plain", DisplayOrder = 4 },
        // DevOps
        new() { Name = "Docker", Category = SkillCategory.DevOps, Proficiency = 86, IconClass = "devicon-docker-plain", DisplayOrder = 1 },
        new() { Name = "Kubernetes", Category = SkillCategory.DevOps, Proficiency = 72, IconClass = "devicon-kubernetes-plain", DisplayOrder = 2 },
        new() { Name = "Azure", Category = SkillCategory.DevOps, Proficiency = 76, IconClass = "devicon-azure-plain", DisplayOrder = 3 },
        new() { Name = "GitHub Actions", Category = SkillCategory.DevOps, Proficiency = 80, IconClass = "devicon-githubactions-plain", DisplayOrder = 4 },
        // Architecture
        new() { Name = "Clean Architecture", Category = SkillCategory.Architecture, Proficiency = 90, IconClass = "bi bi-layers", DisplayOrder = 1 },
        new() { Name = "Domain-Driven Design", Category = SkillCategory.Architecture, Proficiency = 82, IconClass = "bi bi-boxes", DisplayOrder = 2 },
        new() { Name = "Mikroservisler", Category = SkillCategory.Architecture, Proficiency = 85, IconClass = "bi bi-grid-3x3-gap", DisplayOrder = 3 },
        new() { Name = "Test Driven Development", Category = SkillCategory.Architecture, Proficiency = 80, IconClass = "bi bi-check2-circle", DisplayOrder = 4 },
        // Tools
        new() { Name = "Git", Category = SkillCategory.Tools, Proficiency = 92, IconClass = "devicon-git-plain", DisplayOrder = 1 },
        new() { Name = "Visual Studio", Category = SkillCategory.Tools, Proficiency = 94, IconClass = "devicon-visualstudio-plain", DisplayOrder = 2 },
        new() { Name = "RabbitMQ", Category = SkillCategory.Tools, Proficiency = 80, IconClass = "devicon-rabbitmq-original", DisplayOrder = 3 },
        // Soft skills
        new() { Name = "Takım Liderliği", Category = SkillCategory.SoftSkill, Proficiency = 82, IconClass = "bi bi-people", DisplayOrder = 1 },
        new() { Name = "Mentorluk", Category = SkillCategory.SoftSkill, Proficiency = 85, IconClass = "bi bi-mortarboard", DisplayOrder = 2 },
        new() { Name = "Teknik Yazarlık", Category = SkillCategory.SoftSkill, Proficiency = 78, IconClass = "bi bi-pencil-square", DisplayOrder = 3 }
    ];

    public static List<Project> Projects() =>
    [
        new()
        {
            Title = "PayFlow — Ödeme Orkestrasyon Servisi",
            Summary = "Birden fazla ödeme sağlayıcısını tek API arkasında birleştiren, saga pattern tabanlı orkestrasyon servisi.",
            Description = "Retry, idempotency ve circuit breaker desteğiyle sağlayıcı kesintilerinde otomatik yönlendirme yapar.",
            GitHubUrl = "https://github.com/example/payflow",
            Technologies = [".NET 10", "MassTransit", "PostgreSQL", "Polly", "Docker"],
            IsFeatured = true,
            CompletedAt = D(2025, 11),
            DisplayOrder = 1
        },
        new()
        {
            Title = "CvPortal",
            Summary = "Admin panel ve REST API ile yönetilebilen kişisel CV / portfolyo uygulaması (bu site!).",
            Description = "Clean Architecture, ASP.NET Core MVC, Cookie Auth, API Key korumalı REST API ve OpenAPI dokümantasyonu.",
            GitHubUrl = "https://github.com/example/cvportal",
            LiveUrl = "https://example.com",
            Technologies = [".NET 10", "ASP.NET Core MVC", "Bootstrap 5", "OpenAPI", "xUnit"],
            IsFeatured = true,
            CompletedAt = D(2026, 10),
            DisplayOrder = 2
        },
        new()
        {
            Title = "FleetPulse",
            Summary = "Araç filoları için gerçek zamanlı konum ve telemetri izleme panosu.",
            GitHubUrl = "https://github.com/example/fleetpulse",
            LiveUrl = "https://example.com/fleetpulse",
            Technologies = ["ASP.NET Core", "SignalR", "React", "Leaflet", "Redis"],
            IsFeatured = true,
            CompletedAt = D(2024, 5),
            DisplayOrder = 3
        },
        new()
        {
            Title = "Outbox.Light",
            Summary = "EF Core için hafif, bağımlılıksız Transactional Outbox kütüphanesi (NuGet paketi).",
            GitHubUrl = "https://github.com/example/outbox-light",
            Technologies = ["C#", "EF Core", "NuGet", "GitHub Actions"],
            CompletedAt = D(2025, 3),
            DisplayOrder = 4
        },
        new()
        {
            Title = "DevNotes CLI",
            Summary = "Terminalden markdown not almayı ve etiketlemeyi sağlayan çapraz platform CLI aracı.",
            GitHubUrl = "https://github.com/example/devnotes",
            Technologies = ["C#", "System.CommandLine", "SQLite"],
            CompletedAt = D(2023, 8),
            DisplayOrder = 5
        }
    ];

    public static List<Certificate> Certificates() =>
    [
        new()
        {
            Name = "Microsoft Certified: Azure Developer Associate (AZ-204)",
            Issuer = "Microsoft",
            IssueDate = D(2024, 4),
            ExpirationDate = D(2027, 4),
            CredentialId = "AZ204-XXXX-0000",
            CredentialUrl = "https://learn.microsoft.com",
            DisplayOrder = 1
        },
        new()
        {
            Name = "Certified Kubernetes Application Developer (CKAD)",
            Issuer = "The Linux Foundation",
            IssueDate = D(2023, 10),
            ExpirationDate = D(2025, 10),
            CredentialId = "LF-CKAD-0000",
            CredentialUrl = "https://training.linuxfoundation.org",
            DisplayOrder = 2
        },
        new()
        {
            Name = "Professional Scrum Master I (PSM I)",
            Issuer = "Scrum.org",
            IssueDate = D(2022, 6),
            CredentialUrl = "https://www.scrum.org",
            DisplayOrder = 3
        }
    ];

    public static List<SpokenLanguage> Languages() =>
    [
        new() { Name = "Türkçe", Level = LanguageLevel.Native, DisplayOrder = 1 },
        new() { Name = "İngilizce", Level = LanguageLevel.C1, DisplayOrder = 2 },
        new() { Name = "Almanca", Level = LanguageLevel.A2, DisplayOrder = 3 }
    ];

    public static List<ContactMessage> Messages() =>
    [
        new()
        {
            Name = "Elif Kaya",
            Email = "elif.kaya@example.com",
            Subject = "Senior Backend pozisyonu hakkında",
            Message = "Merhaba Deniz, ekibimizdeki Senior Backend Engineer pozisyonu için profilinizi çok uygun bulduk. Kısa bir görüşme ayarlayabilir miyiz?",
            CreatedAt = DateTime.UtcNow.AddHours(-5)
        },
        new()
        {
            Name = "Mert Demir",
            Email = "mert@example.com",
            Subject = "Outbox.Light kütüphanesi",
            Message = "Kütüphaneyi projemizde kullanıyoruz, çok işimize yaradı. PostgreSQL için LISTEN/NOTIFY desteği eklemeyi düşünüyor musunuz?",
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new()
        {
            Name = "Zeynep Arslan",
            Email = "zeynep.arslan@example.com",
            Subject = "Konferans konuşma daveti",
            Message = "Kasım ayında düzenleyeceğimiz .NET Meetup etkinliğinde mikroservislerde gözlemlenebilirlik üzerine bir konuşma yapmak ister misiniz?",
            CreatedAt = DateTime.UtcNow.AddDays(-6),
            IsRead = true,
            ReadAt = DateTime.UtcNow.AddDays(-5)
        }
    ];
}
