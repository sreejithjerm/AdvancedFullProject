# AdvancedFullProject (.NET 8 Clean Architecture Web API)

Production-ready starter template with **Clean Architecture**, **CQRS + MediatR**, **ADO.NET + Stored Procedures**, JWT security, middleware, caching, observability, background jobs, RabbitMQ, webhook support, and tests.

## 1) Folder Structure

```text
AdvancedFullProject/
├── AdvancedFullProject.sln
├── Directory.Build.props
├── src/
│   ├── AdvancedFullProject.API/
│   │   ├── Controllers/
│   │   ├── Extensions/
│   │   ├── Filters/
│   │   ├── Middlewares/
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs
│   ├── AdvancedFullProject.Application/
│   │   ├── Abstractions/
│   │   ├── Auth/
│   │   ├── Common/
│   │   ├── DependencyInjection/
│   │   ├── Employees/
│   │   └── Mapping/
│   ├── AdvancedFullProject.Domain/
│   │   ├── Common/
│   │   └── Entities/
│   └── AdvancedFullProject.Infrastructure/
│       ├── Background/
│       ├── Caching/
│       ├── DependencyInjection/
│       ├── Messaging/
│       ├── Options/
│       ├── Persistence/
│       ├── Security/
│       └── Webhook/
├── tests/
│   ├── AdvancedFullProject.UnitTests/
│   └── AdvancedFullProject.IntegrationTests/
├── database/
│   └── employee_module.sql
└── docker/
    └── Dockerfile
```

## 2) What Is Implemented

### Clean Architecture + SOLID
- API, Application, Domain, Infrastructure are separated with references flowing inward.
- CQRS handlers for Employee and Auth modules.
- Repository + Unit of Work with SQL Server ADO.NET and stored procedures.

### Security
- JWT authentication (`JwtBearer`).
- Role-based authorization (`[Authorize(Roles=...)]`).
- Policy-based authorization (`CanReadEmployees`, `HighPrivilege`).
- Refresh token persistence and rotation.
- BCrypt password hashing.

### Middleware & Error Handling
- .NET 8 `IExceptionHandler` global exception middleware.
- Request/Response logging middleware.
- Correlation ID middleware.
- Standardized `ProblemDetails` output.

### Validation & Mapping
- FluentValidation + MediatR pipeline behavior.
- AutoMapper profile for Employee mapping.

### Performance
- In-memory cache registration.
- Redis distributed cache integration.
- Response caching.
- Fixed-window rate limiting.
- Gzip + Brotli compression.
- Pagination/filtering/sorting via stored procedure.

### Logging
- Serilog Console + rolling file sink.
- Structured request logs.

### Background Processing
- Quartz.NET sample job (`EmployeeSyncJob`).
- `IHostedService` sample (`HeartbeatHostedService`).
- RabbitMQ publisher implementation.
- Webhook sender implementation.

### API Features
- API versioning.
- Swagger with JWT Bearer support.
- Health checks endpoint (`/health`).
- File upload endpoint.
- Streaming endpoint.
- Minimal API endpoint with endpoint filter.

### DevOps & Config
- Environment-specific appsettings.
- Strongly typed options (`JwtOptions`, `DatabaseOptions`).
- Dockerfile.
- Clean Program.cs bootstrap.

### Database
- SQL Server schema + procedures in `database/employee_module.sql`.
- Soft delete (`IsDeleted`).
- Audit fields (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`).

### Modern .NET 8
- Endpoint filters.
- Result pattern included in Application layer.
- Global using file in API.

## 3) NuGet Packages

- MediatR
- FluentValidation + DI extensions
- AutoMapper
- Microsoft.Data.SqlClient
- BCrypt.Net-Next
- Microsoft.AspNetCore.Authentication.JwtBearer
- Asp.Versioning.Mvc + ApiExplorer
- Swashbuckle.AspNetCore
- Serilog.AspNetCore + Console + File sinks
- Microsoft.Extensions.Caching.StackExchangeRedis
- Quartz.Extensions.Hosting
- RabbitMQ.Client
- xUnit + FluentAssertions + ASP.NET Core Testing

## 4) Employee Sample CRUD (CQRS)

- `GET /api/v1/employees` (paged)
- `GET /api/v1/employees/{id}`
- `POST /api/v1/employees`
- `PUT /api/v1/employees/{id}`
- `DELETE /api/v1/employees/{id}` (soft delete)

## 5) Setup Instructions

1. Install .NET 8 SDK, SQL Server, Redis, RabbitMQ.
2. Create DB and run:
   - `database/employee_module.sql`
3. Configure `src/AdvancedFullProject.API/appsettings.json`:
   - SQL connection string
   - Redis connection string
   - JWT key/issuer/audience
4. Restore & run:
   ```bash
   dotnet restore AdvancedFullProject.sln
   dotnet build AdvancedFullProject.sln
   dotnet run --project src/AdvancedFullProject.API/AdvancedFullProject.API.csproj
   ```
5. Open Swagger:
   - `https://localhost:<port>/swagger`
6. For Docker:
   ```bash
   docker build -f docker/Dockerfile -t advancedfullproject .
   docker run -p 8080:8080 advancedfullproject
   ```

## 6) Tests

- Unit test: FluentValidation for CreateEmployee command.
- Integration test: health endpoint returns 200.

Run:
```bash
dotnet test AdvancedFullProject.sln
```

---

> This template is production-oriented and extensible; before go-live, set real secrets (KeyVault), tighten webhook URL allowlists, add resilience policies (Polly), and harden RabbitMQ/Redis credentials.
