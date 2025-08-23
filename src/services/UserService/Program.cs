using Common.Data;
using Common.Extensions;
using Common.Middleware;
using Common.Services;
using Common.Caching;
using Common.Performance;
using Common.Monitoring; // 🔧 ADD: Monitoring namespace
using Contracts.Repositories;
using Contracts.Services;
using Contracts.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserService.Mappings;
using UserService.Services; // 🔧 FIX: Add this using directive for ICacheInvalidationService
using UserService.Middleware;
using UserService.HealthChecks;
using StackExchange.Redis;
using Serilog;
using Common.Constants;
using UserService.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus; // 🔧 FIX: Alias to resolve ambiguity

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

// 🔧 FIX: Declare variables at the correct scope before the try block
var redisConnectionString = Environment.GetEnvironmentVariable("Redis__ConnectionString")
    ?? builder.Configuration["Redis:ConnectionString"]
    ?? builder.Configuration.GetConnectionString("Redis");

var isPerformanceOrTesting = builder.Environment.EnvironmentName == "Performance" 
                           || builder.Environment.EnvironmentName == "Testing";

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
    Console.WriteLine($"🔍 Redis connection string resolved to: {redisConnectionString}");
    Console.WriteLine($"🧪 Performance/Testing environment: {isPerformanceOrTesting}");

    // Only configure Redis if we have a connection string AND we're not in performance/testing
    if (!string.IsNullOrEmpty(redisConnectionString) && !isPerformanceOrTesting)
    {
        Console.WriteLine("✅ Configuring full Redis services");
        
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
        builder.Services.AddSingleton<IMetricsCollector, RedisMetricsCollector>();
        builder.Services.AddStackExchangeRedisCache(o => o.Configuration = redisConnectionString);
    }
    else
    {
        Console.WriteLine("⚡ Using in-memory cache services (Performance/Testing mode)");
        
        // Use in-memory implementations for performance tests and when Redis is not available
        builder.Services.AddMemoryCache();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
        builder.Services.AddSingleton<IPermissionCache, MemoryPermissionCache>();
        builder.Services.AddScoped<ICacheInvalidationService, NoOpCacheInvalidationService>(); // 🔧 FIX: Use correct implementation
        builder.Services.AddSingleton<IMetricsCollector, MemoryMetricsCollector>();
    }

    Console.WriteLine("✅ Cache services configured");

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

    // 🔧 PHASE 11: Enhanced Security and Monitoring Services
    Console.WriteLine("🔐 Registering Enhanced Security and Monitoring services...");
    Log.Information("Registering Enhanced Security and Monitoring services");
    
    // Enhanced Security
    builder.Services.AddEnhancedSecurity();
    builder.Services.AddEnhancedAuthorizationPolicies();
    
    // 🆕 ADD: Monitoring Service
    builder.Services.AddScoped<IMonitoringService, MonitoringService>();
    
    Console.WriteLine("✅ Enhanced Security and Monitoring services registered successfully");
    Log.Information("Enhanced Security and Monitoring services registered successfully");

    // 🆕 ADD: Phase 11 Session 3 - Compliance and Alert Services
    Console.WriteLine("📋 Registering Compliance and Alert services...");
    Log.Information("Registering Compliance and Alert services");

    builder.Services.AddScoped<IComplianceReportingService, ComplianceReportingService>();
    builder.Services.AddScoped<ISecurityAlertService, SecurityAlertService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddHttpClient<ISecurityAlertService, SecurityAlertService>();

    Console.WriteLine("✅ Compliance and Alert services registered successfully");
    Log.Information("Compliance and Alert services registered successfully");

    // 🔧 PHASE 11: Enhanced Health Checks
    Console.WriteLine("🩺 Registering Enhanced Health Checks...");
    builder.Services.AddHealthChecks()
        // Basic infrastructure health checks
        .AddCheck<RedisHealthCheck>("redis", 
            failureStatus: HealthStatus.Unhealthy, // 🔧 FIX: Now uses Microsoft's HealthStatus
            tags: new[] { "cache", "redis", "infrastructure", "critical" })
        .AddCheck<DatabaseHealthCheck>("database", 
            failureStatus: HealthStatus.Unhealthy, // 🔧 FIX: Now uses Microsoft's HealthStatus
            tags: new[] { "db", "sql", "infrastructure", "critical" })
        
        // 🆕 ADD: Enhanced health checks from Phase 11
        .AddCheck<EnhancedDatabaseHealthCheck>("enhanced_database",
            failureStatus: HealthStatus.Degraded, // 🔧 FIX: Now uses Microsoft's HealthStatus
            tags: new[] { "db", "enhanced", "performance", "monitoring" })
        .AddCheck<EnhancedPermissionCacheHealthCheck>("enhanced_cache",
            failureStatus: HealthStatus.Degraded, // 🔧 FIX: Now uses Microsoft's HealthStatus
            tags: new[] { "cache", "enhanced", "performance", "monitoring" })
        .AddCheck<EnhancedAuthorizationHealthCheck>("enhanced_authorization",
            failureStatus: HealthStatus.Degraded, // 🔧 FIX: Now uses Microsoft's HealthStatus
            tags: new[] { "auth", "enhanced", "performance", "monitoring" })
        .AddCheck<SystemHealthCheck>("system_overall",
            failureStatus: HealthStatus.Degraded, // 🔧 FIX: Now uses Microsoft's HealthStatus
            tags: new[] { "system", "overall", "monitoring", "comprehensive" })
        
        // Performance metrics health check
        .AddCheck<PerformanceHealthCheck>("performance",
            failureStatus: HealthStatus.Degraded, // 🔧 FIX: Now uses Microsoft's HealthStatus
            tags: new[] { "performance", "cache", "metrics" });
    
    Console.WriteLine("✅ Enhanced Health Checks registered");

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
            
        // 🆕 ADD: Monitoring-specific policies
        options.AddPolicy("SystemMonitor", policy =>
            policy.RequireClaim("permission", "system.monitor"));
        options.AddPolicy("SystemAdmin", policy =>
            policy.RequireClaim("permission", "system.manage"));
        
        // 🆕 ADD: Compliance-specific policies (add to existing options.AddPolicy calls)
        options.AddPolicy("ComplianceReports", policy =>
            policy.RequireClaim("permission", "compliance.generate_reports"));
        options.AddPolicy("ComplianceView", policy =>
            policy.RequireClaim("permission", "compliance.view_reports"));
        options.AddPolicy("ComplianceExport", policy =>
            policy.RequireClaim("permission", "compliance.export_reports"));
        options.AddPolicy("SecurityAlerts", policy =>
            policy.RequireClaim("permission", "security.view_alerts"));
        options.AddPolicy("SecurityAlertManage", policy =>
            policy.RequireClaim("permission", "security.manage_alerts"));
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

    // 🔧 PHASE 11: Enhanced Security Middleware
    Console.WriteLine("🔐 Configuring Enhanced Security middleware...");
    Log.Information("Configuring Enhanced Security middleware");
    
    app.UseEnhancedSecurity();
    
    Console.WriteLine("✅ Enhanced Security middleware configured successfully");
    Log.Information("Enhanced Security middleware configured successfully");

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    Console.WriteLine("✅ Middleware pipeline configured");

    // 🔧 PHASE 11: Enhanced Health Check Endpoints
    Console.WriteLine("🩺 Mapping Enhanced Health Check endpoints...");
    
    // 1. Primary health endpoint (comprehensive overview)
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        AllowCachingResponses = false,
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK, // 🔧 FIX: Microsoft's HealthStatus
            [HealthStatus.Degraded] = StatusCodes.Status200OK, // 🔧 FIX: Microsoft's HealthStatus
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable // 🔧 FIX: Microsoft's HealthStatus
        },
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                environment = app.Environment.EnvironmentName,
                service = "UserService",
                version = "1.0.0-Phase11",
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration,
                    description = e.Value.Description,
                    tags = e.Value.Tags,
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

    // 2. Critical infrastructure health (database + redis)
    app.MapHealthChecks("/health/critical", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("critical"),
        AllowCachingResponses = false,
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                message = report.Status == HealthStatus.Healthy ? "All critical systems operational" : "Critical system issues detected", // 🔧 FIX: Microsoft's HealthStatus
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

    // 3. Enhanced monitoring health checks
    app.MapHealthChecks("/health/enhanced", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("enhanced") || check.Tags.Contains("monitoring"),
        AllowCachingResponses = false,
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                message = "Enhanced Security & Monitoring Health Status",
                phase = "Phase 11 - Enhanced Security & Monitoring",
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration,
                    description = e.Value.Description,
                    data = e.Value.Data,
                    tags = e.Value.Tags
                })
            };
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            }));
        }
    });

    // 4. Performance and cache health
    app.MapHealthChecks("/health/performance", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("performance") || check.Tags.Contains("cache"),
        AllowCachingResponses = false,
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                message = "Performance and Caching Health Status",
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

    // 5. System overview health check
    app.MapHealthChecks("/health/system", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("system"),
        AllowCachingResponses = false,
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                duration = report.TotalDuration,
                message = "Overall System Health with Performance Scoring",
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

    // 6. Kubernetes/Container health endpoints
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("critical"),
        AllowCachingResponses = false
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false // Only checks if the app is running
    });

    // 7. Legacy simple health endpoint (keeping for backward compatibility)
    app.MapGet("/health/simple", () => Results.Ok(new { 
        Status = "Healthy", 
        Timestamp = DateTime.UtcNow,
        Service = "UserService",
        Phase = "Phase 11 - Enhanced Security & Monitoring"
    }));

    Console.WriteLine("✅ Enhanced Health Check endpoints mapped");

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
            .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;

        foreach (var address in addresses ?? Enumerable.Empty<string>())
        {
            Console.WriteLine($"Now listening on: {address}");
            Log.Information("Now listening on: {Address}", address);
        }
        
        Console.WriteLine("🩺 Enhanced Health Check endpoints available:");
        Console.WriteLine("  - GET /health (comprehensive overview)");
        Console.WriteLine("  - GET /health/critical (database + Redis status)");
        Console.WriteLine("  - GET /health/enhanced (Phase 11 enhanced monitoring)");
        Console.WriteLine("  - GET /health/performance (cache and performance metrics)");
        Console.WriteLine("  - GET /health/system (overall system health score)");
        Console.WriteLine("  - GET /health/ready (Kubernetes readiness)");
        Console.WriteLine("  - GET /health/live (Kubernetes liveness)");
        Console.WriteLine("📊 Enhanced Security & Monitoring features:");
        Console.WriteLine("  - Rate limiting with tenant awareness");
        Console.WriteLine("  - Security event detection and logging");
        Console.WriteLine("  - Performance metrics collection with Redis storage");
        Console.WriteLine("  - Enhanced audit trail for all actions");
        Console.WriteLine("  - Comprehensive health monitoring with performance scoring");
        Console.WriteLine("  - Real-time system metrics and alerts");
        Log.Information("Enhanced Security & Monitoring (Phase 11) configured and operational");
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
    Log.Information("Starting UserService with Phase 11 Enhanced Security & Monitoring");
    Console.WriteLine("=== UserService Starting with Phase 11 Enhanced Security & Monitoring ===");

    try
    {
        Console.WriteLine("🔧 Testing cache connection...");
        
        // Only test Redis if we actually configured it
        if (!isPerformanceOrTesting && !string.IsNullOrEmpty(redisConnectionString))
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
        else
        {
            Console.WriteLine("✅ In-memory cache configured (Performance/Testing mode)");
            Log.Information("In-memory cache configured for Performance/Testing environment");
        }
    }
    catch (Exception ex) when (ex.GetType().Name.Contains("Redis"))
    {
        Console.WriteLine($"⚠️  Redis connection failed: {ex.Message}");
        Log.Warning(ex, "Redis connection test failed, using fallback caching");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Cache connection test error: {ex.Message}");
        Log.Warning(ex, "Cache connection test failed with unexpected error");
    }

    Console.WriteLine("🔧 Seeding database...");
    await app.Services.SeedDatabaseAsync();
    Console.WriteLine("✅ Database seeded");

    Console.WriteLine("🔧 Seeding monitoring user...");
    await MonitoringUserSeeder.SeedAsync(app.Services);
    Console.WriteLine("✅ Monitoring user seeded (monitor@local / ChangeMe123!)");

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
