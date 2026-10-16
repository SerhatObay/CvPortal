using CvPortal.Application.Services;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Domain.Entities;
using CvPortal.Infrastructure.MockData;
using CvPortal.Infrastructure.Persistence;
using CvPortal.Web.Security;
using CvPortal.Web.Setup;
using Microsoft.Extensions.Logging.Abstractions;

namespace CvPortal.Tests;

public class SiteSettingsServiceTests
{
    private static readonly TimeProvider Time = new FixedTimeProvider(new DateTimeOffset(2026, 10, 16, 9, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task ApiKey_IsRejectedWhileApiDisabled_AndAcceptedAfterEnabling()
    {
        var service = new SiteSettingsService(new InMemoryRepository<SiteSettings>(), Time);
        var key = (await service.GetAsync()).ApiKey;

        Assert.StartsWith("cvp_", key);
        Assert.Equal(ApiKeyValidationResult.ApiDisabled, await service.ValidateApiKeyAsync(key));

        await service.SetApiEnabledAsync(true);

        Assert.Equal(ApiKeyValidationResult.Valid, await service.ValidateApiKeyAsync(key));
        Assert.Equal(ApiKeyValidationResult.Invalid, await service.ValidateApiKeyAsync(key + "x"));
        Assert.Equal(ApiKeyValidationResult.Missing, await service.ValidateApiKeyAsync(""));
    }

    [Fact]
    public async Task RegenerateApiKey_InvalidatesOldKey()
    {
        var service = new SiteSettingsService(new InMemoryRepository<SiteSettings>(), Time);
        await service.SetApiEnabledAsync(true);
        var oldKey = (await service.GetAsync()).ApiKey;

        var newKey = await service.RegenerateApiKeyAsync();

        Assert.NotEqual(oldKey, newKey);
        Assert.Equal(ApiKeyValidationResult.Invalid, await service.ValidateApiKeyAsync(oldKey));
        Assert.Equal(ApiKeyValidationResult.Valid, await service.ValidateApiKeyAsync(newKey));
    }
}

public class AdminAuthServiceTests
{
    private static readonly TimeProvider Time = new FixedTimeProvider(new DateTimeOffset(2026, 10, 16, 9, 0, 0, TimeSpan.Zero));

    [Fact]
    public async Task CreateAdmin_StoresHashNotPlainText_AndPasswordCanBeChanged()
    {
        var repo = new InMemoryRepository<AdminUser>();
        var auth = new AdminAuthService(repo, Time);

        var user = await auth.CreateAdminAsync("admin", "a@example.com", "Ayşe", "Sifre1234");

        Assert.DoesNotContain("Sifre1234", user.PasswordHash);
        Assert.False(await auth.ChangePasswordAsync(user.Id, "yanlis-sifre", "YeniSifre99"));
        Assert.True(await auth.ChangePasswordAsync(user.Id, "Sifre1234", "YeniSifre99"));
        Assert.True(await auth.ResetPasswordAsync("admin", "BaskaSifre42"));
        Assert.False(await auth.ResetPasswordAsync("olmayan", "BaskaSifre42"));
    }
}

public class SetupServiceTests
{
    private static readonly TimeProvider Time = new FixedTimeProvider(new DateTimeOffset(2026, 10, 16, 9, 0, 0, TimeSpan.Zero));

    private sealed record Sut(SetupService Setup, InMemoryRepository<Experience> Experiences, ProfileService Profiles, SiteSettingsService Settings);

    private static Sut Create()
    {
        var experiences = new InMemoryRepository<Experience>(MockDataProvider.Experiences());
        var profiles = new ProfileService(new InMemoryRepository<Profile>([MockDataProvider.Profile()]), Time);
        var settings = new SiteSettingsService(new InMemoryRepository<SiteSettings>(), Time);
        var reset = new ContentResetService(experiences,
            new InMemoryRepository<Education>(), new InMemoryRepository<Skill>(), new InMemoryRepository<Project>(),
            new InMemoryRepository<Certificate>(), new InMemoryRepository<SpokenLanguage>(), new InMemoryRepository<ContactMessage>());
        var auth = new AdminAuthService(new InMemoryRepository<AdminUser>(), Time);

        return new Sut(new SetupService(auth, settings, profiles, reset, NullLogger<SetupService>.Instance), experiences, profiles, settings);
    }

    private static SetupViewModel Model(bool startEmpty) => new()
    {
        FullName = "Ali Veli", Title = "Backend Developer", Email = "ali@example.com",
        Username = "ali", Password = "Sifre1234", ConfirmPassword = "Sifre1234",
        StartEmpty = startEmpty, EnableApi = true
    };

    [Fact]
    public async Task Complete_KeepsDemoContent_ButAppliesOwnerIdentity()
    {
        var sut = Create();

        var admin = await sut.Setup.CompleteAsync(Model(startEmpty: false));

        Assert.NotNull(admin);
        Assert.True(await sut.Setup.IsCompletedAsync());
        Assert.True((await sut.Settings.GetAsync()).IsApiEnabled);
        Assert.Equal(4, await sut.Experiences.CountAsync());

        var profile = await sut.Profiles.GetAsync();
        Assert.Equal("Ali Veli", profile.FullName);
        Assert.Equal("ali@example.com", profile.Email);
    }

    [Fact]
    public async Task Complete_StartEmpty_ClearsContent()
    {
        var sut = Create();

        await sut.Setup.CompleteAsync(Model(startEmpty: true));

        Assert.Equal(0, await sut.Experiences.CountAsync());
        Assert.Equal("Backend Developer", (await sut.Profiles.GetAsync()).Title);
    }

    [Fact]
    public async Task Complete_CanOnlyRunOnce()
    {
        var sut = Create();

        Assert.NotNull(await sut.Setup.CompleteAsync(Model(false)));
        Assert.Null(await sut.Setup.CompleteAsync(Model(true)));
        Assert.Equal(4, await sut.Experiences.CountAsync());
    }
}
