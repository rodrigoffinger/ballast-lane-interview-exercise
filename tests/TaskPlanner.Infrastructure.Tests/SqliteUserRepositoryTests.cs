using FluentAssertions;
using TaskPlanner.Domain.Users;
using TaskPlanner.Infrastructure.Persistence;
using TaskPlanner.Infrastructure.Persistence.Repositories;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class SqliteUserRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistUser_AndFindByEmail()
    {
        using var database = TestDatabase.Create();
        await new SqliteDatabaseInitializer(database.ConnectionFactory).InitializeAsync();
        var repository = new SqliteUserRepository(database.ConnectionFactory);
        var user = new User(Guid.NewGuid(), "Demo User", "demo@ballastlane.local", "hash", DateTimeOffset.UtcNow);

        await repository.AddAsync(user);

        var savedUser = await repository.FindByEmailAsync("DEMO@ballastlane.local");

        savedUser.Should().NotBeNull();
        savedUser!.Id.Should().Be(user.Id);
        savedUser.Email.Should().Be("demo@ballastlane.local");
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        using var database = TestDatabase.Create();
        await new SqliteDatabaseInitializer(database.ConnectionFactory).InitializeAsync();
        var repository = new SqliteUserRepository(database.ConnectionFactory);
        await repository.AddAsync(new User(Guid.NewGuid(), "Demo User", "demo@ballastlane.local", "hash", DateTimeOffset.UtcNow));

        var exists = await repository.EmailExistsAsync("demo@ballastlane.local");

        exists.Should().BeTrue();
    }
}

