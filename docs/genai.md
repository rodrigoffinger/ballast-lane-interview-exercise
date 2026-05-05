# GenAI Usage

This document describes how GenAI was used during the TaskPlanner implementation, how the output was validated, and what was corrected or improved.

## Goal

Use GenAI as a development assistant while keeping engineering judgment, validation, and code ownership with the developer.

The exercise asked for a RESTful API for a simple task management system with:

- Task CRUD.
- Task title.
- Task description.
- Task status.
- Task due date.
- User association.
- Authentication, validation, and edge case handling.

## Prompt Used

```text
You are helping implement a .NET 10 technical interview exercise.

Create a simple task management API using ASP.NET Core Web API controllers and Clean Architecture.

Requirements:
- Use C# and .NET 10.
- Use controllers, not Minimal APIs.
- Use a Domain project with User, TaskItem, and TaskStatus.
- Use an Application project with services/use cases, DTOs, validation, Result<T>, and repository interfaces.
- Use an Infrastructure project with SQLite repositories implemented through Microsoft.Data.Sqlite and manual parameterized SQL.
- Do not use Entity Framework, Dapper, or MediatR.
- Implement user registration and login.
- Use JWT authentication.
- Users must only access their own tasks.
- Implement CRUD endpoints for tasks.
- Use proper HTTP status codes.
- Return JSON responses with a consistent success/data/errors envelope except for 204 No Content.
- Include xUnit tests for Application, Infrastructure, and API endpoints.
- Include seeded demo credentials.
- Create and maintain Markdown documentation in /docs.
- Document the technical plan, architecture decisions, backlog, and progress log.
- Keep documentation updated as implementation phases are completed.

Focus on a simple, explainable implementation suitable for a technical interview.
```

## Representative AI Output

The representative scaffold suggested the following shape:

```text
src/
  TaskPlanner.Domain/
  TaskPlanner.Application/
  TaskPlanner.Infrastructure/
  TaskPlanner.Api/

tests/
  TaskPlanner.Application.Tests/
  TaskPlanner.Infrastructure.Tests/
  TaskPlanner.Api.Tests/
```

It also suggested these endpoint groups:

```http
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me

GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
```

And this response envelope:

```json
{
  "success": true,
  "data": {},
  "errors": []
}
```

It also produced representative controller and service code like this:

```csharp
[Authorize]
[Route("api/tasks")]
public sealed class TasksController : ApiControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await _taskService.CreateAsync(User.GetUserId(), request, cancellationToken);
        return CreatedFromResult(nameof(GetById), new { id = result.Value?.Id }, result);
    }
}
```

```csharp
public async Task<Result<TaskResponse>> CreateAsync(
    Guid userId,
    CreateTaskRequest request,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Result<TaskResponse>.Validation("Title is required.");
    }

    if (!Enum.IsDefined(request.Status))
    {
        return Result<TaskResponse>.Validation("Status is invalid.");
    }

    var task = new TaskItem(
        Guid.NewGuid(),
        userId,
        request.Title.Trim(),
        request.Description.Trim(),
        request.Status,
        request.DueDate,
        DateTimeOffset.UtcNow,
        DateTimeOffset.UtcNow);

    await _taskRepository.AddAsync(task, cancellationToken);
    return Result<TaskResponse>.Success(TaskResponse.FromTask(task));
}
```

These snippets are representative rather than copied blindly. The final implementation was reviewed and adjusted so that controllers stay thin, business rules stay in the Application layer, ownership is derived from JWT claims, and persistence remains behind repository interfaces.

## Validation Approach

The AI output was not accepted blindly. It was validated against:

- The original PDF requirements.
- Clean Architecture dependency direction.
- The restriction against Entity Framework, Dapper, and MediatR.
- HTTP semantics.
- Authentication and authorization requirements.
- Testability.
- Frontend integration needs.
- Local build and test results.

Documentation was also used as a validation tool. The technical plan, architecture decisions, backlog, and progress log kept the scope, implementation phases, decisions, and verification evidence traceable throughout the exercise.

Validation commands used during implementation:

```text
dotnet test TaskPlanner.slnx
```

```text
npm run build
```

```text
cmd /c run-dev.bat --no-run
```

```text
Browser smoke test through Chromium:
- Open the API-served SPA.
- Sign in with seeded demo credentials.
- Confirm the authenticated task UI renders.
- Confirm no browser console warnings, errors, or page errors are emitted.
```

## Corrections and Improvements

### Clean Architecture Boundaries

The generated architecture idea was kept, but the dependency direction was made explicit:

- Domain has no project dependencies.
- Application depends only on Domain.
- Infrastructure implements Application interfaces.
- API composes the application at runtime.

### Data Access

The data layer was implemented with manual SQL through `Microsoft.Data.Sqlite`.

Parameterized SQL was used throughout to avoid SQL injection risks. Entity Framework and Dapper were intentionally avoided.

### Task Ownership

The API and repository methods were designed around user-scoped access.

For example, task retrieval uses both task ID and user ID. This prevents users from reading, updating, or deleting another user's tasks.

When a task belongs to another user, the API returns `404 Not Found` rather than revealing that the task exists.

### Password Storage

Raw passwords are never stored.

The implementation uses PBKDF2-SHA256 with a per-password salt and fixed-time hash comparison.

### JWT Claims

The JWT includes:

- User ID.
- User email.
- User name.

The API uses the user ID claim to scope task operations.

### HTTP Status Codes

The response envelope does not replace HTTP semantics.

Examples:

- `201 Created` for registration and task creation.
- `200 OK` for successful reads, updates, and login.
- `204 No Content` for successful delete.
- `400 Bad Request` for validation errors.
- `401 Unauthorized` for missing or invalid authentication.
- `404 Not Found` for missing or non-owned tasks.
- `409 Conflict` for duplicate email registration.
- `500 Internal Server Error` for unexpected errors.

### Windows Batch Script Behavior

The initial `run-dev.bat` draft called `npm` directly. On Windows, `npm` resolves to `npm.cmd`, and a batch script must use `call npm ...` to return control to the parent script.

The script was corrected to use:

```bat
call npm install
call npm run build
```

## Edge Cases Handled

- Duplicate email registration.
- Invalid registration input.
- Invalid login credentials.
- Missing authentication token.
- Authenticated access to another user's task.
- Missing task.
- Task validation errors.
- SQLite temporary file cleanup during tests.
- SPA fallback when serving the frontend from the API.

## Final Position

GenAI was used as an accelerator for planning, scaffolding ideas, and implementation review. The final project was validated through tests, manual architectural review, and iterative corrections.

The most important decisions were made deliberately rather than copied from generated output.
