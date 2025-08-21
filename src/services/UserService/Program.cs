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
        Description = "User Management and Profile Services"
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

// ✅ NEW: Configure Redis for Phase 10 Caching
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? builder.Configuration["Redis:ConnectionString"]
    ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    configuration.ConnectTimeout = 5000;
    configuration.SyncTimeout = 5000;
    
    var connection = ConnectionMultiplexer.Connect(configuration);
    
    // Log connection status
    var logger = sp.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Redis connection status: {Status}", connection.IsConnected ? "Connected" : "Disconnected");
    
    return connection;
});

// ✅ NEW: Register cache services for Phase 10
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IPermissionCache, RedisPermissionCache>();

// ✅ NEW: Add distributed caching with Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConnectionMultiplexerFactory = async () =>
    {
        var multiplexer = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<IConnectionMultiplexer>();
        return await Task.FromResult(multiplexer);
    };
});

// USE DATABASE EXTENSION (includes DbContext + Repositories)
builder.Services.AddDatabase(builder.Configuration);

// FIXED: Use Common extension for AutoMapper
builder.Services.AddAutoMapperProfiles(typeof(UserMappingProfile));

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
    
    // ✅ NEW: Test Redis connection on startup
    try
    {
        var redis = app.Services.GetRequiredService<IConnectionMultiplexer>();
        var db = redis.GetDatabase();
        await db.StringSetAsync("startup-test", DateTime.UtcNow.ToString(), TimeSpan.FromSeconds(10));
        var test = await db.StringGetAsync("startup-test");
        Console.WriteLine($"✅ Redis connection successful: {test}");
        Log.Information("Redis connection test successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Redis connection failed: {ex.Message}");
        Log.Warning(ex, "Redis connection test failed, caching will be unavailable");
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
