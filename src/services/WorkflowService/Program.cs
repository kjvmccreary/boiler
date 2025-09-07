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
using Contracts.Repositories;
using Common.Repositories;
using Contracts.Services;
using Common.Services;
using WorkflowService.Services.Validation;

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
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WorkflowService API", Version = "1.0" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header. Example: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
            });
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection("Tenancy"));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TenantSettings>>().Value);

        builder.Services.AddScoped<ITenantProvider, TenantProvider>();
        builder.Services.AddScoped<IAuditService, AuditService>();

        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddDynamicAuthorization();
        builder.Services.AddDeterministicHashing(builder.Configuration);

        var cs = builder.Configuration.GetConnectionString("DefaultConnection");
        var isTesting = builder.Environment.IsEnvironment("Testing");
        var disableBackground = isTesting || Environment.GetEnvironmentVariable("WF_DISABLE_BG") == "1";

        // IMPORTANT: Use only ONE EF provider per service provider.
        if (isTesting)
        {
            builder.Services.AddDbContext<WorkflowDbContext>(o =>
                o.UseInMemoryDatabase("wf-int-" + Guid.NewGuid().ToString("N"))
                 .EnableSensitiveDataLogging());

            builder.Services.AddDbContext<ApplicationDbContext>(o =>
                o.UseInMemoryDatabase("app-int-" + Guid.NewGuid().ToString("N"))
                 .EnableSensitiveDataLogging());
        }
        else
        {
            builder.Services.AddDbContext<WorkflowDbContext>(o =>
                o.UseNpgsql(cs, npgsql => npgsql.EnableRetryOnFailure()));

            builder.Services.AddDbContext<ApplicationDbContext>(o =>
                o.UseNpgsql(cs, npgsql => npgsql.EnableRetryOnFailure()));
        }

        // RBAC
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
        builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IPermissionService, PermissionService>();

        // Core workflow app services
        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddScoped<IDefinitionService, DefinitionService>();
        builder.Services.AddScoped<IInstanceService, InstanceService>();
        builder.Services.AddScoped<IWorkflowGraphValidator, WorkflowGraphValidator>();
        builder.Services.AddScoped<IGraphValidationService, GraphValidationService>();
        builder.Services.AddScoped<IWorkflowPublishValidator, WorkflowPublishValidator>();

        // Engine runtime
        builder.Services.AddScoped<IWorkflowRuntime, WorkflowRuntime>();
        builder.Services.AddScoped<IConditionEvaluator, JsonLogicConditionEvaluator>();

        // Executors
        builder.Services.AddScoped<INodeExecutor, StartEndExecutor>();
        builder.Services.AddScoped<INodeExecutor, HumanTaskExecutor>();
        builder.Services.AddScoped<INodeExecutor, AutomaticExecutor>();
        builder.Services.AddScoped<INodeExecutor, GatewayEvaluator>();
        builder.Services.AddScoped<INodeExecutor, TimerExecutor>();
        builder.Services.AddScoped<INodeExecutor, JoinExecutor>();

        // Automatic actions / diagnostics
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

        // Outbox (core services always; workers only if enabled)
        builder.Services.AddScoped<IOutboxWriter, OutboxWriter>();
        builder.Services.AddScoped<IEventPublisher, EventPublisher>();
        builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Workflow:Outbox"));
        builder.Services.AddHealthChecks().AddCheck<OutboxHealthCheck>("outbox");
        builder.Services.AddSingleton<IOutboxMetricsProvider, OutboxMetricsProvider>();
        builder.Services.Configure<OutboxBackfillOptions>(builder.Configuration.GetSection("Workflow:Outbox:Backfill"));
        builder.Services.AddScoped<IOutboxTransport, LoggingOutboxTransport>();
        builder.Services.AddScoped<OutboxDispatcherInterface, OutboxDispatcherImpl>();

        if (!disableBackground)
        {
            builder.Services.AddHostedService<OutboxIdempotencyBackfillWorker>();
            builder.Services.AddHostedService<OutboxBackgroundWorker>();
            builder.Services.AddHostedService<TimerWorker>();
            builder.Services.Configure<JoinTimeoutOptions>(builder.Configuration.GetSection("Workflow:JoinTimeouts"));
            builder.Services.AddHostedService<JoinTimeoutWorker>();
        }

        builder.Services.AddSignalR();

        // Notifications
        builder.Services.AddSingleton<ITaskNotificationDispatcher, TaskNotificationDispatcher>();
        builder.Services.AddScoped<IWorkflowNotificationDispatcher, WorkflowNotificationDispatcher>();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("workflow.read",  p => p.RequireClaim("permission", "workflow.read",  "workflow.admin"));
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
        app.MapHub<WorkflowNotificationsHub>("/hubs/instances");
        app.MapHub<WorkflowNotificationsHub>("/api/workflow/hubs/instances");

        app.MapControllers();
        return app;
    }

    public static void Main(string[] args)
    {
        var app = BuildApp(args);
        app.Run();
    }
}
