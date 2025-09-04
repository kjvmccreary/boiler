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
using WorkflowService.Hubs;
using WorkflowService.Services.Validation;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.AutomaticActions.Executors;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Validation; // ADD
using WorkflowService.Engine.Gateways; // ADD

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

// Controllers & JSON
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger + JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WorkflowService API",
        Version = "v1",
        Description = "Workflow Engine Service with JSON-based definitions and execution"
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

// Common platform services
builder.Services.AddCommonServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDynamicAuthorization();
builder.Services.AddDatabase(builder.Configuration); // Main shared ApplicationDbContext

// Workflow DB
builder.Services.AddDbContext<WorkflowDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure());
});

// Engine + Condition evaluation
builder.Services.AddScoped<IConditionEvaluator, JsonLogicConditionEvaluator>();
builder.Services.AddScoped<IWorkflowRuntime, WorkflowRuntime>();

// Executors
builder.Services.AddScoped<INodeExecutor, StartEndExecutor>();
builder.Services.AddScoped<INodeExecutor, HumanTaskExecutor>();
builder.Services.AddScoped<INodeExecutor, AutomaticExecutor>(); // REFACTORED to use registry
builder.Services.AddScoped<INodeExecutor, GatewayEvaluator>();
builder.Services.AddScoped<INodeExecutor, TimerExecutor>();

// Automatic Action Infrastructure
builder.Services.AddScoped<IAutomaticActionExecutor, NoopAutomaticActionExecutor>();
builder.Services.AddScoped<IAutomaticActionExecutor, WebhookAutomaticActionExecutor>();
builder.Services.AddSingleton<IAutomaticActionRegistry, AutomaticActionRegistry>();
builder.Services.AddHttpClient("workflow-webhook")
    .ConfigureHttpClient(c =>
    {
        c.Timeout = TimeSpan.FromSeconds(15);
        c.DefaultRequestHeaders.UserAgent.ParseAdd("WorkflowService/automatic-action");
    });

// Background workers
builder.Services.AddHostedService<TimerWorker>();
builder.Services.AddHostedService<OutboxWorker>();

// HTTP Context / User
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

// Outbox dispatcher
builder.Services.AddScoped<IOutboxDispatcher, LoggingOutboxDispatcher>();

// Business services
builder.Services.AddScoped<IDefinitionService, DefinitionService>();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IRoleWorkflowUsageService, RoleWorkflowUsageService>();
builder.Services.AddScoped<IWorkflowExecutionService, WorkflowExecutionService>(); // Legacy

// NEW: Graph validation service
builder.Services.AddScoped<IGraphValidationService, GraphValidationService>();
builder.Services.AddScoped<IWorkflowGraphValidator, WorkflowGraphValidator>();

// Authorization policies
builder.Services.AddWorkflowPolicies();

// AutoMapper / Validation
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddFluentValidation();
builder.Services.AddWorkflowPublishValidation(); // ADD

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("main-database")
    .AddDbContextCheck<WorkflowDbContext>("workflow-database")
    .AddCheck("workflow-service", () =>
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Workflow service operational"));

// SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<ITaskNotificationDispatcher, TaskNotificationDispatcher>();

// UoW
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Diagnostics options & buffers
builder.Services.Configure<WorkflowDiagnosticsOptions>(builder.Configuration.GetSection("Workflow:Diagnostics"));
builder.Services.AddSingleton<IAutomaticDiagnosticsBuffer, AutomaticDiagnosticsBuffer>();

// Gateway strategies
builder.Services.AddSingleton<IGatewayStrategy, ExclusiveGatewayStrategy>();
builder.Services.AddSingleton<IGatewayStrategy, ParallelGatewayStrategy>();
builder.Services.AddSingleton<IGatewayStrategyRegistry, GatewayStrategyRegistry>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkflowService API V1");
        c.RoutePrefix = "swagger";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ValidationMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseTenantResolution();
app.UseAuthorization();

app.MapControllers();
app.MapHub<TaskNotificationsHub>("/api/workflow/hubs/tasks");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Startup log
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
        .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;

    foreach (var address in addresses ?? Enumerable.Empty<string>())
    {
        Console.WriteLine($"Now listening on: {address}");
        Serilog.Log.Information("Now listening on: {Address}", address);
    }

    Console.WriteLine("🌊 WorkflowService started (runtime, timer automation, graph validation, outbox idempotency).");
    Serilog.Log.Information("WorkflowService startup complete (TimerWorker + GraphValidation + Outbox Idempotency).");
});

// Migrations + Run
try
{
    Serilog.Log.Information("Starting WorkflowService");
    Console.WriteLine("=== WorkflowService Starting ===");

    using (var scope = app.Services.CreateScope())
    {
        var mainContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await mainContext.Database.MigrateAsync();
        Console.WriteLine("✅ Main database migrations applied");

        var workflowContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        await workflowContext.Database.MigrateAsync();
        Console.WriteLine("✅ Workflow database migrations applied");
    }

    app.Run();
}
catch (Exception ex)
{
    Serilog.Log.Fatal(ex, "WorkflowService terminated unexpectedly");
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
}
finally
{
    Serilog.Log.CloseAndFlush();
}

public partial class Program { }
