# TaskPlanner Progress Log

This log records important implementation steps, decisions, and verification results. It should be updated throughout the project so the final submission shows a clear engineering process.

## 2026-05-05

### Initial Exercise Review

Reviewed the technical interview PDF located at:

```text
docs/Net - BLA - Technical Interview Exercise - V5.pdf
```

Key exercise requirements identified:

- Build a .NET web application with an API and data layer.
- Use Clean Architecture principles.
- Use Test-Driven Development.
- Provide CRUD operations through API endpoints.
- Add user creation and login.
- Store users and application data.
- Include authorized and non-authorized endpoints.
- Build a responsive frontend with a framework of choice.
- Include seeded data and demo credentials.
- Avoid Entity Framework, Dapper, and MediatR.
- Include README documentation.
- Include a GenAI prompt, representative output, validation notes, corrections, and edge case handling.

### Scope Alignment

Confirmed the following implementation scope:

- Domain: task management.
- Application name: TaskPlanner.
- Main entity name: `TaskItem`.
- Backend: .NET 10 with ASP.NET Core Web API.
- API style: controllers.
- Database: SQLite with manual SQL.
- Persistence boundary: Repository Pattern.
- Authentication: JWT.
- Authorization rule: users can only access their own tasks.
- Frontend: React, Vite, TypeScript, and Tailwind CSS.
- Demo approach: build frontend into API `wwwroot` and run from a single endpoint.
- Development helper: `run-dev.bat`.
- Docker: excluded from initial scope.
- Documentation language: English.
- Code comments, method names, entities, and project naming: English.

### Architecture Planning

Created initial architecture documentation:

- `docs/technical-plan.md`
- `docs/architecture-decisions.md`
- `docs/backlog.md`
- `docs/progress-log.md`

No application code had been implemented at this point.

### Phase 1 Foundation Completed

Created the .NET solution and project structure:

```text
TaskPlanner.slnx

src/
  TaskPlanner.Api/
  TaskPlanner.Application/
  TaskPlanner.Domain/
  TaskPlanner.Infrastructure/

tests/
  TaskPlanner.Api.Tests/
  TaskPlanner.Application.Tests/
  TaskPlanner.Infrastructure.Tests/
```

Configured project references to preserve Clean Architecture dependency direction:

- `TaskPlanner.Domain` has no project dependencies.
- `TaskPlanner.Application` depends on `TaskPlanner.Domain`.
- `TaskPlanner.Infrastructure` depends on `TaskPlanner.Application` and `TaskPlanner.Domain`.
- `TaskPlanner.Api` depends on `TaskPlanner.Application` and `TaskPlanner.Infrastructure`.
- Test projects reference the projects they are intended to verify.

Removed default template artifacts:

- `Class1.cs` files from class library projects.
- `UnitTest1.cs` files from test projects.
- `WeatherForecast` API example files from the Web API template.

Added initial test dependencies:

- `FluentAssertions`
- `Microsoft.AspNetCore.Mvc.Testing` for API integration tests

Added one smoke test per test project to verify test discovery and assembly load behavior. These are intentionally minimal and do not represent business behavior coverage.

### Tooling Notes

The .NET 10 SDK generated a `TaskPlanner.slnx` solution file instead of a traditional `.sln` file.

Package installation initially failed because a configured NuGet source at `https://nuget.digitaldesk.com.br/DataServices/Packages.svc` was unreachable. Package installation succeeded when explicitly using `https://api.nuget.org/v3/index.json`.

### Verification

Ran:

```text
dotnet build TaskPlanner.slnx
```

Result:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Ran:

```text
dotnet test TaskPlanner.slnx
```

Result:

```text
Passed: 3
Failed: 0
```

### Current Status

Phase 2 Domain and Application setup is complete. The next planned step is Phase 3: implement SQLite-backed infrastructure with repository tests first.

### Phase 2 Domain and Application TDD Completed

Started Phase 2 by writing failing Application tests before implementing the production code. The initial test run failed because the planned domain types, application contracts, DTOs, result model, and services did not exist yet.

Implemented domain model:

- `User`
- `TaskItem`
- `TaskStatus`

Implemented application abstractions:

- `IUserRepository`
- `ITaskRepository`
- `IPasswordHasher`
- `ITokenService`
- `IClock`

Implemented application support types:

- `Result`
- `Result<T>`
- `ResultStatus`
- `ResultError`

Implemented authentication use cases:

- Register user.
- Reject invalid registration input.
- Reject duplicate email registration.
- Hash passwords through an abstraction.
- Login user.
- Reject invalid login credentials.
- Return access token through an abstraction.

Implemented task use cases:

- Create task.
- List user tasks.
- Get task by ID scoped to the authenticated user.
- Update task scoped to the authenticated user.
- Delete task scoped to the authenticated user.
- Reject invalid task title.
- Reject invalid task status.

### Phase 2 Design Notes

The application layer uses a result pattern for expected failures instead of throwing exceptions for validation, duplicate email, not found, and unauthorized outcomes. This keeps controller mapping straightforward later and keeps the application layer independent from ASP.NET Core.

The task entity is named `TaskItem` to avoid ambiguity with `System.Threading.Tasks.Task`. The task status enum intentionally remains named `TaskStatus`, so Application code uses explicit namespace aliases where necessary to avoid conflicts with `System.Threading.Tasks.TaskStatus`.

### Phase 2 Verification

Ran:

```text
dotnet test tests\TaskPlanner.Application.Tests\TaskPlanner.Application.Tests.csproj
```

Result:

```text
Passed: 11
Failed: 0
```

Ran:

```text
dotnet test TaskPlanner.slnx
```

Result:

```text
Passed: 13
Failed: 0
```
