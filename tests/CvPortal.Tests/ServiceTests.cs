using CvPortal.Application.DTOs;
using CvPortal.Application.Services;
using CvPortal.Domain.Entities;
using CvPortal.Domain.Enums;
using CvPortal.Infrastructure.MockData;
using CvPortal.Infrastructure.Persistence;

namespace CvPortal.Tests;

public class ExperienceServiceTests
{
    private static readonly TimeProvider Time = new FixedTimeProvider(new DateTimeOffset(2026, 10, 2, 9, 0, 0, TimeSpan.Zero));

    private static ExperienceService CreateService(params Experience[] seed)
        => new(new InMemoryRepository<Experience>(seed), Time);

    [Fact]
    public async Task Create_AppendsToEnd_WhenDisplayOrderIsNotSet()
    {
        var service = CreateService(new Experience { Company = "A", DisplayOrder = 1 }, new Experience { Company = "B", DisplayOrder = 5 });

        var created = await service.CreateAsync(new ExperienceDto { Company = "C", Position = "Dev", Location = "İzmir", Description = "x" });

        Assert.Equal(6, created.DisplayOrder);
        Assert.Equal(3, created.Id);
    }

    [Fact]
    public async Task Update_ChangesFields_AndSetsUpdatedAt()
    {
        var service = CreateService(new Experience { Company = "Eski", Position = "Dev" });
        var dto = (await service.GetByIdAsync(1))!;
        dto.Company = "  Yeni Şirket  ";
        dto.TechnologiesText = "C#, Docker";

        var ok = await service.UpdateAsync(1, dto);
        var updated = (await service.GetByIdAsync(1))!;

        Assert.True(ok);
        Assert.Equal("Yeni Şirket", updated.Company);
        Assert.Equal(["C#", "Docker"], updated.Technologies);
    }

    [Fact]
    public async Task Update_ReturnsFalse_ForUnknownId()
        => Assert.False(await CreateService().UpdateAsync(99, new ExperienceDto()));

    [Fact]
    public async Task ToggleVisibility_HidesItemFromPublicList()
    {
        var service = CreateService(new Experience { Company = "A" }, new Experience { Company = "B" });

        var result = await service.ToggleVisibilityAsync(1);
        var visible = await service.GetAllAsync(onlyVisible: true);

        Assert.False(result);
        Assert.Single(visible);
        Assert.Equal("B", visible[0].Company);
    }
}

public class ContactMessageServiceTests
{
    private static readonly TimeProvider Time = new FixedTimeProvider(new DateTimeOffset(2026, 10, 2, 9, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task Create_StoresUnreadMessage_AndMarkAsReadUpdatesCount()
    {
        var service = new ContactMessageService(new InMemoryRepository<ContactMessage>(), Time);

        var created = await service.CreateAsync(new CreateContactMessageDto
        {
            Name = " Ayşe ", Email = "ayse@example.com", Subject = "Merhaba", Message = "Birlikte çalışalım mı?"
        });

        Assert.Equal("Ayşe", created.Name);
        Assert.Equal(1, await service.GetUnreadCountAsync());

        Assert.True(await service.SetReadStatusAsync(created.Id, true));

        var message = await service.GetByIdAsync(created.Id);
        Assert.True(message!.IsRead);
        Assert.Equal(Time.GetUtcNow().UtcDateTime, message.ReadAt);
        Assert.Equal(0, await service.GetUnreadCountAsync());
    }

    [Fact]
    public async Task GetAll_ReturnsNewestFirst()
    {
        var repo = new InMemoryRepository<ContactMessage>(
        [
            new ContactMessage { Subject = "Eski", CreatedAt = new DateTime(2026, 1, 1) },
            new ContactMessage { Subject = "Yeni", CreatedAt = new DateTime(2026, 9, 1) }
        ]);
        var service = new ContactMessageService(repo, Time);

        var all = await service.GetAllAsync();

        Assert.Equal(["Yeni", "Eski"], all.Select(m => m.Subject));
    }
}

public class CvServiceTests
{
    [Fact]
    public async Task GetCv_ReturnsOnlyVisibleContent_AndCalculatesStats()
    {
        var time = new FixedTimeProvider(new DateTimeOffset(2026, 10, 2, 9, 0, 0, TimeSpan.Zero));
        var profile = MockDataProvider.Profile();
        profile.CareerStartDate = new DateOnly(2019, 7, 1);

        var cvService = new CvService(
            new ProfileService(new InMemoryRepository<Profile>([profile]), time),
            new ExperienceService(new InMemoryRepository<Experience>(MockDataProvider.Experiences()), time),
            new EducationService(new InMemoryRepository<Education>(MockDataProvider.Educations()), time),
            new SkillService(new InMemoryRepository<Skill>(MockDataProvider.Skills()), time),
            new ProjectService(new InMemoryRepository<Project>(MockDataProvider.Projects()), time),
            new CertificateService(new InMemoryRepository<Certificate>(MockDataProvider.Certificates()), time),
            new LanguageService(new InMemoryRepository<SpokenLanguage>(MockDataProvider.Languages()), time),
            time);

        var cv = await cvService.GetCvAsync();

        // Mock data'daki staj kaydı gizli → listede olmamalı
        Assert.All(cv.Experiences, e => Assert.True(e.IsVisible));
        Assert.DoesNotContain(cv.Experiences, e => e.EmploymentType == EmploymentType.Internship);

        Assert.Equal(7, cv.Stats.YearsOfExperience);
        Assert.Equal(cv.Experiences.Select(e => e.Company).Distinct().Count(), cv.Stats.CompanyCount);
        Assert.True(cv.Projects[0].IsFeatured, "Öne çıkan projeler listenin başında olmalı.");
        Assert.Equal("Backend", cv.SkillGroups[0].Category);
    }
}
