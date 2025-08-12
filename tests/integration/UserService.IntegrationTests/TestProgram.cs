using Common.Data;
using Common.Extensions;
using Common.Repositories;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserService.Mappings;
using UserService.Services;

namespace UserService.IntegrationTests;

public class TestProgram
{
    public static WebApplication CreateTestApp()
    {
        var builder = WebApplication.CreateBuilder(new string[0]);
        
        // Configure for testing
        builder.Environment.EnvironmentName = "Testing";
        
        // ✅ CONFIGURE INMEMORY DATABASE FIRST - BEFORE ANY OTHER SERVICES
        var databaseName = $"TestDb_{Guid.NewGuid():N}";
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName);
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            
            // Ignore warnings for InMemory database
            options.ConfigureWarnings(warnings => 
            {
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning);
            });
        });

        // ✅ ADD ALL OTHER SERVICES FROM THE MAIN PROGRAM (minus the database)
        ConfigureServices(builder.Services, builder.Configuration);
        
        var app = builder.Build();
        
        // Configure middleware pipeline (simplified for tests)
        ConfigureMiddleware(app);
        
        return app;
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Add controllers
        services.AddControllers();
        
        // Add common services (JWT, configuration, etc.) - but NOT database
        services.AddCommonServices(configuration);
        services.AddJwtAuthentication(configuration);
        
        // Add AutoMapper
        services.AddAutoMapperProfiles(typeof(UserMappingProfile));
        
        // Add service implementations
        services.AddScoped<Contracts.User.IUserService, UserServiceImplementation>();
        services.AddScoped<Contracts.User.IUserProfileService, UserProfileService>();
        
        // Add FluentValidation
        services.AddFluentValidation();
        
        // Add repositories and services manually (since we skipped AddDatabase)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantManagementRepository, TenantManagementRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        
        // Add authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin,SuperAdmin"));

            options.AddPolicy("OwnerOrAdmin", policy =>
                policy.RequireAssertion(context =>
                {
                    var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var isAdmin = context.User.IsInRole("Admin") || context.User.IsInRole("SuperAdmin");
                    return isAdmin || (userId != null);
                }));
        });
        
        // Add HTTP context accessor
        services.AddHttpContextAccessor();
        
        // Add logging
        services.AddLogging();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        // Simplified middleware pipeline for tests
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
