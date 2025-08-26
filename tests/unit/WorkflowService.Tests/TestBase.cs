using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Mappings;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;
using DTOs.Entities;
using Microsoft.AspNetCore.Http;
using Contracts.Services;
using System.Reflection;
using System.Security.Claims;

namespace WorkflowService.Tests;

/// <summary>
/// Base class for all WorkflowService unit tests
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly Mock<ILogger<object>> MockLogger;
    protected readonly IMapper Mapper;
    protected readonly WorkflowDbContext DbContext;
    protected readonly Mock<IHttpContextAccessor> MockHttpContextAccessor;
    protected readonly Mock<ITenantProvider> MockTenantProvider;

    protected TestBase()
    {
        MockLogger = new Mock<ILogger<object>>();
        MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        MockTenantProvider = new Mock<ITenantProvider>();
        
        // Configure tenant provider to return test tenant
        MockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);
        
        // 🔧 FIX: Set up HTTP context with user claims
        SetupHttpContext();
        
        // 🔧 FIX: Configure AutoMapper to scan the WorkflowService assembly for profiles
        var config = new MapperConfiguration(cfg =>
        {
            // Scan the WorkflowService assembly for profiles (where WorkflowMappingProfile exists)
            var workflowServiceAssembly = typeof(WorkflowService.Services.TaskService).Assembly;
            cfg.AddMaps(workflowServiceAssembly);
        });
        
        Mapper = config.CreateMapper();

        // Configure In-Memory Database with required dependencies
        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        DbContext = new WorkflowDbContext(options, MockHttpContextAccessor.Object, MockTenantProvider.Object);
        
        // Set test environment to disable query filters
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        
        // Seed test data
        SeedTestData();
    }

    private void SetupHttpContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"), // Test user ID
            new Claim("tenantId", "1"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim("permissions", "workflow.read,workflow.write")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        httpContext.Request.Headers["X-Tenant-ID"] = "1";
        httpContext.Items["TenantId"] = 1; // For tenant context
        httpContext.Items["UserId"] = 1; // For user context

        MockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(httpContext);
    }

    private void SeedTestData()
    {
        // Note: WorkflowDbContext doesn't have Tenants - it only references them
        // The tenant data lives in the main ApplicationDbContext
        // For testing, we just ensure the TenantProvider returns a valid tenant ID
    }

    protected Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    public virtual void Dispose()
    {
        DbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
