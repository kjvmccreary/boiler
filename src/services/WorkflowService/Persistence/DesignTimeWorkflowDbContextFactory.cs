using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Http;
using WorkflowService.Persistence;
using Contracts.Services;

namespace WorkflowService.Persistence;

// Design-time factory so `dotnet ef` can create WorkflowDbContext without the full Host / DI graph.
public class DesignTimeWorkflowDbContextFactory : IDesignTimeDbContextFactory<WorkflowDbContext>
{
    public WorkflowDbContext CreateDbContext(string[] args)
    {
        // Build a configuration stack similar to runtime
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                          Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
                          "Development";

        var basePath = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<DesignTimeWorkflowDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Order of precedence:
        // 1. CLI override (--connection="...")
        // 2. Environment variables (ConnectionStrings__WorkflowService, WORKFLOW_DB)
        // 3. appsettings connection string
        // 4. Fallback local default
        string? cliConnection = ParseArg(args, "--connection=");
        var cs =
            cliConnection ??
            Environment.GetEnvironmentVariable("ConnectionStrings__WorkflowService") ??
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ?? // added
            Environment.GetEnvironmentVariable("WORKFLOW_DB") ??
            config.GetConnectionString("WorkflowService") ??
            config.GetConnectionString("DefaultConnection") ?? // added
            "Host=localhost;Port=5432;Database=boiler_dev;Username=boiler_app;Password=dev_password123"; // adjust fallback to real creds

        LogConnectionUsed(cs, environment);

        var optionsBuilder = new DbContextOptionsBuilder<WorkflowDbContext>();
        optionsBuilder.UseNpgsql(cs, npg =>
        {
            npg.MigrationsAssembly(typeof(WorkflowDbContext).Assembly.FullName);
        });

        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();

        var httpAccessor = new HttpContextAccessor();
        var tenantProvider = new DesignTimeTenantProvider();
        var logger = NullLogger<WorkflowDbContext>.Instance;

        return new WorkflowDbContext(optionsBuilder.Options, httpAccessor, tenantProvider, logger);
    }

    private static string? ParseArg(string[] args, string keyPrefix)
    {
        if (args == null) return null;
        foreach (var a in args)
        {
            if (a.StartsWith(keyPrefix, StringComparison.OrdinalIgnoreCase))
                return a[keyPrefix.Length..].Trim('"');
        }
        return null;
    }

    private static void LogConnectionUsed(string cs, string env)
    {
        try
        {
            // Mask password for safety
            var parts = cs.Split(';', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
                {
                    parts[i] = "Password=*****";
                }
            }
            Console.WriteLine($"[DesignTimeDbContextFactory] Environment={env} UsingConnection=\"{string.Join(';', parts)}\"");
        }
        catch { /* swallow */ }
    }

    // Minimal tenant provider used only so WorkflowDbContext can be constructed at design time.
    private sealed class DesignTimeTenantProvider : ITenantProvider
    {
        private int? _tenantId = 1;

        public Task<int?> GetCurrentTenantIdAsync() => Task.FromResult(_tenantId);
        public Task<string?> GetCurrentTenantIdentifierAsync() =>
            Task.FromResult<string?>(_tenantId?.ToString()); // simple numeric identifier

        public Task SetCurrentTenantAsync(int tenantId)
        {
            _tenantId = tenantId;
            return Task.CompletedTask;
        }

        public Task SetCurrentTenantAsync(string tenantIdentifier)
        {
            // Accept any string, map to fixed design-time tenant 1
            _tenantId = 1;
            return Task.CompletedTask;
        }

        public Task ClearCurrentTenantAsync()
        {
            _tenantId = null;
            return Task.CompletedTask;
        }

        public bool HasTenantContext => _tenantId.HasValue;
    }
}
