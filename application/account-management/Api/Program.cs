using System.Security.Claims;
using PlatformPlatform.AccountManagement.Api.Auth;
using PlatformPlatform.AccountManagement.Api.Tenants;
using PlatformPlatform.AccountManagement.Api.Users;
using PlatformPlatform.AccountManagement.Application;
using PlatformPlatform.AccountManagement.Infrastructure;
using PlatformPlatform.SharedKernel.ApiCore;
using PlatformPlatform.SharedKernel.ApiCore.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure services for the Application, Infrastructure, and Api layers like Entity Framework, Repositories, MediatR,
// FluentValidation validators, Pipelines.
builder.Services
    .AddApplicationServices()
    .AddDatabaseContext(builder)
    .AddInfrastructureServices()
    .AddAuthenticationServices()
    .AddApiCoreServices(builder);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Add common configuration for all APIs like Swagger, HSTS, DeveloperExceptionPage, and run EF database migrations.
app.AddApiCoreConfiguration<AccountManagementDbContext>();
app.UseWebAppMiddleware();

app.MapUserEndpoints();
app.MapTenantEndpoints();
app.MapRegistrationEndpoints();
app.MapAuthenticationEndpoints();
app.MapPasswordEndpoints();

app.MapGet("/api/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name} Role: {user.FindFirst(ClaimTypes.Role)}. My secret")
    .RequireAuthorization();
app.MapGet("/api/secretOwner", () => "Hello Owner. My secret")
    .RequireAuthorization("RequireOwnerRole");
app.MapGet("/api/secretMember", () => "Hello Member. My secret")
    .RequireAuthorization("RequireMemberRole");

app.Run();