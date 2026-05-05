using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Application.Tasks;

public sealed record TaskResponse(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    DomainTaskStatus Status,
    DateTimeOffset DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
