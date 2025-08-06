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

// Add Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add common services (JWT, configuration, etc.)
builder.Services.AddCommonServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// FIXED: AutoMapper configuration - Correct method signature for AutoMapper 15.0.1
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MappingProfile>();
});

// FIXED: Repository registrations - Simplified approach
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITenantManagementRepository, TenantManagementRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Add services
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();
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
    app.UseSwaggerUI();
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

try
{
    Log.Information("Starting AuthService");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AuthService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
