using Common.Data;
using Common.Extensions;
using Common.Middleware;
using Common.Services;
using Common.Caching;
using Contracts.Repositories;
using Contracts.Services;
using Contracts.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserService.Mappings;
using UserService.Services;
using UserService.Middleware;
using StackExchange.Redis;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UserService API",
        Version = "v1",
        Description = "User Management and Profile Services with Redis Caching"
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

// ✅ CRITICAL FIX: Add dynamic authorization for permission-based policies
builder.Services.AddDynamicAuthorization();

// 🔧 FIXED: Improved Redis connection string resolution for Docker environments
var redisConnectionString = Environment.GetEnvironmentVariable("Redis__ConnectionString") 
    ?? builder.Configuration["Redis:ConnectionString"]
    ?? builder.Configuration.GetConnectionString("Redis")
    ?? (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" && 
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" 
        ? "redis:6379"  // Docker container networking
        : "localhost:6379");  // Local development

// 🔧 ADD: Debug logging to see which connection string is being used
Console.WriteLine($"🔍 Redis connection string resolved to: {redisConnectionString}");
Console.WriteLine($"🐳 Running in container: {Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")}");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    configuration.ConnectTimeout = 15000;  // ✅ INCREASED: Give more time for Docker networking
    configuration.SyncTimeout = 15000;    // ✅ INCREASED: Give more time for Docker networking
    configuration.ConnectRetry = 3;       // ✅ NEW: Retry connections
    
    // ✅ NEW: Add logging for connection attempts
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
        
        // ✅ NEW: Create a "null" multiplexer that won't crash the app
        // This allows the service to start even if Redis is unavailable
        throw; // Re-throw for now, but we could return a NullConnectionMultiplexer
    }
});

// ✅ NEW: Register cache services for Phase 10
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IPermissionCache, RedisPermissionCache>();

// ✅ NEW: Register cache invalidation service
builder.Services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();

// ✅ FIXED: Add distributed caching with Redis without BuildServiceProvider warning
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});

// USE DATABASE EXTENSION (includes DbContext + Repositories)
builder.Services.AddDatabase(builder.Configuration);

// FIXED: Use Common extension for AutoMapper
builder.Services.AddAutoMapperProfiles(typeof(UserMappingProfile));

// ✅ UPDATED: Replace PermissionService with CachedPermissionService
// Remove the old registration and add the cached version
builder.Services.AddScoped<IPermissionService, CachedPermissionService>();

// Add your service implementations
builder.Services.AddScoped<Contracts.User.IUserService, UserServiceImplementation>();
builder.Services.AddScoped<Contracts.User.IUserProfileService, UserProfileService>();

// Register tenant services
builder.Services.AddScoped<ITenantService, UserService.Services.TenantService>();
builder.Services.AddScoped<IRoleTemplateService, RoleTemplateService>();

// Add FluentValidation
builder.Services.AddFluentValidation();

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin,SuperAdmin"));

    options.AddPolicy("OwnerOrAdmin", policy =>
        policy.RequireAssertion(context =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = context.User.IsInRole("Admin") || context.User.IsInRole("SuperAdmin");
            return isAdmin || (userId != null);
        }));
});

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

// Add this line to register the password service
builder.Services.AddScoped<IPasswordService, Common.Services.PasswordService>();

// ✅ NEW: Register Performance Metrics Service for Phase 10 Session 3
builder.Services.AddSingleton<IPerformanceMetricsService, PerformanceMetricsService>();

var app = builder.Build();

// Configure the HTTP request pipeline
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

// Add validation middleware
app.UseMiddleware<ValidationMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// 🔧 FIX: DevelopmentTenantMiddleware FIRST (adds X-Tenant-ID header in development)
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<DevelopmentTenantMiddleware>();
}

// 🔧 FIX: TenantMiddleware SECOND (processes the tenant header)
app.UseTenantResolution();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

// ➕ ADD: Explicit logging for startup visibility
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
    Log.Information("Starting UserService with Redis caching enabled");
    Console.WriteLine("=== UserService Starting with Redis Caching ===");
    
    // ✅ IMPROVED: Test Redis connection with better error handling
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
    
    // 🔧 ADD: Automatic database seeding on startup
    await app.Services.SeedDatabaseAsync();
    
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

// ADDED: Make Program class accessible for testing
public partial class Program { }
