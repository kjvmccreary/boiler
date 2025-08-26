using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Persistence;
using DTOs.Common;
using DTOs.Workflow;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace WorkflowService.Tests.Controllers;

public class DefinitionsControllerTests : TestBase
{
    private readonly Mock<ILogger<DefinitionsController>> _mockLogger;
    private readonly DefinitionsController _controller;

    public DefinitionsControllerTests()
    {
        _mockLogger = CreateMockLogger<DefinitionsController>();
        
        // Use actual DbContext from TestBase
        _controller = new DefinitionsController(
            DbContext,
            _mockLogger.Object);

        SetupControllerContext();
    }

    [Fact]
    public async Task GetDefinitions_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        await SeedTestDefinitionsAsync();

        // Act - Use actual controller method signature
        var result = await _controller.GetDefinitions(published: true);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
        
        var responseData = actionResult.Value as ApiResponseDto<List<WorkflowDefinitionDto>>;
        responseData.Should().NotBeNull();
        responseData!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetDefinition_ExistingId_ShouldReturnOkResult()
    {
        // Arrange
        var definition = await CreateTestDefinitionAsync();

        // Act - Use actual controller method name
        var result = await _controller.GetDefinition(definition.Id);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetDefinition_NonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var definitionId = 999;

        // Act
        var result = await _controller.GetDefinition(definitionId);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as NotFoundObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(404);
    }

    private async Task SeedTestDefinitionsAsync()
    {
        var definitions = new[]
        {
            new WorkflowService.Domain.Models.WorkflowDefinition
            {
                TenantId = 1,
                Name = "Published Workflow",
                Version = 1,
                JSONDefinition = "{}",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new WorkflowService.Domain.Models.WorkflowDefinition
            {
                TenantId = 1,
                Name = "Draft Workflow",
                Version = 1,
                JSONDefinition = "{}",
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        DbContext.WorkflowDefinitions.AddRange(definitions);
        await DbContext.SaveChangesAsync();
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
