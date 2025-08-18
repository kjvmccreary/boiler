using AutoMapper;
using Common.Data;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using DTOs.Tenant;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UserService.Services;
using Xunit;

namespace UserService.UnitTests.Services;

public class TenantServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<TenantService>> _loggerMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IRoleTemplateService> _roleTemplateServiceMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ITenantProvider> _tenantProviderMock;
    private readonly ApplicationDbContext _context;
    private readonly TenantService _tenantService;

    public TenantServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<TenantService>>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _roleTemplateServiceMock = new Mock<IRoleTemplateService>();
        _auditServiceMock = new Mock<IAuditService>();
        _tenantProviderMock = new Mock<ITenantProvider>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new ApplicationDbContext(options, null!, _tenantProviderMock.Object);
        
        _tenantService = new TenantService(
            _context,
            _mapperMock.Object,
            _loggerMock.Object,
            _passwordServiceMock.Object,
            _roleTemplateServiceMock.Object,
            _auditServiceMock.Object,
            _tenantProviderMock.Object);
    }

    [Fact]
    public async Task CreateTenantAsync_WithValidData_ShouldCreateTenant()
    {
        // Arrange
        var createDto = new CreateTenantDto
        {
            Name = "Test Tenant",
            Domain = "test.com",
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin@test.com",
                FirstName = "Admin",
                LastName = "User",
                Password = "Password123!"
            }
        };

        var expectedTenant = new Tenant
        {
            Id = 1,
            Name = "Test Tenant",
            Domain = "test.com",
            SubscriptionPlan = "Basic",
            IsActive = true
        };

        var expectedTenantDto = new TenantDto
        {
            Id = 1,
            Name = "Test Tenant",
            Domain = "test.com",
            SubscriptionPlan = "Basic",
            IsActive = true
        };

        _passwordServiceMock.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");
        
        _mapperMock.Setup(x => x.Map<TenantDto>(It.IsAny<Tenant>()))
            .Returns(expectedTenantDto);

        _roleTemplateServiceMock.Setup(x => x.CreateDefaultRolesForTenantAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tenantService.CreateTenantAsync(createDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Tenant");

        var createdTenant = await _context.Tenants.FirstOrDefaultAsync();
        createdTenant.Should().NotBeNull();
        createdTenant!.Name.Should().Be("Test Tenant");

        var createdUser = await _context.Users.FirstOrDefaultAsync();
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be("admin@test.com");

        _roleTemplateServiceMock.Verify(x => x.CreateDefaultRolesForTenantAsync(It.IsAny<int>()), 
            Times.Once);
    }

    [Fact]
    public async Task CreateTenantAsync_WithDuplicateDomain_ShouldReturnError()
    {
        // Arrange
        var existingTenant = new Tenant
        {
            Name = "Existing Tenant",
            Domain = "duplicate.com",
            SubscriptionPlan = "Basic",
            IsActive = true
        };
        
        await _context.Tenants.AddAsync(existingTenant);
        await _context.SaveChangesAsync();

        var createDto = new CreateTenantDto
        {
            Name = "New Tenant",
            Domain = "duplicate.com", // Same domain
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin@new.com",
                FirstName = "Admin",
                LastName = "User",
                Password = "Password123!"
            }
        };

        // Act
        var result = await _tenantService.CreateTenantAsync(createDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already in use");
    }

    [Fact]
    public async Task GetTenantAsync_WithValidId_ShouldReturnTenant()
    {
        // Arrange
        var tenant = new Tenant
        {
            Id = 1,
            Name = "Test Tenant",
            Domain = "test.com",
            SubscriptionPlan = "Basic",
            IsActive = true
        };

        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var expectedDto = new TenantDto
        {
            Id = 1,
            Name = "Test Tenant",
            Domain = "test.com",
            SubscriptionPlan = "Basic",
            IsActive = true
        };

        _mapperMock.Setup(x => x.Map<TenantDto>(It.IsAny<Tenant>()))
            .Returns(expectedDto);

        // Act
        var result = await _tenantService.GetTenantAsync(1);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Tenant");
    }

    [Fact]
    public async Task GetTenantAsync_WithInvalidId_ShouldReturnError()
    {
        // Act
        var result = await _tenantService.GetTenantAsync(999);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Tenant not found");
    }
}
