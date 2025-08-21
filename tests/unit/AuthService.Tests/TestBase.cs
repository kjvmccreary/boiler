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

        // ✅ FIXED for .NET 9: Use the simple approach that works in all EF Core versions
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Default(WarningBehavior.Ignore))
            .Options;

        Context = new ApplicationDbContext(
            options,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
            MockTenantProvider.Object
        );

        // Setup AutoMapper
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddAutoMapper(cfg =>
        {
            // User mappings
            cfg.CreateMap<User, DTOs.User.UserDto>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles != null ? src.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role != null ? ur.Role.Name : string.Empty).ToList() : new List<string>()));

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
        Context.TenantUsers.Add(tenantUser);
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
