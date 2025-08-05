// FILE: src/shared/Common/Extensions/DatabaseExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.Configuration;
using Common.Services;
using Common.Repositories;
using Contracts.Services;
using Contracts.Repositories;

namespace Common.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration
        var databaseSettings = configuration.GetRequiredSection<DatabaseSettings>(DatabaseSettings.SectionName);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(databaseSettings.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("Common"); // Specify where migrations are stored
                npgsqlOptions.CommandTimeout(databaseSettings.CommandTimeout);
            });

            if (databaseSettings.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }

            if (databaseSettings.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }
        });

        // Tenant Provider
        services.AddScoped<ITenantProvider, TenantProvider>();

        // Repository registrations
        services.AddRepositories();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Base repositories
        services.AddScoped(typeof(IRepository<>), typeof(TenantRepository<>));

        // Specific repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantScopedRepository>();
        services.AddScoped<ITenantManagementRepository, TenantManagementRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }

    public static IServiceCollection AddDatabaseHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetRequiredSection<DatabaseSettings>(DatabaseSettings.SectionName);

        services.AddHealthChecks()
            .AddNpgSql(
                databaseSettings.ConnectionString,
                name: "database",
                tags: new[] { "db", "postgresql" });

        return services;
    }

    // Extension method to ensure database is created and migrated
    public static async Task<IServiceProvider> EnsureDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Apply any pending migrations
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }

        return serviceProvider;
    }

    // Extension method to seed initial data
    public static async Task<IServiceProvider> SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedInitialDataAsync(context);

        return serviceProvider;
    }

    private static async Task SeedInitialDataAsync(ApplicationDbContext context)
    {
        // Check if we already have data
        if (await context.Tenants.AnyAsync())
            return;

        // Create default tenant
        var defaultTenant = new Common.Entities.Tenant
        {
            Name = "Default Tenant",
            Domain = "localhost",
            SubscriptionPlan = "Basic",
            IsActive = true,
            Settings = "{\"theme\":\"default\",\"features\":[\"users\",\"auth\"]}"
        };

        context.Tenants.Add(defaultTenant);
        await context.SaveChangesAsync();

        // You can add more seed data here as needed
    }
}
