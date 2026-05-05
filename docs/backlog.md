# TaskPlanner Backlog

This backlog tracks planned implementation work for the interview exercise. It is organized by delivery phase plus a continuous documentation track that should be updated as the project evolves.

## Status Legend

- `Todo`: Not started.
- `In Progress`: Currently being worked on.
- `Done`: Completed and verified.
- `Blocked`: Waiting on a decision or dependency.

## Continuous Track: Documentation

| Status | Item | Notes |
| --- | --- | --- |
| Done | Create README | Setup, architecture, tests, demo credentials. |
| Done | Create `docs/genai.md` | Prompt, sample output, validation, improvements. |
| Done | Create technical plan | Initial plan documented. |
| Done | Create architecture decisions | Initial ADRs documented. |
| Done | Create backlog | Initial backlog documented. |
| Done | Create progress log | Initial progress log documented. |
| In Progress | Update backlog during implementation | Keep statuses aligned with actual progress. |
| In Progress | Update progress log during implementation | Record meaningful milestones and verification results. |

## Phase 1: Foundation

| Status | Item | Notes |
| --- | --- | --- |
| Done | Create .NET solution `TaskPlanner` | Created as `TaskPlanner.slnx` with .NET 10 SDK. |
| Done | Create backend projects | API, Application, Domain, Infrastructure. |
| Done | Create test projects | Application, Infrastructure, API tests. |
| Done | Configure project references | Clean Architecture dependency direction is configured. |
| Done | Add basic build verification | `dotnet build TaskPlanner.slnx` succeeds with zero warnings. |
| Done | Add smoke tests | One assembly smoke test per test project. |

## Phase 2: Domain and Application TDD

| Status | Item | Notes |
| --- | --- | --- |
| Done | Create domain entities | `User`, `TaskItem`, `TaskStatus`. |
| Done | Create application result model | `Result<T>` and application errors. |
| Done | Define repository interfaces | `IUserRepository`, `ITaskRepository`. |
| Done | Define infrastructure service interfaces | `IPasswordHasher`, `ITokenService`, `IClock`. |
| Done | Write registration service tests first | Invalid input, duplicate email, success. |
| Done | Implement registration service | Validation is kept in Application. |
| Done | Write login service tests first | Invalid credentials and success. |
| Done | Implement login service | Returns token through abstraction. |
| Done | Write task service tests first | CRUD validation and ownership behavior. |
| Done | Implement task service | User-scoped task operations. |

## Phase 3: Infrastructure

| Status | Item | Notes |
| --- | --- | --- |
| Done | Add SQLite dependency | Uses `Microsoft.Data.Sqlite`; does not use EF or Dapper. |
| Done | Implement connection factory | SQLite details are isolated behind `SqliteConnectionFactory`. |
| Done | Implement database initializer | Creates users and tasks tables. |
| Done | Implement seed data | Demo user and three sample tasks. |
| Done | Write SQLite user repository tests first | Persist and query users. |
| Done | Implement SQLite user repository | Parameterized SQL only. |
| Done | Write SQLite task repository tests first | CRUD and user scoping. |
| Done | Implement SQLite task repository | Stores status as integer. |
| Done | Implement password hashing | PBKDF2-SHA256 with per-password salt. |
| Done | Implement JWT token service | Includes user ID, email, and name claims. |

## Phase 4: API

| Status | Item | Notes |
| --- | --- | --- |
| Done | Configure dependency injection | Application and Infrastructure are wired in API startup. |
| Done | Configure JWT authentication | Private endpoints require bearer tokens. |
| Done | Add exception handling middleware | Unexpected errors return consistent envelopes. |
| Done | Add response envelope | `ApiResponse<T>` is used for JSON responses except `204`. |
| Done | Implement `AuthController` | Register, login, me. |
| Done | Implement `TasksController` | User-scoped CRUD. |
| Done | Implement `HealthController` | Public and secure health checks. |
| Done | Write API tests first where practical | Register, login, auth, CRUD, and health covered. |
| Done | Verify HTTP status codes | Integration tests cover expected status codes. |

## Phase 5: Frontend

| Status | Item | Notes |
| --- | --- | --- |
| Done | Create React app | Vite, TypeScript. |
| Done | Add Tailwind CSS | Responsive, clean UI foundation. |
| Done | Create API client | Handles envelope and auth token. |
| Done | Create auth state | Stores JWT in local storage. |
| Done | Create login page | Uses API errors and seeded demo credentials. |
| Done | Create register page | Uses API errors. |
| Done | Create tasks page | List, create, update, delete tasks. |
| Done | Add route protection | Redirects unauthenticated users. |
| Done | Verify responsive layout | Implemented with responsive Tailwind layouts; final browser pass remains in polish. |
| Todo | Check browser console | Final browser-console pass will happen after one-endpoint demo setup. |

## Phase 6: Single Endpoint Demo

| Status | Item | Notes |
| --- | --- | --- |
| Done | Configure API static files | Serves frontend from `wwwroot`. |
| Done | Configure SPA fallback | Falls back to `index.html`. |
| Done | Create `run-dev.bat` | Restore, install, build, copy, run; supports `--no-run` verification. |
| Done | Verify one-endpoint demo | `run-dev.bat --no-run` builds and copies frontend into API `wwwroot`. |

## Phase 7: Final Verification and Polish

| Status | Item | Notes |
| --- | --- | --- |
| Done | Run backend tests | Application, Infrastructure, API currently pass. |
| Done | Run backend build | Verified through `run-dev.bat --no-run`. |
| Done | Run frontend build | Production assets build successfully. |
| Done | Run one-endpoint flow | `run-dev.bat --no-run` validated. |
| Done | Review README instructions | Setup and verification instructions documented. |
| Todo | Review naming and comments | Keep all code and docs in English. |
| Todo | Prepare optional presentation notes | Decide after core delivery. |
