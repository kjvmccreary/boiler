using Common.Data;
using Common.Extensions;
using Common.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using WorkflowService.Persistence; // ✅ UNCOMMENT: We have WorkflowDbContext
// REMOVE: These namespaces don't exist yet - will add them later
// using WorkflowService.Services;
// using WorkflowService.Engine;
// using WorkflowService.Background;
// using WorkflowService.Security;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for .NET 9
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

// Add services to the container
builder.Services.AddControllers();
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

// ✅ FIX: Add the main ApplicationDbContext (required by common services)
builder.Services.AddDatabase(builder.Configuration);

// ✅ UNCOMMENT: Add WorkflowService database context (NOW READY!)
builder.Services.AddDbContext<WorkflowDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure();
    });
});

// TODO: Add Workflow Services when created
// builder.Services.AddScoped<IDefinitionService, DefinitionService>();
// builder.Services.AddScoped<IInstanceService, InstanceService>();
// builder.Services.AddScoped<ITaskService, TaskService>();
// builder.Services.AddScoped<IAdminService, AdminService>();
// builder.Services.AddScoped<IEventPublisher, EventPublisher>();

// TODO: Add Workflow Engine when created
// builder.Services.AddScoped<IWorkflowRuntime, WorkflowRuntime>();
// builder.Services.AddScoped<IConditionEvaluator, JsonLogicConditionEvaluator>();
// builder.Services.AddScoped<IAutomaticExecutor, AutomaticExecutor>();
// builder.Services.AddScoped<IHumanTaskExecutor, HumanTaskExecutor>();
// builder.Services.AddScoped<ITimerExecutor, TimerExecutor>();
// builder.Services.AddScoped<IGatewayEvaluator, GatewayEvaluator>();

// TODO: Add Background Services when created
// builder.Services.AddHostedService<TimerWorker>();

// TODO: Add Workflow Security when created
// builder.Services.AddWorkflowPolicies();

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

// ✅ ENHANCED: Add Health Checks with both database contexts
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
    
    Console.WriteLine("🌊 WorkflowService started with dual database contexts");
    Log.Information("WorkflowService started - main and workflow database contexts operational");
});

try
{
    Log.Information("Starting WorkflowService");
    Console.WriteLine("=== WorkflowService Starting ===");
    
    // ✅ UNCOMMENT: Run database migrations (NOW READY!)
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
