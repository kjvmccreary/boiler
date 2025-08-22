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

// 🔧 ENHANCED STARTUP DIAGNOSTICS
Console.WriteLine("=== UserService Startup Diagnostics ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
Console.WriteLine($"Content Root: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Running in Container: {Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")}");

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// QUICK DIAGNOSTIC
var monitoringEnabledRaw = builder.Configuration["Monitoring:Enabled"];
Console.WriteLine($"[Startup] Monitoring:Enabled raw config value = '{monitoringEnabledRaw}'");
Log.Information("Monitoring:Enabled = {Value}", monitoringEnabledRaw);

try
{
    Console.WriteLine("🔧 Registering base services...");
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    Console.WriteLine("✅ Base services registered");

    Console.WriteLine("🔧 Configuring Swagger...");
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "UserService API",
            Version = "v1",
            Description = "User Management and Profile Services with Redis Caching and Enhanced Security Monitoring"
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
    Console.WriteLine("✅ Swagger configured");

    Console.WriteLine("🔧 Registering common services...");
    builder.Services.AddCommonServices(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddDynamicAuthorization();
    Console.WriteLine("✅ Common services registered");

    Console.WriteLine("🔧 Configuring Redis...");
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
        configuration.AllowAdmin = true;

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
    Console.WriteLine("✅ Redis configured");

    Console.WriteLine("🔧 Registering database services...");
    builder.Services.AddDatabase(builder.Configuration);
    Console.WriteLine("✅ Database services registered");

    Console.WriteLine("🔧 Registering user services...");
    builder.Services.AddAutoMapperProfiles(typeof(UserMappingProfile));
    builder.Services.AddScoped<IPermissionService, CachedPermissionService>();
    builder.Services.AddScoped<Contracts.User.IUserService, UserServiceImplementation>();
    builder.Services.AddScoped<Contracts.User.IUserProfileService, UserProfileService>();
    builder.Services.AddScoped<ITenantService, UserService.Services.TenantService>();
    builder.Services.AddScoped<IRoleTemplateService, RoleTemplateService>();
    builder.Services.AddFluentValidation();
    Console.WriteLine("✅ User services registered");

    // ✅ PHASE 11: Enhanced Security and Monitoring
    Console.WriteLine("🔐 Registering Enhanced Security services...");
    Log.Information("Registering Enhanced Security services");
    
    builder.Services.AddEnhancedSecurity();
    builder.Services.AddEnhancedAuthorizationPolicies();
    
    Console.WriteLine("✅ Enhanced Security services registered successfully");
    Log.Information("Enhanced Security services registered successfully");

    Console.WriteLine("🔧 Registering health checks...");
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
    Console.WriteLine("✅ Health checks registered");

    Console.WriteLine("🔧 Configuring authorization policies...");
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
            
        options.AddPolicy("HealthCheckAccess", policy =>
            policy.RequireAssertion(context =>
            {
                var path = context.Resource as Microsoft.AspNetCore.Http.DefaultHttpContext;
                return path?.Request.Path.StartsWithSegments("/health") == true;
            }));
    });
    Console.WriteLine("✅ Authorization policies configured");

    Console.WriteLine("🔧 Configuring CORS...");
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });
    Console.WriteLine("✅ CORS configured");

    Console.WriteLine("🔧 Registering additional services...");
    builder.Services.AddScoped<IPasswordService, Common.Services.PasswordService>();
    builder.Services.AddSingleton<IPerformanceMetricsService, PerformanceMetricsService>();
    Console.WriteLine("✅ Additional services registered");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ SERVICE REGISTRATION ERROR: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Log.Fatal(ex, "Failed to register services during startup");
    throw;
}

Console.WriteLine("🏗️ Building application...");
var app = builder.Build();
Console.WriteLine("✅ Application built successfully");

try
{
    Console.WriteLine("🔧 Configuring middleware pipeline...");
    
    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine("🔧 Adding development middleware...");
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService API V1");
            c.RoutePrefix = "swagger";
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });
        Console.WriteLine("✅ Development middleware added");
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

    // ✅ PHASE 11: Enhanced Security Middleware
    Console.WriteLine("🔐 Configuring Enhanced Security middleware...");
    Log.Information("Configuring Enhanced Security middleware");
    
    app.UseEnhancedSecurity();
    
    Console.WriteLine("✅ Enhanced Security middleware configured successfully");
    Log.Information("Enhanced Security middleware configured successfully");

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    Console.WriteLine("✅ Middleware pipeline configured");

    Console.WriteLine("🔧 Mapping health check endpoints...");
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
    Console.WriteLine("✅ Health check endpoints mapped");

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
            .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;

        foreach (var address in addresses ?? Enumerable.Empty<string>())
        {
            Console.WriteLine($"Now listening on: {address}");
            Log.Information("Now listening on: {Address}", address);
        }
        
        Console.WriteLine("Health check endpoints available:");
        Console.WriteLine("  - GET /health (basic health status)");
        Console.WriteLine("  - GET /health/cache (Redis and cache status)");
        Console.WriteLine("  - GET /health/infrastructure (database + Redis)");
        Console.WriteLine("  - GET /health/performance (performance metrics)");
        Console.WriteLine("Enhanced Security & Monitoring features:");
        Console.WriteLine("  - Rate limiting with tenant awareness");
        Console.WriteLine("  - Security event detection and logging");
        Console.WriteLine("  - Performance metrics collection");
        Console.WriteLine("  - Enhanced audit trail for all actions");
        Log.Information("Health check endpoints and Enhanced Security features configured and available");
    });
}
catch (Exception ex)
{
    Console.WriteLine($"❌ MIDDLEWARE CONFIGURATION ERROR: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Log.Fatal(ex, "Failed to configure middleware during startup");
    throw;
}

try
{
    Log.Information("Starting UserService with Redis caching and Enhanced Security enabled");
    Console.WriteLine("=== UserService Starting with Redis Caching and Enhanced Security ===");

    try
    {
        Console.WriteLine("🔧 Testing Redis connection...");
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

    Console.WriteLine("🔧 Seeding database...");
    await app.Services.SeedDatabaseAsync();
    Console.WriteLine("✅ Database seeded");

    Console.WriteLine("🔧 Seeding monitoring user...");
    await MonitoringUserSeeder.SeedAsync(app.Services);
    Console.WriteLine("✅ Monitoring user seeded");

    Console.WriteLine("🚀 Starting application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "UserService terminated unexpectedly");
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
