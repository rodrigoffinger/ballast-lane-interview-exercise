using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Application.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string Description,
    DomainTaskStatus Status,
    DateTimeOffset DueDate);
