using Asp.Versioning;
using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;
using System.Security.Claims;
using System.Text;

// Create the application builder
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configure API versioning.
/// Sets the default API version to v1.0 and enables reporting of supported versions.
/// </summary>
builder.Services.AddApiVersioning(options =>
{
    // Default API version when none is specified
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // Use the default version if the client does not specify one
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Include supported/deprecated API versions in response headers
    options.ReportApiVersions = true;
})
.AddMvc();

// Register MVC controllers
builder.Services.AddControllers();

// Register services required for Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/// <summary>
/// Configure Entity Framework Core with SQL Server.
/// The connection string is read from appsettings.json.
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// Configure ASP.NET Core Identity.
/// Defines password rules and user requirements.
/// </summary>
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password must contain at least one number
        options.Password.RequireDigit = true;

        // Password must contain lowercase letters
        options.Password.RequireLowercase = true;

        // Uppercase letters are optional
        options.Password.RequireUppercase = false;

        // Special characters are optional
        options.Password.RequireNonAlphanumeric = false;

        // Minimum password length
        options.Password.RequiredLength = 6;

        // Every user must have a unique email address
        options.User.RequireUniqueEmail = true;
    })
    // Store Identity data using Entity Framework Core
    .AddEntityFrameworkStores<ApplicationDbContext>()

    // Register token providers (used for password reset, email confirmation, etc.)
    .AddDefaultTokenProviders();

/// <summary>
/// Read JWT configuration values from appsettings.json.
/// </summary>
var jwtSection = builder.Configuration.GetSection("Jwt");

// Secret key used for signing JWT tokens
var key = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is missing.");

// Token issuer
var issuer = jwtSection["Issuer"];

// Token audience
var audience = jwtSection["Audience"];

/// <summary>
/// Configure JWT Bearer Authentication.
/// </summary>
builder.Services
    .AddAuthentication(options =>
    {
        // Use JWT Bearer authentication by default
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Configure token validation rules
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate token issuer
            ValidateIssuer = true,

            // Validate intended audience
            ValidateAudience = true,

            // Validate expiration time
            ValidateLifetime = true,

            // Validate the signing key
            ValidateIssuerSigningKey = true,

            // Expected issuer
            ValidIssuer = issuer,

            // Expected audience
            ValidAudience = audience,

            // Secret key used to validate token signature
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            // No additional tolerance for expired tokens
            ClockSkew = TimeSpan.Zero
        };

        // JWT event handlers
        options.Events = new JwtBearerEvents
        {
            // Executed when authentication fails
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },

            // Executed when authentication is required but no valid token is provided
            OnChallenge = context =>
            {
                Console.WriteLine("JWT Challenge: 401 Unauthorized");
                return Task.CompletedTask;
            },

            // Executed after successful token validation
            OnTokenValidated = context =>
            {
                // Retrieve authenticated username from claims
                var userName = context.Principal?.FindFirst(ClaimTypes.Name)?.Value;

                Console.WriteLine($"JWT Token Validated for user: {userName}");

                return Task.CompletedTask;
            }
        };
    });

/// <summary>
/// Register application services with Dependency Injection.
/// </summary>

// Service responsible for user management operations
builder.Services.AddScoped<UserManagementService>();

// Enable authorization services
builder.Services.AddAuthorization();

// Service responsible for generating JWT tokens
builder.Services.AddScoped<JwtTokenService>();

/// <summary>
/// Configure Cross-Origin Resource Sharing (CORS).
/// Allows requests from Blazor frontend applications.
/// </summary>
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy
            // Allowed frontend origins
            .WithOrigins(
                "https://localhost:7067",
                "http://localhost:5169",
                "https://localhost:55277",
                "http://localhost:55277",
                "https://localhost:7140")

            // Allow all HTTP headers
            .AllowAnyHeader()

            // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
            .AllowAnyMethod();
    });
});

// Build the application
var app = builder.Build();

/// <summary>
/// Configure middleware for Development environment.
/// Enables Swagger UI.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        // Swagger endpoint
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthApi V1");

        // Make Swagger available at the application root
        c.RoutePrefix = string.Empty;
    });
}

/// <summary>
/// Configure the HTTP request pipeline.
/// </summary>

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Enable configured CORS policy
app.UseCors("AllowBlazor");

// Authenticate incoming requests
app.UseAuthentication();

// Authorize authenticated users
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// Start the application
app.Run();