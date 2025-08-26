using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Persistence;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace WorkflowService.Tests.Controllers;

public class InstancesControllerTests : TestBase
{
    private readonly Mock<ILogger<InstancesController>> _mockLogger;
    private readonly InstancesController _controller;

    public InstancesControllerTests()
    {
        _mockLogger = CreateMockLogger<InstancesController>();
        
        // Use actual DbContext from TestBase
        _controller = new InstancesController(
            DbContext,
            _mockLogger.Object);

        SetupControllerContext();
    }

    [Fact]
    public async Task GetInstances_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        await SeedTestInstancesAsync();

        // Act - Use actual controller method signature
        var result = await _controller.GetInstances(
            status: InstanceStatus.Running,
            workflowDefinitionId: null,
            startedByUserId: null,
            page: 1,
            pageSize: 50);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetInstance_ExistingId_ShouldReturnOkResult()
    {
        // Arrange
        var instance = await CreateTestInstanceAsync();

        // Act - Use actual controller method name
        var result = await _controller.GetInstance(instance.Id);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task StartInstance_ValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var definition = await CreateTestDefinitionAsync();
        var request = new StartInstanceRequestDto
        {
            WorkflowDefinitionId = definition.Id,
            InitialContext = """{"requestId": 123}"""
        };

        // Act
        var result = await _controller.StartInstance(request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as CreatedAtActionResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task TerminateInstance_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var instance = await CreateTestInstanceAsync();

        // Act - Use correct method signature (no request parameter)
        var result = await _controller.TerminateInstance(instance.Id);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    private async Task SeedTestInstancesAsync()
    {
        var definition = await CreateTestDefinitionAsync();
        
        var instance = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = """["start"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowService.Domain.Models.WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();
    }

    private async Task<WorkflowService.Domain.Models.WorkflowInstance> CreateTestInstanceAsync()
    {
        var definition = await CreateTestDefinitionAsync();
        
        var instance = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = """["start"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowService.Domain.Models.WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();
        return instance;
    }

    private async Task<WorkflowService.Domain.Models.WorkflowDefinition> CreateTestDefinitionAsync()
    {
        var definition = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = 1,
            Name = "Test Workflow",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.WorkflowDefinitions.Add(definition);
        await DbContext.SaveChangesAsync();
        return definition;
    }

    private void SetupControllerContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("tenantId", "1"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        httpContext.Request.Headers["X-Tenant-ID"] = "1";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}
