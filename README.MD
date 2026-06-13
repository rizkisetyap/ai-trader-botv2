# ai-trader-bot

ai-trader-bot is a small multi-service project for stock analysis workflows. It contains a .NET 8 Web API, a Python worker placeholder, a shared test folder, and Docker Compose wiring for local development.

## Project Structure

- `backend-net/` - .NET 8 Web API using Entity Framework Core and SQLite.
- `worker-python/` - Python worker service placeholder.
- `UnitTest/` - Test workspace for backend and Python test scaffolding.
- `docker-compose.yml` - Local orchestration for RabbitMQ, API, and worker services.

## Backend Overview

The backend uses `AppDbContext` with SQLite and a single current entity set: `AnalisaRequests`.

Key pieces:

- `backend-net/Program.cs` registers `AppDbContext` and `AnalisaService` in DI.
- `backend-net/Controllers/SahamController.cs` exposes endpoints for submitting and reading analisa requests.
- `backend-net/Services/AnalisaService.cs` handles CRUD operations for analisa requests.
- `backend-net/Migrations/` contains the initial EF Core migration.

SQLite connection string:

```json
"ConnectionStrings": {
	"db-ai_bot": "Data Source=db-ai_bot.db"
}
```

The database file is created inside `backend-net/` by default.

## Local Setup

### Prerequisites

- .NET 8 SDK
- Python 3.11 or newer
- Docker and Docker Compose if you want to run the containerized stack

### Run the backend locally

From `backend-net/`:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

### Run the backend tests

From `UnitTest/backend-net.Tests/`:

```bash
dotnet test
```

### Run the Python tests

From `UnitTest/python-worker-tests/`:

```bash
pytest
```

## Docker Compose

The root `docker-compose.yml` starts:

- RabbitMQ on ports `5672` and `15672`
- the .NET API on port `5000`
- the Python worker container

Start everything with:

```bash
docker compose up --build
```

## API Endpoints

Base route: `api/v1/Saham`

- `POST /terima-analisa` - save a new analisa request
- `GET /get-analisa-result` - list all requests
- `GET /get-analisa-result/{id}` - get one request by id
- `DELETE /get-analisa-result/{id}` - delete a request by id

## Tests

The repository currently includes:

- service tests for `AnalisaService`
- controller tests for `SahamController`
- a Python pytest scaffold for the worker side

## Notes

- The backend currently persists analisa requests in SQLite.
- `worker-python/` is still empty and can be filled in later with the AI worker implementation.
- `UnitTest/` is intentionally separate from the application folders so backend and worker tests stay isolated.
