using AuthService.Mappings;
using AuthService.Services;
using Common.Data;
using Common.Extensions;
using Common.Middleware;
using Common.Repositories;
using Common.Services;
using Contracts.Auth;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // ADD: For Swagger JWT configuration
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure AutoMapper license early if provided
var licenseKey = builder.Configuration["AutoMapperSettings:LicenseKey"] ??
                 Environment.GetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY");

if (!string.IsNullOrEmpty(licenseKey))
{
    // Set license key before any AutoMapper operations
    Environment.SetEnvironmentVariable("AUTOMAPPER_LICENSE_KEY", licenseKey);
}

// ✅ FIXED: Configure Serilog for .NET 9
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ENHANCED: Configure Swagger with JWT authentication support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AuthService API",
        Version = "v1",
        Description = "Authentication Service with RBAC"
    });

    // JWT Authentication configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add common services (JWT, configuration, etc.)
builder.Services.AddCommonServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDynamicAuthorization();

// FIX: Use AddDatabase extension which includes all RBAC repositories and services
builder.Services.AddDatabase(builder.Configuration);

// 🔧 ADD: AutoMapper registration (this line is missing!)
builder.Services.AddAutoMapperProfiles(typeof(MappingProfile));

// Add AuthService-specific services
builder.Services.AddScoped<IPasswordService, Common.Services.PasswordService>();
builder.Services.AddScoped<ITokenService, EnhancedTokenService>(); // ← Use EnhancedTokenService
builder.Services.AddScoped<IAuthService, AuthServiceImplementation>();

// Add FluentValidation
builder.Services.AddFluentValidation();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService API V1");
        c.RoutePrefix = "swagger"; // Sets Swagger UI at /swagger
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });
}

app.UseSerilogRequestLogging();

// Add validation middleware
app.UseMiddleware<ValidationMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

// ➕ ADD: Explicit logging for startup visibility (like other services)
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
        .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;
    
    foreach (var address in addresses ?? Enumerable.Empty<string>())
    {
        Console.WriteLine($"Now listening on: {address}");
        Log.Information("Now listening on: {Address}", address);
    }
});

try
{
    Log.Information("Starting AuthService");
    Console.WriteLine("=== AuthService Starting ===");
    
    // 🔧 ADD: Automatic database seeding on startup
    await app.Services.SeedDatabaseAsync();
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthService terminated unexpectedly");
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}

// ADDED: Make Program class accessible for testing
public partial class Program { }
