#region NameSpaces
using Blazored.LocalStorage;
using Blazored.Toast;
using BlazorFrontend;
using BlazorFrontend.Auth;
using BlazorFrontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
#endregion

// Creates the WebAssembly application builder.
// This is the entry point for configuring services and application settings.
var builder = WebAssemblyHostBuilder.CreateDefault(args);


// Registers a handler responsible for processing unauthorized API responses.
// This service can redirect users or handle expired authentication sessions.
builder.Services.AddTransient<UnauthorizedHandler>();


// Registers the main Blazor application component.
// "#app" refers to the HTML element where the Blazor application will be rendered.
builder.RootComponents.Add<BlazorFrontend.App>("#app");


// Registers the component responsible for managing the document head section.
// Used for dynamically changing page titles, metadata, etc.
builder.RootComponents.Add<HeadOutlet>("head::after");


// Reads the backend API base URL from application configuration.
// If no value exists, the default local API URL is used.
var apiBaseUrl =
    builder.Configuration["ApiBaseUrl"]
    ?? "https://localhost:55281/";


// Registers browser local storage support.
// Used for storing JWT tokens and client-side settings.
builder.Services.AddBlazoredLocalStorage();


// Registers a custom HTTP message handler.
// This handler attaches JWT tokens to outgoing API requests.
builder.Services.AddScoped<JwtAuthorizationMessageHandler>();


// Registers a named HttpClient for communicating with the backend API.
builder.Services.AddHttpClient("ApiClient", client =>
{
    // Sets the base address for all API requests.
    client.BaseAddress = new Uri(apiBaseUrl);

})
// Adds JWT authentication handling to outgoing requests.
// Automatically attaches the access token when available.
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>()

// Handles unauthorized responses such as HTTP 401.
// Typically used for logout or redirecting to login.
.AddHttpMessageHandler<UnauthorizedHandler>();


// Registers HttpClient as the default injected client.
// Components and services can inject HttpClient directly.
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>()
      .CreateClient("ApiClient"));

// Enables authorization support in Blazor WebAssembly.
// Provides authorization services without ASP.NET Core server dependencies.
builder.Services.AddAuthorizationCore();

// Registers custom authentication state management.
// This service reads authentication information from JWT tokens
// and notifies the application when authentication changes.
builder.Services.AddScoped<CustomAuthStateProvider>();

// Replaces the default AuthenticationStateProvider
// with the custom JWT-based implementation.
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());

// Registers authentication service.
// Handles login, registration, logout, and authentication API calls.
builder.Services.AddScoped<AuthService>();

// Registers user management service.
// Handles user-related API communication.
builder.Services.AddScoped<UserManagementService>();

// Enables toast notifications throughout the Blazor application.
// Used for displaying success/error messages to users.
builder.Services.AddBlazoredToast();

// Builds and runs the Blazor WebAssembly application.
await builder.Build().RunAsync();