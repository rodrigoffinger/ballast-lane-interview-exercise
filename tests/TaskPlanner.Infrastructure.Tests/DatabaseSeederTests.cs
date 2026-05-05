using TaskPlanner.Infrastructure.Persistence;
using TaskPlanner.Infrastructure.Persistence.Repositories;
using TaskPlanner.Infrastructure.Seeding;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class DatabaseSeederTests
{
    [Fact]
    public async Task SeedAsync_ShouldCreateDemoUserAndTasks_OnlyOnce()
    {
        using var database = TestDatabase.Create();
        await new SqliteDatabaseInitializer(database.ConnectionFactory).InitializeAsync();
        var users = new SqliteUserRepository(database.ConnectionFactory);
        var tasks = new SqliteTaskRepository(database.ConnectionFactory);
        var seeder = new DatabaseSeeder(users, tasks, new PasswordHasherStub());

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var demoUser = await users.FindByEmailAsync("demo@ballastlane.local");
        Assert.NotNull(demoUser);
        Assert.Equal("hashed:Demo123!", demoUser!.PasswordHash);

        var demoTasks = await tasks.ListByUserIdAsync(demoUser.Id);
        Assert.Equal(3, demoTasks.Count);
    }

    private sealed class PasswordHasherStub : TaskPlanner.Application.Abstractions.IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }
}
