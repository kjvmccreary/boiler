// FILE: src/shared/Common/Extensions/DatabaseExtensions.cs
using Common.Data;
using Common.Repositories;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("Common"); // Specify where migrations are stored
                npgsqlOptions.CommandTimeout(30);
            });

            // Enable in development only
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
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
        // Only register repositories that actually exist and work
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Add other specific repositories as they are implemented:
        // services.AddScoped<ITenantManagementRepository, TenantManagementRepository>();
        // services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }

    public static IServiceCollection AddDatabaseHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found"),
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
        var defaultTenant = new Tenant
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
