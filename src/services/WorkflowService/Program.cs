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

namespace WorkflowService;

public class Program
{
    // Exposed for tests (WebApplicationFactory<Program>)
    public static WebApplication BuildApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Serilog
        builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

        // Controllers & JSON
        builder.Services
            .AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddEndpointsApiExplorer();

        // Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WorkflowService API",
                Version = "v1",
                Description = "Workflow Engine Service"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using Bearer scheme.",
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
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Common platform services
        builder.Services.AddCommonServices(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddDynamicAuthorization();
        builder.Services.AddDatabase(builder.Configuration);

        // Workflow DB
        builder.Services.AddDbContext<WorkflowDbContext>(options =>
        {
            var cs = builder.Configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(cs, npgsql => npgsql.EnableRetryOnFailure());
        });

        // Engine / runtime
        builder.Services.AddScoped<IConditionEvaluator, JsonLogicConditionEvaluator>();
        builder.Services.AddScoped<IWorkflowRuntime, WorkflowRuntime>();

        // Executors
        builder.Services.AddScoped<INodeExecutor, StartEndExecutor>();
        builder.Services.AddScoped<INodeExecutor, HumanTaskExecutor>();
        builder.Services.AddScoped<INodeExecutor, AutomaticExecutor>();
        builder.Services.AddScoped<INodeExecutor, GatewayEvaluator>();
        builder.Services.AddScoped<INodeExecutor, TimerExecutor>();
        builder.Services.AddScoped<INodeExecutor, JoinExecutor>();

        // Diagnostics & Automatic action infra
        builder.Services.AddScoped<IAutomaticActionRegistry, AutomaticActionRegistry>();
        builder.Services.AddScoped<IAutomaticActionExecutor, NoopAutomaticActionExecutor>();
        builder.Services.AddSingleton<IAutomaticDiagnosticsBuffer>(sp =>
        {
            var opts = Microsoft.Extensions.Options.Options.Create(new WorkflowDiagnosticsOptions
            {
                EnableAutomaticTrace = false,
                AutomaticBufferSize = 64
            });
            return new AutomaticDiagnosticsBuffer(opts);
        });

        // New diagnostics service
        builder.Services.AddScoped<IWorkflowDiagnosticsService, WorkflowDiagnosticsService>();

        // Background workers (timers)
        builder.Services.AddHostedService<TimerWorker>();

        // Security policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("workflow.read", p => p.RequireClaim("permission", "workflow.read", "workflow.admin"));
            options.AddPolicy("workflow.write", p => p.RequireClaim("permission", "workflow.write", "workflow.admin"));
            options.AddPolicy("workflow.admin", p => p.RequireClaim("permission", "workflow.admin"));
        });

        var app = builder.Build();

        // Middleware
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

        app.MapControllers();

        return app;
    }

    public static void Main(string[] args)
    {
        var app = BuildApp(args);
        app.Run();
    }
}
