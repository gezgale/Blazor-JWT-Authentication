# Auth API — ASP.NET Core Identity and JWT

An ASP.NET Core 9 Web API that provides Identity-based user registration, login, JWT Bearer authentication, protected endpoints, API versioning, SQL Server persistence, and Swagger documentation.

## Repository Description

> ASP.NET Core 9 authentication API with Identity, JWT Bearer tokens, API versioning, EF Core, SQL Server, Swagger, and standardized API responses.

## Features

- ASP.NET Core Identity with a custom `ApplicationUser`
- Registration and login using email/password
- Signed JWT access tokens
- User and role claims embedded in tokens
- Bearer-token validation with zero clock skew
- Authorized controllers and endpoints
- Versioned routes such as `/api/v1/User/Login`
- Entity Framework Core with SQL Server
- Included Identity database migration
- Swagger/OpenAPI in Development
- CORS support for a separate Blazor WebAssembly client
- Standard API-response wrapper through `ApiResultFilterAttribute`
- Basic user-management service and endpoints

## Stack

- .NET 9
- ASP.NET Core Web API
- ASP.NET Core Identity
- JWT Bearer Authentication
- Entity Framework Core 9
- SQL Server
- Asp.Versioning.Mvc
- Swashbuckle / Swagger

## Structure

```text
AuthApi/
├── Controllers/
│   ├── v1/
│   │   ├── UserController.cs
│   │   └── UserManagementController.cs
│   ├── BaseController.cs
│   ├── TestController.cs
│   └── WeatherController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── DTOs/
├── Framework/
│   ├── Common/
│   └── SharedKernel/
├── Migrations/
├── Models/
├── Services/
│   ├── JwtTokenService.cs
│   └── UserManagementService.cs
└── Program.cs
```

## Configuration

Never commit production database credentials or JWT signing keys.

Recommended local configuration:

```bash
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=DotNet9JwtIdentityDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_LONG_RANDOM_SECRET_KEY"
dotnet user-secrets set "Jwt:Issuer" "AuthApi"
dotnet user-secrets set "Jwt:Audience" "BlazorClient"
dotnet user-secrets set "Jwt:ExpiresInMinutes" "30"
```

## Run Locally

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Development URLs:

```text
https://localhost:55281
http://localhost:55282
```

## API Endpoints

| Method | Route | Access | Description |
|---|---|---|---|
| POST | `/api/v1/User/Register` | Anonymous | Register a user and issue a JWT |
| POST | `/api/v1/User/Login` | Anonymous | Authenticate a user and issue a JWT |
| GET | `/api/v1/UserManagement/GetUsers` | Authorized | Return users |
| PUT | `/api/v1/UserManagement/UpdateUser` | Authorized | Placeholder update endpoint |
| GET | `/api/weather` | Authorized | Protected sample data |
| GET | `/api/test/public` | Anonymous | Public health/test request |
| GET | `/api/test/secure` | Authorized | Validate JWT authentication |

## JWT Claims

Generated tokens currently include:

- Subject/user ID
- Email
- Name identifier
- User name
- Full name
- JWT ID
- Identity roles

## Production Notes

- Apply an administrator policy to user-management endpoints.
- Remove duplicate and sample authentication controllers.
- Implement refresh-token rotation or another session-renewal strategy.
- Add rate limiting and Identity lockout.
- Keep secrets outside source control.
- Replace debug console output with structured logging.
- Use asynchronous database queries for user-list operations.
- Add integration tests for authentication and authorization.
