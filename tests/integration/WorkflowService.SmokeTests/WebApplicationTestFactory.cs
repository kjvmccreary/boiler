using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.Data;
using WorkflowService.Persistence;
using WorkflowService;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Contracts.Services;
using Contracts.Repositories;
using Common.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder; // 🔧 FIX: Add missing using directive
using Microsoft.AspNetCore.Authentication; // 🔧 FIX: Add for authentication
using System.Text.Encodings.Web; // 🔧 FIX: Add for UrlEncoder
using Microsoft.Extensions.Options; // 🔧 FIX: Add for IOptionsMonitor

namespace WorkflowService.SmokeTests;

/// <summary>
/// Test factory that configures WorkflowService with in-memory databases for smoke testing
/// </summary>
public class WebApplicationTestFactory : WebApplicationFactory<Program>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        // 🔧 CRITICAL FIX: Override the entire host builder to prevent Program.cs from registering PostgreSQL
        return Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TestStartup>();
                webBuilder.UseEnvironment("Testing");
                webBuilder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Warning);
                });
            });
    }
}

/// <summary>
/// Test-specific startup class that completely replaces Program.cs configuration
/// </summary>
public class TestStartup
{
    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // 🔧 STEP 1: Add basic ASP.NET Core services
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        // 🔧 STEP 2: Add test-specific configuration
        var testConfig = new Dictionary<string, string?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = "Testing",
            ["JwtSettings:Issuer"] = "TestWorkflowService",
            ["JwtSettings:Audience"] = "TestApp",
            ["JwtSettings:SecretKey"] = "test-workflow-secret-key-that-is-long-enough-for-testing",
            ["JwtSettings:ExpiryMinutes"] = "60",
            ["JwtSettings:ValidateIssuer"] = "false",
            ["JwtSettings:ValidateAudience"] = "false",
            ["JwtSettings:ValidateLifetime"] = "false",
            ["TenantSettings:RequireTenantContext"] = "false",
            ["TenantSettings:DefaultTenantId"] = "1",
        };

        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(testConfig);
        var testConfiguration = configBuilder.Build();

        services.AddSingleton<IConfiguration>(testConfiguration);

        // 🔧 STEP 3: Add ONLY InMemory database contexts
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase($"TestDb_Main_{Guid.NewGuid()}");
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        services.AddDbContext<WorkflowDbContext>(options =>
        {
            options.UseInMemoryDatabase($"TestDb_Workflow_{Guid.NewGuid()}");
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        // 🔧 STEP 4: Add authentication and authorization (simplified for testing)
        services.AddAuthentication("Test")
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                "Test", options => { });

        services.AddAuthorization();

        // 🔧 STEP 5: Add required repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantManagementRepository, TenantManagementRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        // 🔧 STEP 6: Add workflow engine services
        services.AddScoped<WorkflowService.Engine.Interfaces.IWorkflowRuntime, WorkflowService.Engine.WorkflowRuntime>();
        services.AddScoped<WorkflowService.Engine.Interfaces.IConditionEvaluator, WorkflowService.Engine.JsonLogicConditionEvaluator>();

        // Add node executors
        services.AddScoped<WorkflowService.Engine.Interfaces.INodeExecutor, WorkflowService.Engine.Executors.StartEndExecutor>();
        services.AddScoped<WorkflowService.Engine.Interfaces.INodeExecutor, WorkflowService.Engine.Executors.HumanTaskExecutor>();
        services.AddScoped<WorkflowService.Engine.Interfaces.INodeExecutor, WorkflowService.Engine.Executors.AutomaticExecutor>();
        services.AddScoped<WorkflowService.Engine.Interfaces.INodeExecutor, WorkflowService.Engine.Executors.GatewayEvaluator>();
        services.AddScoped<WorkflowService.Engine.Interfaces.INodeExecutor, WorkflowService.Engine.Executors.TimerExecutor>();

        // 🔧 STEP 7: Add workflow business services
        services.AddScoped<WorkflowService.Services.Interfaces.IDefinitionService, WorkflowService.Services.DefinitionService>();
        services.AddScoped<WorkflowService.Services.Interfaces.IInstanceService, WorkflowService.Services.InstanceService>();
        services.AddScoped<WorkflowService.Services.Interfaces.ITaskService, WorkflowService.Services.TaskService>();
        services.AddScoped<WorkflowService.Services.Interfaces.IAdminService, WorkflowService.Services.AdminService>();
        services.AddScoped<WorkflowService.Services.Interfaces.IEventPublisher, WorkflowService.Services.EventPublisher>();

        // 🔧 STEP 8: Add test tenant provider
        services.AddSingleton<ITenantProvider, TestTenantProvider>();

        // 🔧 STEP 9: Add memory cache
        services.AddMemoryCache();
        services.AddSingleton<Microsoft.Extensions.Caching.Memory.IMemoryCache, Microsoft.Extensions.Caching.Memory.MemoryCache>();

        // 🔧 STEP 10: Add AutoMapper
        services.AddAutoMapper(typeof(Program));

        // 🔧 STEP 11: Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // 🔧 STEP 12: Add health checks (without database checks to avoid issues)
        services.AddHealthChecks()
            .AddCheck("workflow-service", () => 
                Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Workflow service operational"));

        // 🔧 STEP 13: Configure logging
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Warning);
            builder.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
            builder.AddFilter("Microsoft.Extensions.Hosting", LogLevel.Warning);
            builder.AddConsole();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 🔧 Configure the HTTP request pipeline for testing
        if (env.IsDevelopment() || env.IsEnvironment("Testing"))
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}

/// <summary>
/// Simple test authentication handler that always succeeds
/// </summary>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IAuthenticationSchemeProvider schemes, 
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder) 
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "TestUser"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
            new System.Security.Claims.Claim("tenant_id", "1")
        };

        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

/// <summary>
/// Simple test tenant provider that always returns tenant ID 1
/// </summary>
public class TestTenantProvider : ITenantProvider
{
    public Task<int?> GetCurrentTenantIdAsync()
    {
        return Task.FromResult<int?>(1);
    }

    public Task<string?> GetCurrentTenantIdentifierAsync()
    {
        return Task.FromResult<string?>("1");
    }

    public Task SetCurrentTenantAsync(int tenantId)
    {
        return Task.CompletedTask;
    }

    public Task SetCurrentTenantAsync(string tenantIdentifier)
    {
        return Task.CompletedTask;
    }

    public Task ClearCurrentTenantAsync()
    {
        return Task.CompletedTask;
    }

    public bool HasTenantContext => true;
}
