using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Persistence;
using Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.TestSupport;

public static class TestDbContextFactory
{
    public static WorkflowDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase(dbName)
            .EnableSensitiveDataLogging()
            .Options;

        var http = new Mock<IHttpContextAccessor>();
        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(t => t.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        var logger = new LoggerFactory().CreateLogger<WorkflowDbContext>();

        return new WorkflowDbContext(options, http.Object, tenantProvider.Object, logger);
    }
}
