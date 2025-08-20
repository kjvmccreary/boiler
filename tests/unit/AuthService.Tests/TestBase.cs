using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using Common.Data;
using DTOs.Entities;
using Contracts.Services;

namespace AuthService.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly ApplicationDbContext Context;
    protected readonly IMapper Mapper;
    protected readonly Mock<ITenantProvider> MockTenantProvider;

    protected Mock<ILogger<T>> MockLogger<T>() => new();

    protected TestBase()
    {
        // Setup mocks
        MockTenantProvider = new Mock<ITenantProvider>();
        MockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        // Setup In-Memory Database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(
            options,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            MockTenantProvider.Object
        );

        // FIXED: Add logging services before AutoMapper configuration
        var services = new ServiceCollection();

        // Add logging services that AutoMapper needs
        services.AddLogging(builder => builder.AddConsole());

        // FIXED: Add AutoMapper configuration with proper mappings
        services.AddAutoMapper(cfg =>
        {
            // User mappings
            cfg.CreateMap<User, DTOs.User.UserDto>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // 🔧 FIX: TenantId set from context
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name).ToList()));

            // Tenant mappings  
            cfg.CreateMap<Tenant, DTOs.Tenant.TenantDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src =>
                    ParseJsonSettings(src.Settings)));
        });

        var serviceProvider = services.BuildServiceProvider();
        Mapper = serviceProvider.GetRequiredService<IMapper>();

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var tenant = new Tenant
        {
            Id = 1,
            Name = "Test Tenant",
            Domain = "test.com",
            IsActive = true,
            SubscriptionPlan = "Basic",
            Settings = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var user = new User
        {
            Id = 1,
            // TenantId = 1, // 🔧 REMOVE: User no longer has TenantId
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            IsActive = true,
            EmailConfirmed = true,
            FailedLoginAttempts = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // 🔧 ADD: Create TenantUser relationship
        var tenantUser = new TenantUser
        {
            TenantId = 1,
            UserId = 1,
            Role = "User",
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Tenants.Add(tenant);
        Context.Users.Add(user);
        Context.TenantUsers.Add(tenantUser); // 🔧 ADD: Create tenant-user relationship
        Context.SaveChanges();
    }

    protected User GetTestUser()
    {
        return Context.Users.First();
    }

    protected Tenant GetTestTenant()
    {
        return Context.Tenants.First();
    }

    // ADDED: Helper method for parsing JSON settings (same as in MappingProfile)
    private static Dictionary<string, object> ParseJsonSettings(string settingsJson)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(settingsJson) || settingsJson == "{}")
                return new Dictionary<string, object>();

            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson)
                   ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
