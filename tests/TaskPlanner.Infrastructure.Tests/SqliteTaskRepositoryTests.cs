using FluentAssertions;
using TaskPlanner.Domain.Tasks;
using TaskPlanner.Domain.Users;
using TaskPlanner.Infrastructure.Persistence;
using TaskPlanner.Infrastructure.Persistence.Repositories;
using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Infrastructure.Tests;

public sealed class SqliteTaskRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistTask_AndListOnlyUsersTasks()
    {
        using var database = TestDatabase.Create();
        await new SqliteDatabaseInitializer(database.ConnectionFactory).InitializeAsync();
        var users = new SqliteUserRepository(database.ConnectionFactory);
        var tasks = new SqliteTaskRepository(database.ConnectionFactory);
        var owner = new User(Guid.NewGuid(), "Owner", "owner@ballastlane.local", "hash", DateTimeOffset.UtcNow);
        var otherUser = new User(Guid.NewGuid(), "Other", "other@ballastlane.local", "hash", DateTimeOffset.UtcNow);
        await users.AddAsync(owner);
        await users.AddAsync(otherUser);

        await tasks.AddAsync(CreateTask(owner.Id, "Owner task"));
        await tasks.AddAsync(CreateTask(otherUser.Id, "Other task"));

        var ownerTasks = await tasks.ListByUserIdAsync(owner.Id);

        ownerTasks.Should().ContainSingle();
        ownerTasks[0].Title.Should().Be("Owner task");
    }

    [Fact]
    public async Task GetByIdForUserAsync_ShouldReturnNull_WhenTaskBelongsToAnotherUser()
    {
        using var database = TestDatabase.Create();
        await new SqliteDatabaseInitializer(database.ConnectionFactory).InitializeAsync();
        var users = new SqliteUserRepository(database.ConnectionFactory);
        var tasks = new SqliteTaskRepository(database.ConnectionFactory);
        var owner = new User(Guid.NewGuid(), "Owner", "owner@ballastlane.local", "hash", DateTimeOffset.UtcNow);
        var otherUser = new User(Guid.NewGuid(), "Other", "other@ballastlane.local", "hash", DateTimeOffset.UtcNow);
        await users.AddAsync(owner);
        await users.AddAsync(otherUser);
        var task = CreateTask(owner.Id, "Private task");
        await tasks.AddAsync(task);

        var result = await tasks.GetByIdForUserAsync(otherUser.Id, task.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        using var database = TestDatabase.Create();
        await new SqliteDatabaseInitializer(database.ConnectionFactory).InitializeAsync();
        var users = new SqliteUserRepository(database.ConnectionFactory);
        var tasks = new SqliteTaskRepository(database.ConnectionFactory);
        var owner = new User(Guid.NewGuid(), "Owner", "owner@ballastlane.local", "hash", DateTimeOffset.UtcNow);
        await users.AddAsync(owner);
        var task = CreateTask(owner.Id, "Draft");
        await tasks.AddAsync(task);

        await tasks.UpdateAsync(task with { Title = "Final", Status = DomainTaskStatus.Done });

        var updated = await tasks.GetByIdForUserAsync(owner.Id, task.Id);
        updated.Should().NotBeNull();
        updated!.Title.Should().Be("Final");
        updated.Status.Should().Be(DomainTaskStatus.Done);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTask()
    {
        using var database = TestDatabase.Create();
        await new SqliteDatabaseInitializer(database.ConnectionFactory).InitializeAsync();
        var users = new SqliteUserRepository(database.ConnectionFactory);
        var tasks = new SqliteTaskRepository(database.ConnectionFactory);
        var owner = new User(Guid.NewGuid(), "Owner", "owner@ballastlane.local", "hash", DateTimeOffset.UtcNow);
        await users.AddAsync(owner);
        var task = CreateTask(owner.Id, "Draft");
        await tasks.AddAsync(task);

        await tasks.DeleteAsync(task);

        var deleted = await tasks.GetByIdForUserAsync(owner.Id, task.Id);
        deleted.Should().BeNull();
    }

    private static TaskItem CreateTask(Guid userId, string title)
    {
        var now = DateTimeOffset.UtcNow;
        return new TaskItem(Guid.NewGuid(), userId, title, "Description", DomainTaskStatus.Todo, now.AddDays(1), now, now);
    }
}

