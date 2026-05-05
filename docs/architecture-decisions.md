# Architecture Decisions

This document records the main architecture decisions for TaskPlanner. It is intended to make the project easier to review, explain, and evolve.

## ADR-001: Use .NET 10

### Decision

Use .NET 10 for the backend.

### Rationale

.NET 10 is an LTS release and is appropriate for a modern interview project. It also gives the project a current technology baseline while remaining familiar to .NET reviewers.

### Consequences

- The local environment must have the .NET 10 SDK installed.
- The project can use current ASP.NET Core conventions.
- The README must clearly mention the required SDK version.

## ADR-002: Use ASP.NET Core Controllers

### Decision

Use traditional ASP.NET Core controllers instead of Minimal APIs.

### Rationale

Controllers are explicit, familiar, easy to navigate in a code review, and align well with a conventional layered architecture. They also make it straightforward to demonstrate route organization, authorization attributes, request models, and integration tests.

### Consequences

- Slightly more boilerplate than Minimal APIs.
- Clearer structure for interview discussion.

## ADR-003: Use Clean Architecture with Pragmatic Boundaries

### Decision

Use separate projects for Domain, Application, Infrastructure, and API.

### Rationale

The exercise explicitly asks for Clean Architecture principles. Separate projects help enforce dependency direction and separation of concerns.

### Consequences

- The domain model remains independent.
- The application layer does not depend on SQLite or ASP.NET Core controllers.
- Infrastructure can be replaced without changing business rules.
- The project remains simple enough to explain without adding unnecessary patterns.

## ADR-004: Use Repository Pattern

### Decision

Define repository interfaces in the Application layer and implement them in Infrastructure.

### Rationale

The exercise requires a data layer and also forbids Entity Framework and Dapper. Repository interfaces provide a clean boundary around persistence and make it easier to replace SQLite with PostgreSQL, SQL Server, Cosmos DB, or another storage provider later.

### Consequences

- SQL details stay outside the Application layer.
- Tests can use fake repositories for application services.
- Repository integration tests can validate the real SQLite implementation.
- Additional repository implementations can be added without changing use cases.

## ADR-005: Use SQLite with Manual SQL

### Decision

Use SQLite through `Microsoft.Data.Sqlite` and write SQL manually.

### Rationale

SQLite is lightweight, easy to run locally, and suitable for an interview exercise. Manual SQL satisfies the requirement to avoid Entity Framework and Dapper while still demonstrating a real data layer.

### Consequences

- SQL must be carefully parameterized to avoid injection.
- Mapping code must be written manually.
- Schema initialization must be handled by the application.
- The repository boundary keeps this implementation replaceable.

## ADR-006: Use JWT Authentication

### Decision

Use JWT bearer authentication for securing API endpoints.

### Rationale

JWT is a common and interview-friendly way to demonstrate login, protected endpoints, and frontend-backend integration.

### Consequences

- The frontend can store the access token and send it in the `Authorization` header.
- The API can identify the authenticated user from token claims.
- Token settings must be documented and configured safely for local development.

## ADR-007: Enforce User Ownership in Task Operations

### Decision

All task operations will be scoped to the authenticated user's ID.

### Rationale

Authentication alone is not enough. The application must ensure that users cannot read, update, or delete tasks owned by someone else.

### Consequences

- Task repository methods should include `userId` for read, update, and delete operations.
- Requests for another user's task should return `404 Not Found` to avoid leaking resource existence.
- API tests must cover cross-user access.

## ADR-008: Use `TaskItem` as the Entity Name

### Decision

Name the task entity `TaskItem`.

### Rationale

`Task` conflicts conceptually and technically with `System.Threading.Tasks.Task`. `TaskItem` keeps the domain name clear and avoids ambiguity in C# code.

### Consequences

- API routes and UI labels can still use "task".
- C# code remains easier to read.

## ADR-009: Store Task Status as an Integer

### Decision

Use a C# enum for task status and store its value as an integer in SQLite.

### Rationale

This keeps the database compact and reflects the explicit status set in the domain model.

### Consequences

- API request validation must reject invalid enum values.
- Mapping between database rows and domain objects must be explicit.
- Documentation should list the enum values.

## ADR-010: Use Manual Application Validation

### Decision

Implement validation in application services instead of adding FluentValidation initially.

### Rationale

Manual validation keeps dependencies low and makes TDD behavior straightforward for a compact interview project.

### Consequences

- Validation logic must be kept organized to avoid duplication.
- If the project grows, FluentValidation could be introduced later without changing API contracts.

## ADR-011: Use a Result Pattern for Expected Failures

### Decision

Represent expected business and validation failures with a `Result<T>` type instead of exceptions.

### Rationale

Expected failures such as invalid input, duplicate emails, and missing tasks are normal application outcomes. A result object makes these outcomes explicit and easy to test.

### Consequences

- Controllers map result statuses to HTTP status codes.
- Application services remain independent from ASP.NET Core response types.
- Unexpected exceptions are reserved for truly exceptional cases and handled by middleware.

## ADR-012: Use Centralized Exception Handling

### Decision

Use middleware to handle unexpected exceptions and return a consistent error response.

### Rationale

Centralized exception handling avoids repeated `try/catch` blocks in controllers and keeps unexpected error responses consistent for the frontend.

### Consequences

- Controllers can stay focused on request handling and result mapping.
- Unexpected errors return `500 Internal Server Error`.
- Logs can be added centrally.

## ADR-013: Use an API Response Envelope

### Decision

Return JSON responses using an `ApiResponse<T>` envelope for most non-empty responses.

### Rationale

A consistent response shape makes frontend error handling simpler and gives the API predictable contracts.

### Consequences

- HTTP status codes remain authoritative.
- `204 No Content` responses will not include an envelope.
- The frontend can consistently read `success`, `data`, and `errors`.

## ADR-014: Use React, Vite, TypeScript, and Tailwind CSS

### Decision

Build the frontend with React, Vite, TypeScript, and Tailwind CSS.

### Rationale

This stack is lightweight, modern, fast to develop, and well-suited for a clean responsive UI.

### Consequences

- The frontend can be built into static assets.
- Tailwind keeps styling local and fast without introducing a large component framework.
- TypeScript improves API contract safety.

## ADR-015: Serve the Frontend from the API for Demo

### Decision

Build the React app and copy it into the API project's `wwwroot` directory.

### Rationale

Serving the frontend and API from one local endpoint simplifies the interview demo and reduces setup friction.

### Consequences

- A `run-dev.bat` script will automate the build and copy process.
- During local development, the frontend may still be run separately if needed.
- The API must be configured for static files and SPA fallback.

## ADR-016: Exclude Docker from the Initial Scope

### Decision

Do not include Docker in the initial implementation.

### Rationale

The project has a 72-hour delivery window. Docker would be useful, but it is not required by the exercise and would add setup surface area.

### Consequences

- Setup instructions must be clear for local .NET and Node execution.
- Docker can be listed as a future improvement.

