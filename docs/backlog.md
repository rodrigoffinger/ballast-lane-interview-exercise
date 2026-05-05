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
| Todo | Create README | Setup, architecture, tests, demo credentials. |
| Todo | Create `docs/genai.md` | Prompt, sample output, validation, improvements. |
| Done | Create technical plan | Initial plan documented. |
| Done | Create architecture decisions | Initial ADRs documented. |
| Done | Create backlog | Initial backlog documented. |
| Done | Create progress log | Initial progress log documented. |
| Todo | Update backlog during implementation | Keep statuses aligned with actual progress. |
| Todo | Update progress log during implementation | Record meaningful milestones and verification results. |

## Phase 1: Foundation

| Status | Item | Notes |
| --- | --- | --- |
| Todo | Create .NET solution `TaskPlanner` | Use .NET 10. |
| Todo | Create backend projects | API, Application, Domain, Infrastructure. |
| Todo | Create test projects | Application, Infrastructure, API tests. |
| Todo | Configure project references | Enforce Clean Architecture dependency direction. |
| Todo | Add basic build verification | Ensure empty solution builds. |

## Phase 2: Domain and Application TDD

| Status | Item | Notes |
| --- | --- | --- |
| Todo | Create domain entities | `User`, `TaskItem`, `TaskStatus`. |
| Todo | Create application result model | `Result<T>` and application errors. |
| Todo | Define repository interfaces | `IUserRepository`, `ITaskRepository`. |
| Todo | Define infrastructure service interfaces | `IPasswordHasher`, `ITokenService`, `IClock`. |
| Todo | Write registration service tests first | Invalid input, duplicate email, success. |
| Todo | Implement registration service | Keep validation in Application. |
| Todo | Write login service tests first | Invalid credentials and success. |
| Todo | Implement login service | Return token through abstraction. |
| Todo | Write task service tests first | CRUD validation and ownership behavior. |
| Todo | Implement task service | User-scoped task operations. |

## Phase 3: Infrastructure

| Status | Item | Notes |
| --- | --- | --- |
| Todo | Add SQLite dependency | Use `Microsoft.Data.Sqlite`; do not use EF or Dapper. |
| Todo | Implement connection factory | Keep SQLite details isolated. |
| Todo | Implement database initializer | Create users and tasks tables. |
| Todo | Implement seed data | Demo user and sample tasks. |
| Todo | Write SQLite user repository tests first | Persist and query users. |
| Todo | Implement SQLite user repository | Parameterized SQL only. |
| Todo | Write SQLite task repository tests first | CRUD and user scoping. |
| Todo | Implement SQLite task repository | Store status as integer. |
| Todo | Implement password hashing | Secure local password storage. |
| Todo | Implement JWT token service | Include user ID and email claims. |

## Phase 4: API

| Status | Item | Notes |
| --- | --- | --- |
| Todo | Configure dependency injection | Wire Application and Infrastructure. |
| Todo | Configure JWT authentication | Protect private endpoints. |
| Todo | Add exception handling middleware | Consistent unexpected error response. |
| Todo | Add response envelope | `ApiResponse<T>` for JSON responses. |
| Todo | Implement `AuthController` | Register, login, me. |
| Todo | Implement `TasksController` | User-scoped CRUD. |
| Todo | Implement `HealthController` | Public and secure health checks. |
| Todo | Write API tests first where practical | Register, login, auth, CRUD, ownership. |
| Todo | Verify HTTP status codes | Ensure status codes match API semantics. |

## Phase 5: Frontend

| Status | Item | Notes |
| --- | --- | --- |
| Todo | Create React app | Vite, TypeScript. |
| Todo | Add Tailwind CSS | Responsive, clean UI. |
| Todo | Create API client | Handle envelope and auth token. |
| Todo | Create auth state | Store JWT in local storage. |
| Todo | Create login page | Use API errors. |
| Todo | Create register page | Use API errors. |
| Todo | Create tasks page | List, create, update, delete tasks. |
| Todo | Add route protection | Redirect unauthenticated users. |
| Todo | Verify responsive layout | Desktop and mobile. |
| Todo | Check browser console | Avoid avoidable warnings. |

## Phase 6: Single Endpoint Demo

| Status | Item | Notes |
| --- | --- | --- |
| Todo | Configure API static files | Serve frontend from `wwwroot`. |
| Todo | Configure SPA fallback | Fallback to `index.html`. |
| Todo | Create `run-dev.bat` | Restore, install, build, copy, run. |
| Todo | Verify one-endpoint demo | Frontend and API work from the same origin. |

## Phase 7: Final Verification and Polish

| Status | Item | Notes |
| --- | --- | --- |
| Todo | Run backend tests | Application, Infrastructure, API. |
| Todo | Run backend build | Ensure no build errors. |
| Todo | Run frontend build | Ensure production assets build. |
| Todo | Run one-endpoint flow | Validate `run-dev.bat`. |
| Todo | Review README instructions | Ensure a reviewer can run the app. |
| Todo | Review naming and comments | Keep all code and docs in English. |
| Todo | Prepare optional presentation notes | Decide after core delivery. |
