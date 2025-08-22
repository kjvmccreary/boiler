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
using StackExchange.Redis;
using Serilog;
using Common.Constants;
using UserService.Infrastructure;

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
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

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
