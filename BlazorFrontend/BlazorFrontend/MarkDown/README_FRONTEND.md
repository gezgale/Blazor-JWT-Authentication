# Blazor Frontend — JWT Authentication Client

A .NET 9 Blazor WebAssembly client that communicates with an ASP.NET Core authentication API and provides login, registration, protected navigation, JWT state management, automatic Bearer-token attachment, user-list scaffolding, and toast notifications.

## Repository Description

> Blazor WebAssembly authentication client with JWT state management, protected routes, API handlers, local storage, PWA support, and toast notifications.

## Features

- Blazor WebAssembly on .NET 9
- Login and registration forms
- Data-annotation validation
- Custom `AuthenticationStateProvider`
- Authentication-state restoration after page reload
- JWT expiration check
- Automatic `Authorization: Bearer` header
- Automatic handling of `401 Unauthorized`
- Protected route rendering with `AuthorizeRouteView`
- Browser local storage through Blazored.LocalStorage
- Toast notifications through Blazored.Toast
- User-list page and edit-page scaffold
- Configurable API base URL
- PWA service worker and web manifest
- RTL/Persian-ready login UI elements

## Stack

- .NET 9
- Blazor WebAssembly
- Razor Components
- Blazored.LocalStorage
- Blazored.Toast
- System.IdentityModel.Tokens.Jwt
- Bootstrap

## Structure

```text
BlazorFrontend/
├── Auth/
│   ├── CustomAuthStateProvider.cs
│   ├── JwtAuthorizationMessageHandler.cs
│   └── UnauthorizedHandle.cs
├── Layout/
├── Models/
│   ├── Base/
│   ├── Dtos/
│   └── Entities/
├── Pages/
│   ├── Auth/
│   ├── Login.razor
│   ├── Register.razor
│   └── Weather.razor
├── Services/
│   ├── AuthService.cs
│   └── UserManagementService.cs
├── wwwroot/
│   ├── appsettings.json
│   ├── manifest.webmanifest
│   └── service-worker.js
├── App.razor
└── Program.cs
```

## Configuration

Set the backend URL in `wwwroot/appsettings.Development.json`:

```json
{
  "ApiBaseUrl": "https://localhost:55281/"
}
```

## Run Locally

```bash
dotnet restore
dotnet run
```

Current development URL:

```text
https://localhost:7140
```

The backend CORS policy must allow this origin.

## Authentication Flow

1. The login or registration form sends credentials to the API.
2. The API returns a JWT access token.
3. `AuthService` stores the token in local storage.
4. `CustomAuthStateProvider` parses claims and updates the UI authentication state.
5. `JwtAuthorizationMessageHandler` attaches the token to API requests.
6. `UnauthorizedHandler` clears the token and redirects to `/login` after a 401 response.
7. `AuthorizeRouteView` protects pages that require authorization.

## Main Pages

| Route | Description |
|---|---|
| `/` | Home page |
| `/login` | User login |
| `/register` | User registration |
| `/users` | Authenticated user list |
| `/users/edit/{id}` | User-edit scaffold |
| `/weather` | Protected API-call sample |

## Production Notes

- Browser local storage is acceptable for a demo but increases exposure if an XSS vulnerability exists.
- Consider secure HttpOnly cookies or a carefully designed token strategy for production.
- Remove duplicate methods and unused dependencies.
- Add loading, empty, and error states to user-management pages.
- Add a real update form and API integration to the edit page.
- Protect administrative routes with roles or policies.
- Replace the vendored Blazored.Toast source project with a normal package reference unless local source modification is intentional.
- Add component and end-to-end tests.
