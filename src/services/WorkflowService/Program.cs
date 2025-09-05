using Common.Data;
using Common.Extensions;
using Common.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using WorkflowService.Persistence;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Engine.Executors;
using WorkflowService.Background;
using WorkflowService.Services.Interfaces;
using WorkflowService.Services;
using WorkflowService.Security;
using System.Text.Json.Serialization;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.AutomaticActions.Executors;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Validation;
using WorkflowService.Engine.Gateways;
using WorkflowService.Middleware;
using Common.Hashing;
using WorkflowService.Engine.Pruning;
using WorkflowService.Engine.FeatureFlags;
using WorkflowService.Engine.Timeouts;
using WorkflowService.Outbox;
using WorkflowService.Hubs;
using OutboxDispatcherInterface = WorkflowService.Outbox.IOutboxDispatcher;
using OutboxDispatcherImpl = WorkflowService.Outbox.OutboxDispatcher;
using Microsoft.AspNetCore.Authorization;
using Common.Configuration;
using Contracts.Repositories;                // ADDED
using Common.Repositories;                  // ADDED
using Contracts.Services;
using Common.Services;

namespace WorkflowService;

public class Program
{
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

        builder.Services
            .AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkflowService API", Version = "v1", Description = "Workflow Engine Service" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using Bearer scheme.",
                Name = "Authorization", In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey, Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
            });
        });

        // Common / tenancy basics
        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection("Tenancy"));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TenantSettings>>().Value);

        builder.Services.AddScoped<ITenantProvider, TenantProvider>();
        builder.Services.AddScoped<IAuditService, AuditService>();

        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddDynamicAuthorization();

        builder.Services.AddDeterministicHashing(builder.Configuration);

        var cs = builder.Configuration.GetConnectionString("DefaultConnection");

        // Workflow DB
        builder.Services.AddDbContext<WorkflowDbContext>(o =>
            o.UseNpgsql(cs, npgsql => npgsql.EnableRetryOnFailure()));

        // Shared application (auth / roles) DB
        var appCs = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(o =>
            o.UseNpgsql(appCs, npgsql => npgsql.EnableRetryOnFailure()));

        // --- RBAC Repository + Service Wiring (fixes docker DI failure) ---
        // These repositories live in Common.Repositories and implement Contracts.Repositories.*
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();                 // ADDED
        builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();     // ADDED
        builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();         // ADDED
        builder.Services.AddScoped<IRoleService, RoleService>();                       // ADDED
        builder.Services.AddScoped<IPermissionService, PermissionService>();           // If required elsewhere

        // Engine services
        builder.Services.AddScoped<IWorkflowRuntime, WorkflowRuntime>();
        builder.Services.AddScoped<IConditionEvaluator, JsonLogicConditionEvaluator>();

        // Executors
        builder.Services.AddScoped<INodeExecutor, StartEndExecutor>();
        builder.Services.AddScoped<INodeExecutor, HumanTaskExecutor>();
        builder.Services.AddScoped<INodeExecutor, AutomaticExecutor>();
        builder.Services.AddScoped<INodeExecutor, GatewayEvaluator>();
        builder.Services.AddScoped<INodeExecutor, TimerExecutor>();
        builder.Services.AddScoped<INodeExecutor, JoinExecutor>();

        // Automatic actions + diagnostics
        builder.Services.AddScoped<IAutomaticActionRegistry, AutomaticActionRegistry>();
        builder.Services.AddScoped<IAutomaticActionExecutor, NoopAutomaticActionExecutor>();
        builder.Services.AddSingleton<IAutomaticDiagnosticsBuffer>(_ =>
            new AutomaticDiagnosticsBuffer(
                Microsoft.Extensions.Options.Options.Create(new WorkflowDiagnosticsOptions
                {
                    EnableAutomaticTrace = false,
                    AutomaticBufferSize = 64
                })));
        builder.Services.AddScoped<IWorkflowDiagnosticsService, WorkflowDiagnosticsService>();

        // Gateway strategies
        builder.Services.AddSingleton<IWorkflowContextPruner, WorkflowContextPruner>();
        builder.Services.AddScoped<IGatewayPruningEventEmitter, GatewayPruningEventEmitter>();
        builder.Services.AddScoped<IExperimentAssignmentEmitter, ExperimentAssignmentEmitter>();
        builder.Services.AddScoped<IGatewayStrategy, AbTestGatewayStrategy>();
        builder.Services.AddScoped<IGatewayStrategy, FeatureFlagGatewayStrategy>();
        builder.Services.AddScoped<IGatewayStrategyRegistry, GatewayStrategyRegistry>();
        builder.Services.AddScoped<IFeatureFlagProvider, NoopFeatureFlagProvider>();
        builder.Services.AddScoped<IFeatureFlagFallbackEmitter, FeatureFlagFallbackEmitter>();

        // Outbox
        builder.Services.AddScoped<IOutboxWriter, OutboxWriter>();
        builder.Services.AddScoped<IEventPublisher, EventPublisher>();
        builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Workflow:Outbox"));
        builder.Services.AddHealthChecks().AddCheck<OutboxHealthCheck>("outbox");
        builder.Services.AddSingleton<IOutboxMetricsProvider, OutboxMetricsProvider>();
        builder.Services.Configure<OutboxBackfillOptions>(builder.Configuration.GetSection("Workflow:Outbox:Backfill"));
        builder.Services.AddHostedService<OutboxIdempotencyBackfillWorker>();
        builder.Services.AddScoped<IOutboxTransport, LoggingOutboxTransport>();
        builder.Services.AddScoped<OutboxDispatcherInterface, OutboxDispatcherImpl>();
        builder.Services.AddHostedService<OutboxBackgroundWorker>();

        // Timers / timeouts
        builder.Services.AddHostedService<TimerWorker>();
        builder.Services.Configure<JoinTimeoutOptions>(builder.Configuration.GetSection("Workflow:JoinTimeouts"));
        builder.Services.AddHostedService<JoinTimeoutWorker>();

        // Task notifications
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<ITaskNotificationDispatcher, TaskNotificationDispatcher>();

        // Authorization policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("workflow.read", p => p.RequireClaim("permission", "workflow.read", "workflow.admin"));
            options.AddPolicy("workflow.write", p => p.RequireClaim("permission", "workflow.write", "workflow.admin"));
            options.AddPolicy("workflow.admin", p => p.RequireClaim("permission", "workflow.admin"));
        });

        var app = builder.Build();

        app.UseSerilogRequestLogging();
        app.UseWorkflowExceptionHandling();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health");
        app.MapHub<TaskNotificationsHub>("/hubs/tasks");
        app.MapHub<TaskNotificationsHub>("/api/workflow/hubs/tasks");

        app.MapControllers();
        return app;
    }

    public static void Main(string[] args)
    {
        var app = BuildApp(args);
        app.Run();
    }
}
