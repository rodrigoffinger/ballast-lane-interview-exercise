using FluentAssertions;
using TaskPlanner.Application.Abstractions;
using TaskPlanner.Application.Common;
using TaskPlanner.Application.Tasks;
using TaskPlanner.Domain.Tasks;
using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Application.Tests;

public sealed class TaskServiceTests
{
    private readonly FakeClock _clock = new(new DateTimeOffset(2026, 5, 5, 12, 0, 0, TimeSpan.Zero));
    private readonly FakeTaskRepository _tasks = new();

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenTitleIsMissing()
    {
        var service = CreateService();

        var result = await service.CreateAsync(Guid.NewGuid(), new CreateTaskRequest("", "Details", DomainTaskStatus.Todo, _clock.UtcNow.AddDays(1)));

        result.Status.Should().Be(ResultStatus.ValidationError);
        result.Errors.Should().Contain(error => error.Code == "TitleRequired");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTask_ForAuthenticatedUser()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var result = await service.CreateAsync(userId, new CreateTaskRequest("Prepare interview", "Review the project", DomainTaskStatus.InProgress, _clock.UtcNow.AddDays(1)));

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value.Title.Should().Be("Prepare interview");
        result.Value.Status.Should().Be(DomainTaskStatus.InProgress);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenTaskBelongsToAnotherUser()
    {
        var service = CreateService();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var created = await service.CreateAsync(ownerId, new CreateTaskRequest("Private task", "Hidden", DomainTaskStatus.Todo, _clock.UtcNow.AddDays(1)));

        var result = await service.GetByIdAsync(otherUserId, created.Value!.Id);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOnlyUsersOwnTask()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();
        var created = await service.CreateAsync(userId, new CreateTaskRequest("Draft", "Initial", DomainTaskStatus.Todo, _clock.UtcNow.AddDays(1)));

        var result = await service.UpdateAsync(userId, created.Value!.Id, new UpdateTaskRequest("Final", "Updated", DomainTaskStatus.Done, _clock.UtcNow.AddDays(2)));

        result.Status.Should().Be(ResultStatus.Success);
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Final");
        result.Value.Status.Should().Be(DomainTaskStatus.Done);
        result.Value.UpdatedAt.Should().Be(_clock.UtcNow);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenTaskDoesNotBelongToUser()
    {
        var service = CreateService();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var created = await service.CreateAsync(ownerId, new CreateTaskRequest("Private task", "Hidden", DomainTaskStatus.Todo, _clock.UtcNow.AddDays(1)));

        var result = await service.DeleteAsync(otherUserId, created.Value!.Id);

        result.Status.Should().Be(ResultStatus.NotFound);
    }

    private TaskService CreateService()
    {
        return new TaskService(_tasks, _clock);
    }

    private sealed class FakeTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = [];

        public Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            _tasks.Add(task);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            _tasks.Remove(task);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TaskItem>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<TaskItem>>(_tasks.Where(task => task.UserId == userId).ToList());
        }

        public Task<TaskItem?> GetByIdForUserAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_tasks.SingleOrDefault(task => task.Id == taskId && task.UserId == userId));
        }

        public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            var index = _tasks.FindIndex(existing => existing.Id == task.Id);
            if (index >= 0)
            {
                _tasks[index] = task;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class FakeClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }
}
