using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Persistence;
using Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.TestSupport;

public static class TenantFilteredServiceContextFactory
{
    public static WorkflowDbContext Create(string dbName, int tenantId)
    {
        // Ensure filters enabled
        Environment.SetEnvironmentVariable("ENABLE_TENANT_FILTERS_IN_TESTS", "true");

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;

        var http = new Mock<IHttpContextAccessor>();
        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);

        var logger = new LoggerFactory().CreateLogger<WorkflowDbContext>();

        return new WorkflowDbContext(options, http.Object, tenantProvider.Object, logger);
    }
}
