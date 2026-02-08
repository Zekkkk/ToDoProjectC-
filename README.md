# Advanced To-Do List API (Student Project)

This project is a .NET 8 Web API for managing tasks, subtasks, and time logs.  
It uses EF Core 8 with PostgreSQL and includes JWT authentication.

Note: The React frontend is included only for **presentation/demo purposes**.

## Tech Stack
- .NET 8 Web API
- EF Core 8
- PostgreSQL
- Swagger (OpenAPI)
- JWT Authentication

## Main Features
- Task CRUD with filters and sorting
- Subtask CRUD (checklist items under a task)
- Time logs (start/stop/list/delete)
- User registration + login (JWT)

## How to Run the API

From the API folder:
```bash
cd /Users/zekoosmani/RiderProjects/ToDo/ToDo.api
dotnet build
dotnet ef database update
dotnet run
```

Swagger URL:
```
http://localhost:5142/swagger
```

## How to Run the Frontend (Demo Only)

```bash
cd /Users/zekoosmani/RiderProjects/ToDo/ToDo.web
npm install
npm run dev
```

Frontend URL:
```
http://localhost:5173
```

## Authentication Flow (Swagger)
1. POST `/api/auth/register`
2. POST `/api/auth/login` (copy token)
3. Click **Authorize** in Swagger and paste: `Bearer <token>`

## Common Endpoints

### Tasks
- POST `/api/tasks`
- GET `/api/tasks`
- GET `/api/tasks/{id}`
- PUT `/api/tasks/{id}`
- DELETE `/api/tasks/{id}`

### Subtasks
- POST `/api/tasks/{taskId}/subtasks`
- GET `/api/tasks/{taskId}/subtasks`
- PUT `/api/subtasks/{id}`
- DELETE `/api/subtasks/{id}`

### Time Logs
- GET `/api/tasks/{taskId}/timelogs`
- POST `/api/tasks/{taskId}/timelogs/start`
- POST `/api/tasks/{taskId}/timelogs/stop`
- DELETE `/api/tasks/{taskId}/timelogs/{timeLogId}`

## Notes
- Due dates are stored as **date-only** (time removed).
- Tasks are user-scoped after login.
- Subtasks and time logs are cascade-deleted with tasks.
