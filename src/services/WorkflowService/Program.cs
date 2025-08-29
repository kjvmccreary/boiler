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

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for .NET 9
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

// Add services to the container
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WorkflowService API",
        Version = "v1",
        Description = "Workflow Engine Service with JSON-based definitions and execution"
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
builder.Services.AddDynamicAuthorization();

// Add the main ApplicationDbContext (required by common services)
builder.Services.AddDatabase(builder.Configuration);

// Add WorkflowService database context
builder.Services.AddDbContext<WorkflowDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
    });
});

// ✅ STEP 1: Add Workflow Engine Services (COMPLETED)
builder.Services.AddScoped<IWorkflowRuntime, WorkflowRuntime>();
builder.Services.AddScoped<IConditionEvaluator, JsonLogicConditionEvaluator>();

// Add Node Executors
builder.Services.AddScoped<INodeExecutor, StartEndExecutor>();
builder.Services.AddScoped<INodeExecutor, HumanTaskExecutor>();
builder.Services.AddScoped<INodeExecutor, AutomaticExecutor>();
builder.Services.AddScoped<INodeExecutor, GatewayEvaluator>();
builder.Services.AddScoped<INodeExecutor, TimerExecutor>();

// Add Background Services
builder.Services.AddHostedService<TimerWorker>();

// 🚀 STEP 2: Add Workflow Business Services (NEW)
builder.Services.AddScoped<IDefinitionService, DefinitionService>();
builder.Services.AddScoped<IInstanceService, InstanceService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddScoped<IRoleWorkflowUsageService, RoleWorkflowUsageService>();
builder.Services.AddScoped<IWorkflowExecutionService, WorkflowExecutionService>();

// TODO: Add Workflow Security when created (STEP 3)
builder.Services.AddWorkflowPolicies();

// Add AutoMapper (basic setup for now)
builder.Services.AddAutoMapper(typeof(Program));

// Add FluentValidation
builder.Services.AddFluentValidation();

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

// Add Health Checks with both database contexts
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("main-database")
    .AddDbContextCheck<WorkflowDbContext>("workflow-database")
    .AddCheck("workflow-service", () => 
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Workflow service operational"));

var app = builder.Build();

// Configure the HTTP request pipeline
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

// Add validation middleware
app.UseMiddleware<ValidationMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();

// ✅ FIX: Use proper tenant resolution middleware (same as UserService)
app.UseTenantResolution();

app.UseAuthorization();

app.MapControllers();

// Add health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Add startup logging
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
        .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;
    
    foreach (var address in addresses ?? Enumerable.Empty<string>())
    {
        Console.WriteLine($"Now listening on: {address}");
        Log.Information("Now listening on: {Address}", address);
    }
    
    Console.WriteLine("🌊 WorkflowService started with workflow engine and service layer enabled");
    Log.Information("WorkflowService started - workflow engine, service layer, and background workers operational");
});

try
{
    Log.Information("Starting WorkflowService with Engine and Service Layer");
    Console.WriteLine("=== WorkflowService Starting with Engine and Service Layer ===");
    
    // Run database migrations
    using (var scope = app.Services.CreateScope())
    {
        // Migrate main ApplicationDbContext first
        var mainContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await mainContext.Database.MigrateAsync();
        Console.WriteLine("✅ Main database migrations applied");
        
        // Migrate WorkflowDbContext
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

// Make Program class accessible for testing
public partial class Program { }
