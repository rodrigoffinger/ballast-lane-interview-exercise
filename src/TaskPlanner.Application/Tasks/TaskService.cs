using TaskPlanner.Application.Abstractions;
using TaskPlanner.Application.Common;
using TaskPlanner.Domain.Tasks;
using DomainTaskStatus = TaskPlanner.Domain.Tasks.TaskStatus;

namespace TaskPlanner.Application.Tasks;

public sealed class TaskService
{
    private readonly ITaskRepository _tasks;

    public TaskService(ITaskRepository tasks)
    {
        _tasks = tasks;
    }

    public async Task<Result<TaskResponse>> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var errors = ValidateTask(request.Title, request.Status);
        if (errors.Count > 0)
        {
            return Result<TaskResponse>.Failure(ResultStatus.ValidationError, errors);
        }

        var now = DateTimeOffset.UtcNow;
        var task = new TaskItem(
            Guid.NewGuid(),
            userId,
            request.Title.Trim(),
            request.Description.Trim(),
            request.Status,
            request.DueDate,
            now,
            now);

        await _tasks.AddAsync(task, cancellationToken);

        return Result<TaskResponse>.Success(Map(task));
    }

    public async Task<Result<IReadOnlyList<TaskResponse>>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tasks = await _tasks.ListByUserIdAsync(userId, cancellationToken);
        return Result<IReadOnlyList<TaskResponse>>.Success(tasks.Select(Map).ToList());
    }

    public async Task<Result<TaskResponse>> GetByIdAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdForUserAsync(userId, taskId, cancellationToken);
        if (task is null)
        {
            return NotFound();
        }

        return Result<TaskResponse>.Success(Map(task));
    }

    public async Task<Result<TaskResponse>> UpdateAsync(Guid userId, Guid taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var errors = ValidateTask(request.Title, request.Status);
        if (errors.Count > 0)
        {
            return Result<TaskResponse>.Failure(ResultStatus.ValidationError, errors);
        }

        var existing = await _tasks.GetByIdForUserAsync(userId, taskId, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        var updated = existing with
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Status = request.Status,
            DueDate = request.DueDate,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _tasks.UpdateAsync(updated, cancellationToken);

        return Result<TaskResponse>.Success(Map(updated));
    }

    public async Task<Result> DeleteAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        var existing = await _tasks.GetByIdForUserAsync(userId, taskId, cancellationToken);
        if (existing is null)
        {
            return Result.Failure(
                ResultStatus.NotFound,
                [new ResultError("TaskNotFound", "The task was not found.")]);
        }

        await _tasks.DeleteAsync(existing, cancellationToken);

        return Result.Success();
    }

    private static List<ResultError> ValidateTask(string title, DomainTaskStatus status)
    {
        var errors = new List<ResultError>();

        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add(new ResultError("TitleRequired", "Title is required."));
        }

        if (!Enum.IsDefined(status))
        {
            errors.Add(new ResultError("InvalidTaskStatus", "Task status is invalid."));
        }

        return errors;
    }

    private static Result<TaskResponse> NotFound()
    {
        return Result<TaskResponse>.Failure(
            ResultStatus.NotFound,
            [new ResultError("TaskNotFound", "The task was not found.")]);
    }

    private static TaskResponse Map(TaskItem task)
    {
        return new TaskResponse(
            task.Id,
            task.UserId,
            task.Title,
            task.Description,
            task.Status,
            task.DueDate,
            task.CreatedAt,
            task.UpdatedAt);
    }
}
