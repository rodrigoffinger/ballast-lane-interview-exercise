# TaskPlanner

TaskPlanner is a full-stack task management application built for the Ballast Lane .NET technical interview exercise.

The application allows users to register, log in, and manage their own tasks through a secured ASP.NET Core Web API and a React frontend.

## User Story

As a registered user, I want to manage my personal tasks with a title, description, status, and due date, so that I can organize my work and track what needs attention.

## Tech Stack

- Backend: .NET 10, ASP.NET Core Web API, C#
- Frontend: React, Vite, TypeScript, Tailwind CSS
- Database: SQLite through `Microsoft.Data.Sqlite`
- Authentication: JWT bearer tokens
- Tests: xUnit, ASP.NET Core integration tests

## Important Constraints

The implementation intentionally does not use:

- Entity Framework
- Dapper
- MediatR

Persistence is implemented with manual, parameterized SQL behind repository interfaces.

## Architecture

The backend follows a pragmatic Clean Architecture structure:

```text
src/
  TaskPlanner.Domain
  TaskPlanner.Application
  TaskPlanner.Infrastructure
  TaskPlanner.Api

tests/
  TaskPlanner.Application.Tests
  TaskPlanner.Infrastructure.Tests
  TaskPlanner.Api.Tests

frontend/

docs/
```

Dependency direction:

- `Domain` has no project dependencies.
- `Application` depends on `Domain`.
- `Infrastructure` depends on `Application` and `Domain`.
- `Api` depends on `Application` and `Infrastructure`.

The Application layer defines repository and infrastructure service interfaces. Infrastructure provides SQLite repositories, password hashing, JWT generation, clock, and seed data.

## Demo Credentials

The application seeds a demo user on startup:

```text
Email: demo@ballastlane.local
Password: Demo123!
```

The demo user is created with three sample tasks.

## How to Run

Prerequisites:

- .NET 10 SDK
- Node.js 20+
- npm

From the repository root:

```bat
run-dev.bat
```

The script will:

- Restore backend packages.
- Install frontend packages.
- Build the React frontend.
- Copy the frontend build into `src/TaskPlanner.Api/wwwroot`.
- Build the backend.
- Run the backend test suite.
- Run the API.

After startup, open the URL shown by `dotnet run`. The frontend and API are served from the same endpoint.

For build verification without starting the server:

```bat
run-dev.bat --no-run
```

This still runs the backend tests.

## Running Tests

Run all backend tests:

```bash
dotnet test TaskPlanner.slnx
```

Run frontend production build:

```bash
cd frontend
npm run build
```

## API Endpoints

Auth:

```http
POST /api/auth/register
POST /api/auth/login
GET  /api/auth/me
```

Tasks:

```http
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
```

Health:

```http
GET /api/health
GET /api/health/secure
```

## HTTP Response Format

Most JSON responses use a consistent envelope:

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

HTTP status codes remain authoritative. `204 No Content` responses intentionally do not include a body.

## Design Decisions

Key decisions are documented in [docs/architecture-decisions.md](docs/architecture-decisions.md).

Highlights:

- Controllers were chosen over Minimal APIs for explicit reviewability.
- Repository Pattern isolates persistence and keeps SQLite replaceable.
- Expected business failures use a `Result<T>` pattern.
- Unexpected errors are handled by centralized middleware.
- Users can only access their own tasks.
- Task status is represented as a C# domain value and stored as an integer in SQLite.

## GenAI Documentation

The GenAI prompt, representative output, validation notes, corrections, and edge case handling are documented in [docs/genai.md](docs/genai.md).

## Future Improvements

- Add migrations instead of startup schema initialization.
- Add pagination and filtering for large task lists.
- Add refresh tokens or session revocation for production authentication.
- Add Docker support.
- Add frontend automated tests.
- Add structured logging and observability.
