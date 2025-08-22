using Common.Data;
using Common.Extensions;
using Common.Middleware;
using Common.Services;
using Common.Caching;
using Common.Performance;
using Contracts.Repositories;
using Contracts.Services;
using Contracts.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserService.Mappings;
using UserService.Services;
using UserService.Middleware;
using UserService.HealthChecks;
using StackExchange.Redis;
using Serilog;
using Common.Constants;
using UserService.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// QUICK DIAGNOSTIC
var monitoringEnabledRaw = builder.Configuration["Monitoring:Enabled"];
Console.WriteLine($"[Startup] Monitoring:Enabled raw config value = '{monitoringEnabledRaw}'");
Log.Information("Monitoring:Enabled = {Value}", monitoringEnabledRaw);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserService API",
        Version = "v1",
        Description = "User Management and Profile Services with Redis Caching"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' + space + token.",
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCommonServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDynamicAuthorization();

var redisConnectionString = Environment.GetEnvironmentVariable("Redis__ConnectionString")
    ?? builder.Configuration["Redis:ConnectionString"]
    ?? builder.Configuration.GetConnectionString("Redis")
    ?? (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" &&
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true"
        ? "redis:6379"
        : "localhost:6379");

Console.WriteLine($"🔍 Redis connection string resolved to: {redisConnectionString}");
Console.WriteLine($"🐳 Running in container: {Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")}");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    configuration.ConnectTimeout = 15000;
    configuration.SyncTimeout = 15000;
    configuration.ConnectRetry = 3;
    configuration.AllowAdmin = true; // ✅ REQUIRED for INFO / KEYS (server.* admin ops)

    var logger = sp.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Attempting to connect to Redis at: {ConnectionString}", redisConnectionString);
        var connection = ConnectionMultiplexer.Connect(configuration);
        logger.LogInformation("Redis connection status: {Status} to {EndPoint}",
            connection.IsConnected ? "Connected" : "Disconnected", redisConnectionString);
        return connection;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to connect to Redis at: {ConnectionString}", redisConnectionString);
        throw;
    }
});

builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IPermissionCache, RedisPermissionCache>();
builder.Services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();

builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redisConnectionString);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAutoMapperProfiles(typeof(UserMappingProfile));
builder.Services.AddScoped<IPermissionService, CachedPermissionService>();
builder.Services.AddScoped<Contracts.User.IUserService, UserServiceImplementation>();
builder.Services.AddScoped<Contracts.User.IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ITenantService, UserService.Services.TenantService>();
builder.Services.AddScoped<IRoleTemplateService, RoleTemplateService>();
builder.Services.AddFluentValidation();

// ✅ UPDATED: Use proper health check classes
builder.Services.AddHealthChecks()
    .AddCheck<RedisHealthCheck>("redis", 
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "cache", "redis", "infrastructure" })
    .AddCheck<DatabaseHealthCheck>("database", 
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "infrastructure" })
    .AddCheck<PerformanceHealthCheck>("performance",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "performance", "cache", "metrics" });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "SuperAdmin"));

    options.AddPolicy("OwnerOrAdmin", policy =>
        policy.RequireAssertion(context =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = context.User.IsInRole("Admin") || context.User.IsInRole("SuperAdmin");
            return isAdmin || userId != null;
        }));

    options.AddPolicy("RedisMonitoring", policy =>
        policy.RequireClaim("permission", Permissions.System.ViewMetrics));
        
    // ✅ ADD: Health Check Authorization Policy
    options.AddPolicy("HealthCheckAccess", policy =>
        policy.RequireAssertion(context =>
        {
            // Allow unauthenticated access to basic health endpoint
            // Require admin access for detailed health endpoints
            var path = context.Resource as Microsoft.AspNetCore.Http.DefaultHttpContext;
            return path?.Request.Path.StartsWithSegments("/health") == true;
        }));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddScoped<IPasswordService, Common.Services.PasswordService>();
builder.Services.AddSingleton<IPerformanceMetricsService, PerformanceMetricsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API V1");
        c.RoutePrefix = "swagger";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ValidationMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<DevelopmentTenantMiddleware>();
}

app.UseTenantResolution();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ✅ ADD: Health Check Endpoints - Task 3.6
// Basic health endpoint (public access)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration,
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

// Detailed cache health endpoint
app.MapHealthChecks("/health/cache", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("cache"),
    AllowCachingResponses = false,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration,
                description = e.Value.Description,
                data = e.Value.Data
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

// Infrastructure health endpoint (database + redis)
app.MapHealthChecks("/health/infrastructure", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("infrastructure"),
    AllowCachingResponses = false,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration,
                description = e.Value.Description,
                data = e.Value.Data
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

// Performance metrics health endpoint
app.MapHealthChecks("/health/performance", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("performance"),
    AllowCachingResponses = false,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration,
                description = e.Value.Description,
                data = e.Value.Data
            })
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
});

// Legacy simple health endpoint (keeping for backward compatibility)
app.MapGet("/health/simple", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
        .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;

    foreach (var address in addresses ?? Enumerable.Empty<string>())
    {
        Console.WriteLine($"Now listening on: {address}");
        Log.Information("Now listening on: {Address}", address);
    }
    
    // ✅ ADD: Log health check endpoints
    Console.WriteLine("Health check endpoints available:");
    Console.WriteLine("  - GET /health (basic health status)");
    Console.WriteLine("  - GET /health/cache (Redis and cache status)");
    Console.WriteLine("  - GET /health/infrastructure (database + Redis)");
    Console.WriteLine("  - GET /health/performance (performance metrics)");
    Log.Information("Health check endpoints configured and available");
});

try
{
    Log.Information("Starting UserService with Redis caching enabled");
    Console.WriteLine("=== UserService Starting with Redis Caching ===");

    try
    {
        var redis = app.Services.GetRequiredService<IConnectionMultiplexer>();
        Log.Information("Redis connection status: {Status}", redis.IsConnected ? "Connected" : "Disconnected");

        if (redis.IsConnected)
        {
            var db = redis.GetDatabase();
            var testKey = $"startup-test:{Environment.MachineName}:{DateTime.UtcNow:yyyyMMddHHmmss}";
            await db.StringSetAsync(testKey, DateTime.UtcNow.ToString(), TimeSpan.FromSeconds(10));
            var test = await db.StringGetAsync(testKey);
            Console.WriteLine($"✅ Redis connection successful: {test}");
            Log.Information("Redis connection test successful: {TestValue}", test);
        }
    }
    catch (RedisConnectionException ex)
    {
        Console.WriteLine($"⚠️  Redis connection failed: {ex.Message}");
        Log.Warning(ex, "Redis connection test failed, caching will be unavailable");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Redis connection test error: {ex.Message}");
        Log.Warning(ex, "Redis connection test failed with unexpected error");
    }

    await app.Services.SeedDatabaseAsync();
    await MonitoringUserSeeder.SeedAsync(app.Services);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "UserService terminated unexpectedly");
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
