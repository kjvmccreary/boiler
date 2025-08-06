// FILE: src/shared/Common/Data/ApplicationDbContextFactory.cs
using Common.Configuration;
using Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Common.Data;

/// <summary>
/// Design-time factory for ApplicationDbContext to support EF Core tools like migrations
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback connection string for migrations
            connectionString = "Host=localhost;Database=starterapp_migrations;Username=postgres;Password=postgres123;Port=5432;Include Error Detail=true";
        }

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsAssembly("Common");
        });

        // Enable logging for migrations
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();

        // Create mock dependencies for design-time
        var httpContextAccessor = new MockHttpContextAccessor();
        var tenantSettings = new TenantSettings();
        var logger = LoggerFactory.Create(builder => builder.AddConsole())
            .CreateLogger<TenantProvider>();
        var tenantProvider = new TenantProvider(httpContextAccessor, tenantSettings, logger);

        return new ApplicationDbContext(optionsBuilder.Options, httpContextAccessor, tenantProvider);
    }
}

/// <summary>
/// Mock HTTP context accessor for design-time operations
/// </summary>
public class MockHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; }
}
