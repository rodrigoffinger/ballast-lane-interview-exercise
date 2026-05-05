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

README and GenAI documentation are complete. The next planned step is final polish and optional presentation notes.

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

### Phase 3 Infrastructure Completed

Implemented SQLite-backed infrastructure with tests covering schema creation, repositories, password hashing, JWT creation, and seed data.

Implemented persistence components:

- `SqliteConnectionFactory`
- `SqliteDatabaseInitializer`
- `SqliteUserRepository`
- `SqliteTaskRepository`

Implemented infrastructure services:

- `PasswordHasher`
- `JwtOptions`
- `JwtTokenService`
- `SystemClock`
- `DatabaseSeeder`

Repository behavior covered by tests:

- Schema initialization creates `users` and `tasks`.
- User repository persists users and finds users by case-insensitive email.
- User repository checks whether an email exists.
- Task repository persists tasks.
- Task repository lists only the requested user's tasks.
- Task repository does not return another user's task.
- Task repository updates task data.
- Task repository deletes tasks.

Security behavior covered by tests:

- Password hashing does not store the raw password.
- Password verification succeeds for the matching password.
- Password verification fails for the wrong password.
- JWT generation includes user ID, email, and name claims.

Seed behavior covered by tests:

- Demo user is created with the expected email.
- Demo tasks are created for the demo user.
- Running the seeder twice does not duplicate demo data.

### Phase 3 Design Notes

SQLite access uses manual, parameterized SQL through `Microsoft.Data.Sqlite`. No Entity Framework or Dapper is used.

The repository boundary remains in the Application layer, so the SQLite implementation can be replaced later without changing use cases.

The password hasher uses PBKDF2-SHA256 with a per-password salt and fixed-time hash comparison.

The test SQLite connection string disables pooling for temporary database files to avoid Windows file-lock cleanup issues during tests.

### Phase 3 Verification

Ran:

```text
dotnet test tests\TaskPlanner.Infrastructure.Tests\TaskPlanner.Infrastructure.Tests.csproj
```

Result:

```text
Passed: 12
Failed: 0
```

### Phase 4 API Completed

Started Phase 4 by writing API integration tests before implementing the endpoints. The first test run returned `404 Not Found` for planned routes, confirming the endpoints did not exist yet.

Implemented API components:

- `ApiResponse<T>` response envelope.
- `ApiError` error model.
- `ExceptionHandlingMiddleware`.
- `AuthController`.
- `TasksController`.
- `HealthController`.
- JWT authentication setup.
- Authorization setup.
- Application and Infrastructure dependency injection.
- Database initialization and demo seeding during startup.

Implemented endpoints:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`
- `GET /api/health`
- `GET /api/health/secure`

API behavior covered by integration tests:

- Registering a new user returns `201 Created`.
- Registering an existing email returns `409 Conflict`.
- Logging in with the seeded demo user returns a JWT.
- Accessing tasks without a token returns `401 Unauthorized`.
- Authenticated task create, list, update, delete, and not-found after delete work as expected.
- Public health endpoint returns `200 OK`.
- Secure health endpoint requires authentication.

### Phase 4 Design Notes

HTTP status codes remain authoritative. The response envelope provides a consistent frontend contract for JSON responses, but `204 No Content` responses intentionally do not include a body.

Task endpoints derive the current user ID from JWT claims and call user-scoped Application methods, preserving the ownership rule at the API boundary.

The API test factory uses a temporary SQLite database with `Pooling=False`, initializes schema, and seeds demo data through the same startup path used by the application.

### Phase 4 Verification

Ran:

```text
dotnet test tests\TaskPlanner.Api.Tests\TaskPlanner.Api.Tests.csproj
```

Result:

```text
Passed: 7
Failed: 0
```

Ran:

```text
dotnet test TaskPlanner.slnx
```

Result:

```text
Passed: 30
Failed: 0
```

### Final Smoke Test

Started the API locally after building and copying the frontend into `wwwroot`, then verified:

- `GET /` returned `200 OK`.
- `GET /api/health` returned `200 OK`.
- `POST /api/auth/login` with the seeded demo user returned `200 OK`.
- The login response included an access token.

Smoke test result:

```text
IndexStatus: 200
HealthStatus: 200
LoginStatus: 200
LoginHasToken: True
```

Also checked source and documentation files for obvious template leftovers and Portuguese terms. No issues were found in the searched files.

### Documentation Completed

Created the root README with:

- Project overview.
- User story.
- Tech stack.
- Architecture overview.
- Demo credentials.
- Local run instructions.
- Test commands.
- API endpoints.
- HTTP response format.
- Key design decisions.
- Future improvements.

Created `docs/genai.md` with:

- Prompt used.
- Representative AI output.
- Validation approach.
- Corrections and improvements.
- Edge cases handled.
- Final position on GenAI usage.


### Phase 5 Frontend Completed

Created a React frontend using Vite, TypeScript, React Router, and Tailwind CSS.

Implemented frontend structure:

- `api/httpClient.ts`
- `api/authApi.ts`
- `api/tasksApi.ts`
- `auth/AuthContext.tsx`
- `components/Layout.tsx`
- `components/ErrorAlert.tsx`
- `components/TaskForm.tsx`
- `components/TaskList.tsx`
- `pages/LoginPage.tsx`
- `pages/RegisterPage.tsx`
- `pages/TasksPage.tsx`

Implemented frontend behavior:

- Login with seeded demo credentials.
- User registration.
- JWT storage in `localStorage`.
- Protected task route.
- Task list.
- Task creation.
- Task update.
- Task deletion.
- API error display using the backend response envelope.

### Phase 5 Design Notes

The frontend uses the same `/api` path as the backend. Vite proxies `/api` to the local API during frontend development, and Phase 6 will build the frontend into the API `wwwroot` folder for a single-endpoint demo.

The UI uses Tailwind CSS and keeps the design functional, responsive, and clean without adding a large component library.

The TypeScript configuration generated by the current Vite template enables `erasableSyntaxOnly`, so the frontend avoids TypeScript `enum` declarations and parameter properties.

### Phase 5 Verification

Ran:

```text
npm run build
```

Result:

```text
TypeScript build succeeded.
Vite production build succeeded.
```

### Phase 6 Single Endpoint Demo Completed

Configured the API to serve the React SPA from `wwwroot`:

- `UseDefaultFiles`
- `UseStaticFiles`
- `MapFallbackToFile("index.html")`

Created `run-dev.bat` at the repository root. The script:

- Restores backend packages.
- Installs frontend packages.
- Builds the frontend.
- Copies `frontend/dist` into `src/TaskPlanner.Api/wwwroot`.
- Builds the backend solution.
- Runs the API unless `--no-run` is provided.

Added ignore rules for generated static assets:

- `frontend/dist/`
- `src/TaskPlanner.Api/wwwroot/`

### Phase 6 Tooling Note

The first `run-dev.bat` draft called `npm` directly. On Windows, `npm` resolves to `npm.cmd`, and batch files must use `call npm ...` to return control to the parent script. The script was corrected to use `call npm install` and `call npm run build`.

### Phase 6 Verification

Ran:

```text
cmd /c run-dev.bat --no-run
```

Result:

```text
Frontend build succeeded.
Static assets copied into API wwwroot.
Backend build succeeded.
0 Warning(s)
0 Error(s)
```

Ran:

```text
dotnet test TaskPlanner.slnx
```

Result:

```text
Passed: 30
Failed: 0
```
