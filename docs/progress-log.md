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

No application code has been implemented yet.

### Current Status

The project is in planning and documentation setup. The next planned step is Phase 1: create the .NET solution, projects, references, and initial test structure.

