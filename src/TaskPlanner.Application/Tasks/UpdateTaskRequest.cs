using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Application.Tasks;

public sealed record UpdateTaskRequest(
    string Title,
    string Description,
    DomainTaskStatus Status,
    DateTimeOffset DueDate);
