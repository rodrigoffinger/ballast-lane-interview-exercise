namespace TaskPlanner.Domain.Tasks;

public sealed record TaskItem(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    TaskStatus Status,
    DateTimeOffset DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

