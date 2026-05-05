# TaskPlanner Technical Plan

## Project Goal

TaskPlanner is a full-stack task management application built for the Ballast Lane .NET technical interview exercise. The application will allow registered users to create, read, update, and delete their own tasks through a secured REST API and a responsive React frontend.

The implementation will prioritize clarity, testability, maintainability, and a pragmatic Clean Architecture structure. The project will intentionally avoid Entity Framework, Dapper, and MediatR, as required by the exercise.

## User Story

As a registered user, I want to manage my personal tasks with a title, description, status, and due date, so that I can organize my work and track what needs attention.

## Core Requirements

- Build an ASP.NET Core Web API using .NET 10.
- Provide CRUD endpoints for task records.
- Provide user registration and login endpoints.
- Store users and tasks in a database or data store.
- Protect task endpoints with authentication.
- Ensure users can only access their own tasks.
- Use Clean Architecture principles.
- Use Test-Driven Development explicitly during implementation.
- Write tests for the application layer, data access layer, and API endpoints.
- Build a responsive React frontend integrated with the backend.
- Include seeded demo data and credentials.
- Include setup instructions and documentation.
- Include documentation about GenAI usage, prompt validation, and improvements.

## Proposed Solution Structure

```text
src/
  TaskPlanner.Api/
  TaskPlanner.Application/
  TaskPlanner.Domain/
  TaskPlanner.Infrastructure/

tests/
  TaskPlanner.Application.Tests/
  TaskPlanner.Infrastructure.Tests/
  TaskPlanner.Api.Tests/

frontend/

docs/
```

## Project Responsibilities

### TaskPlanner.Domain

Contains the core domain model and should not depend on any other project.

Planned types:

- `User`
- `TaskItem`
- `TaskStatus`

### TaskPlanner.Application

Contains use cases, DTOs, validation, application-level result objects, and repository/service abstractions.

Planned responsibilities:

- User registration.
- User login.
- Task creation.
- Task retrieval.
- Task update.
- Task deletion.
- Business validation.
- Ownership checks through user-scoped service methods.

### TaskPlanner.Infrastructure

Contains external implementation details.

Planned responsibilities:

- SQLite connection management.
- Database schema initialization.
- Seed data.
- SQL-based repository implementations.
- Password hashing.
- JWT token generation.
- Clock implementation.

### TaskPlanner.Api

Contains the HTTP interface and runtime composition.

Planned responsibilities:

- Controllers.
- Authentication and authorization setup.
- Dependency injection.
- Exception handling middleware.
- Static file hosting for the built frontend.
- SPA fallback for the React app.

### Frontend

Contains the React application.

Planned responsibilities:

- Login page.
- Register page.
- Tasks dashboard.
- API client.
- Auth state.
- Task CRUD UI.
- Responsive layout.

## Backend Endpoints

### Auth

```http
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
```

### Tasks

```http
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
```

### Health

```http
GET /api/health
GET /api/health/secure
```

## HTTP Response Strategy

The API will use correct HTTP status codes and a consistent response envelope for JSON responses.

Success example:

```json
{
  "success": true,
  "data": {},
  "errors": []
}
```

Error example:

```json
{
  "success": false,
  "data": null,
  "errors": [
    {
      "code": "ValidationError",
      "message": "Title is required."
    }
  ]
}
```

Planned status codes:

- `200 OK` for successful reads, updates, login, and authenticated profile retrieval.
- `201 Created` for successful user and task creation.
- `204 No Content` for successful task deletion.
- `400 Bad Request` for validation errors.
- `401 Unauthorized` for missing or invalid authentication.
- `404 Not Found` for missing resources or resources owned by another user.
- `409 Conflict` for duplicate user email registration.
- `500 Internal Server Error` for unexpected errors handled by middleware.

`204 No Content` responses will not use the response envelope because a `204` response must not include a body.

## Data Model

### Users

```sql
users (
  id TEXT PRIMARY KEY,
  name TEXT NOT NULL,
  email TEXT NOT NULL UNIQUE,
  password_hash TEXT NOT NULL,
  created_at TEXT NOT NULL
)
```

### Tasks

```sql
tasks (
  id TEXT PRIMARY KEY,
  user_id TEXT NOT NULL,
  title TEXT NOT NULL,
  description TEXT NOT NULL,
  status INTEGER NOT NULL,
  due_date TEXT NOT NULL,
  created_at TEXT NOT NULL,
  updated_at TEXT NOT NULL,
  FOREIGN KEY(user_id) REFERENCES users(id)
)
```

## Task Status Values

The application will use a C# enum and persist the value as an integer in SQLite.

```csharp
public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2
}
```

## Seed Data

The application will include demo credentials for interview and local testing purposes.

```text
Email: demo@ballastlane.local
Password: Demo123!
```

The seeded user will have a small set of sample tasks.

## TDD Execution Strategy

Implementation will proceed in small phases. Each meaningful behavior should start from a failing test whenever practical.

Planned test order:

1. Application service tests.
2. Infrastructure repository tests.
3. API integration tests.
4. Frontend behavior will be manually verified and can receive automated tests later if time allows.

## Single Endpoint Demo Strategy

The React frontend will be built and copied into the API project's `wwwroot` directory. The API will serve the SPA and expose the backend under `/api`.

The planned `run-dev.bat` flow is:

```bat
dotnet restore
cd frontend
npm install
npm run build
cd ..
copy frontend dist files into src\TaskPlanner.Api\wwwroot
dotnet build
dotnet run --project src\TaskPlanner.Api
```

This keeps the demo simple because the interviewer can open one local URL and use both the frontend and backend.

## Documentation Deliverables

- `README.md` with setup instructions, architecture overview, requirements mapping, seed credentials, and test commands.
- `docs/genai.md` with the GenAI prompt, representative output, validation notes, improvements, and edge cases.
- `docs/architecture-decisions.md` with explicit technical decisions and tradeoffs.
- `docs/backlog.md` with implementation phases and task tracking.
- `docs/progress-log.md` with a chronological implementation log.

