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
builder.Services.AddScoped<INodeExecutor, AutomaticExecutor>();
builder.Services.AddScoped<INodeExecutor, GatewayEvaluator>();
builder.Services.AddScoped<INodeExecutor, TimerExecutor>();

// Background workers
builder.Services.AddHostedService<TimerWorker>();
builder.Services.AddHostedService<OutboxWorker>();

// HTTP Context / User
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

// Outbox dispatcher stub (logging-only MVP)
builder.Services.AddScoped<IOutboxDispatcher, LoggingOutboxDispatcher>();

// Business services
builder.Services.AddScoped<IDefinitionService, DefinitionService>();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IRoleWorkflowUsageService, RoleWorkflowUsageService>();
builder.Services.AddScoped<IWorkflowExecutionService, WorkflowExecutionService>(); // Legacy traversal (kept until fully removed)

// NEW: Graph validation service (strict publish-time validation)
builder.Services.AddScoped<IGraphValidationService, GraphValidationService>();

// Authorization policies
builder.Services.AddWorkflowPolicies();

// AutoMapper / Validation
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddFluentValidation();

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

// UoW (if leveraged by controllers/services)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
        Log.Information("Now listening on: {Address}", address);
    }

    Console.WriteLine("🌊 WorkflowService started (runtime, timer automation, graph validation, outbox idempotency).");
    Log.Information("WorkflowService startup complete (TimerWorker + GraphValidation + Outbox Idempotency).");
});

// Migrations + Run
try
{
    Log.Information("Starting WorkflowService");
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
    Log.Fatal(ex, "WorkflowService terminated unexpectedly");
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }
