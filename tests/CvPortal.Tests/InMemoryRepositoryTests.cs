using CvPortal.Domain.Entities;
using CvPortal.Infrastructure.Persistence;

namespace CvPortal.Tests;

public class InMemoryRepositoryTests
{
    [Fact]
    public async Task Seed_AssignsSequentialIds()
    {
        var repo = new InMemoryRepository<Skill>([new Skill { Name = "C#" }, new Skill { Name = "Go" }]);

        var all = await repo.GetAllAsync();

        Assert.Equal([1, 2], all.Select(s => s.Id).Order());
    }

    [Fact]
    public async Task Add_ContinuesFromHighestId()
    {
        var repo = new InMemoryRepository<Skill>([new Skill { Id = 10, Name = "C#" }]);

        var added = await repo.AddAsync(new Skill { Name = "Rust" });

        Assert.Equal(11, added.Id);
        Assert.Equal(2, await repo.CountAsync());
    }

    [Fact]
    public async Task Delete_RemovesEntity_AndReturnsFalseWhenMissing()
    {
        var repo = new InMemoryRepository<Skill>([new Skill { Name = "C#" }]);

        Assert.True(await repo.DeleteAsync(1));
        Assert.False(await repo.DeleteAsync(1));
        Assert.Null(await repo.GetByIdAsync(1));
    }

    [Fact]
    public async Task Update_ReturnsFalseForUnknownEntity()
    {
        var repo = new InMemoryRepository<Skill>();
        Assert.False(await repo.UpdateAsync(new Skill { Id = 42 }));
    }
}
