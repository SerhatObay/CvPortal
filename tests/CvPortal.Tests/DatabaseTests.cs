using CvPortal.Application.Services;
using CvPortal.Domain.Entities;
using CvPortal.Domain.Enums;
using CvPortal.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CvPortal.Tests;

/// <summary>
/// EF Core katmanını gerçek bir ilişkisel veritabanına (bellek içi SQLite) karşı test eder.
/// SQL Server'a özgü migration'lar yerine EnsureCreated ile şema kurulur.
/// </summary>
public sealed class DatabaseTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private static readonly TimeProvider Time = new FixedTimeProvider(new DateTimeOffset(2026, 10, 16, 9, 0, 0, TimeSpan.Zero));

    public DatabaseTests() => _connection.Open();

    public void Dispose() => _connection.Dispose();

    private CvPortalDbContext CreateContext()
    {
        var db = new CvPortalDbContext(new DbContextOptionsBuilder<CvPortalDbContext>().UseSqlite(_connection).Options);
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public async Task SeedDemoData_AddsContentOnce()
    {
        await using var db = CreateContext();

        await DatabaseInitializer.SeedDemoDataAsync(db, NullLogger.Instance, default);
        await DatabaseInitializer.SeedDemoDataAsync(db, NullLogger.Instance, default);

        Assert.Equal(1, await db.Profiles.CountAsync());
        Assert.Equal(4, await db.Experiences.CountAsync());
        Assert.True(await db.Skills.CountAsync() > 10);
        Assert.Equal(0, await db.ContactMessages.CountAsync());
    }

    [Fact]
    public async Task EfRepository_PersistsListsAndEnums()
    {
        await using (var db = CreateContext())
        {
            var service = new ExperienceService(new EfRepository<Experience>(db), Time);
            await service.CreateAsync(new()
            {
                Company = "Acme", Position = "Dev", Location = "İzmir", Description = "x",
                EmploymentType = EmploymentType.Contract, TechnologiesText = "C#, SQL Server"
            });
        }

        // Yeni bir context ile okuyarak verinin gerçekten veritabanına yazıldığını doğrula
        await using (var db = CreateContext())
        {
            var saved = await db.Experiences.SingleAsync();
            Assert.Equal(["C#", "SQL Server"], saved.Technologies);
            Assert.Equal(EmploymentType.Contract, saved.EmploymentType);
            Assert.Equal(1, saved.DisplayOrder);
        }
    }

    [Fact]
    public async Task EfRepository_UpdateDeleteAndDeleteAll()
    {
        await using var db = CreateContext();
        var repo = new EfRepository<Skill>(db);
        var a = await repo.AddAsync(new Skill { Name = "C#", Category = SkillCategory.Backend, Proficiency = 90 });
        await repo.AddAsync(new Skill { Name = "Go", Category = SkillCategory.Backend, Proficiency = 50 });

        a.Proficiency = 95;
        Assert.True(await repo.UpdateAsync(a));
        Assert.Equal(1, await repo.CountAsync(s => s.Proficiency > 90));

        Assert.True(await repo.DeleteAsync(a.Id));
        Assert.False(await repo.DeleteAsync(a.Id));

        Assert.Equal(1, await repo.DeleteAllAsync());
        Assert.False(await repo.AnyAsync());
    }

    [Fact]
    public async Task UniqueUsername_IsEnforcedByDatabase()
    {
        await using var db = CreateContext();
        db.AdminUsers.Add(new AdminUser { Username = "admin", Email = "a@x.com", DisplayName = "A", PasswordHash = "h" });
        await db.SaveChangesAsync();

        db.AdminUsers.Add(new AdminUser { Username = "admin", Email = "b@x.com", DisplayName = "B", PasswordHash = "h" });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}
