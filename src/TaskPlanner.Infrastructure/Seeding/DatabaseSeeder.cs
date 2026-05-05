using TaskPlanner.Application.Abstractions;
using TaskPlanner.Domain.Tasks;
using TaskPlanner.Domain.Users;
using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Infrastructure.Seeding;

public sealed class DatabaseSeeder
{
    public const string DemoEmail = "demo@ballastlane.local";
    public const string DemoPassword = "Demo123!";

    private readonly IUserRepository _users;
    private readonly ITaskRepository _tasks;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(IUserRepository users, ITaskRepository tasks, IPasswordHasher passwordHasher)
    {
        _users = users;
        _tasks = tasks;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _users.EmailExistsAsync(DemoEmail, cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Demo User",
            DemoEmail,
            _passwordHasher.Hash(DemoPassword),
            now);

        await _users.AddAsync(user, cancellationToken);

        var seedTasks = new[]
        {
            new TaskItem(
                Guid.Parse("22222222-2222-2222-2222-222222222221"),
                user.Id,
                "Review project requirements",
                "Read the exercise brief and map each requirement to the implementation plan.",
                DomainTaskStatus.Done,
                now.AddDays(1),
                now,
                now),
            new TaskItem(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                user.Id,
                "Implement secured task API",
                "Build authenticated CRUD endpoints that only expose the current user's tasks.",
                DomainTaskStatus.InProgress,
                now.AddDays(2),
                now,
                now),
            new TaskItem(
                Guid.Parse("22222222-2222-2222-2222-222222222223"),
                user.Id,
                "Prepare frontend demo",
                "Create a clean React UI for login, registration, and task management.",
                DomainTaskStatus.Todo,
                now.AddDays(3),
                now,
                now)
        };

        foreach (var task in seedTasks)
        {
            await _tasks.AddAsync(task, cancellationToken);
        }
    }
}

