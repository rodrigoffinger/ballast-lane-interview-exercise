using TaskPlanner.Domain.Tasks;

namespace TaskPlanner.Application.Abstractions;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<TaskItem?> GetByIdForUserAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default);

    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default);
}

