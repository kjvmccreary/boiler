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

        // FIXED: AutoMapper configuration for .NET 9
        var services = new ServiceCollection();
        services.AddAutoMapper(cfg =>
        {
            // Configure mappings if needed
            cfg.CreateMap<User, DTOs.User.UserDto>();
            cfg.CreateMap<Tenant, DTOs.Tenant.TenantDto>();
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var user = new User
        {
            Id = 1,
            TenantId = 1,
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

        Context.Tenants.Add(tenant);
        Context.Users.Add(user);
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

    public void Dispose()
    {
        Context.Dispose();
    }
}
