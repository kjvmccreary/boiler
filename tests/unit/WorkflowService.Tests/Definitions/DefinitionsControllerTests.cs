using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Controllers;
using WorkflowService.Services.Interfaces;
using WorkflowService.Services.Validation;
using DTOs.Workflow;
using DTOs.Common;
using Xunit;

namespace WorkflowService.Tests.Definitions;

public class DefinitionsControllerTests : TestBase
{
    private readonly Mock<IDefinitionService> _mockDefinitionService = new();
    private readonly Mock<ILogger<DefinitionsController>> _mockLogger;
    private readonly Mock<IWorkflowGraphValidator> _mockGraphValidator = new();
    private readonly DefinitionsController _controller;

    public DefinitionsControllerTests()
    {
        _mockLogger = CreateMockLogger<DefinitionsController>();
        _controller = new DefinitionsController(
            _mockDefinitionService.Object,
            MockTenantProvider.Object,
            DbContext,
            _mockLogger.Object,
            _mockGraphValidator.Object);

        OverrideControllerHttpContextWithPermissionClaims(_controller);
    }

    #region Helpers

    private void OverrideControllerHttpContextWithPermissionClaims(ControllerBase controller)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new("tenantId", "1"),
            new("permission", Common.Constants.Permissions.Workflow.ViewDefinitions),
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var http = new DefaultHttpContext { User = principal };
        http.Request.Headers["X-Tenant-ID"] = "1";
        controller.ControllerContext = new ControllerContext { HttpContext = http };
    }

    private static PagedResultDto<WorkflowDefinitionDto> Paged(params WorkflowDefinitionDto[] defs) =>
        new()
        {
            Items = defs.ToList(),
            TotalCount = defs.Length,
            Page = 1,
            PageSize = defs.Length == 0 ? 1 : defs.Length
        };

    #endregion

    [Fact]
    public async Task GetAll_Default_ShouldExcludeArchived()
    {
        _mockDefinitionService
            .Setup(s => s.GetAllAsync(
                It.Is<GetWorkflowDefinitionsRequestDto>(r => r.IncludeArchived == false),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.SuccessResult(
                Paged(
                    new WorkflowDefinitionDto
                    {
                        Id = 1,
                        Name = "ActiveDraft",
                        Version = 1,
                        JSONDefinition = "{}",
                        IsPublished = false,
                        IsArchived = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new WorkflowDefinitionDto
                    {
                        Id = 2,
                        Name = "ActivePublished",
                        Version = 1,
                        JSONDefinition = "{}",
                        IsPublished = true,
                        IsArchived = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                )));

        var result = await _controller.GetAll(includeArchived: false);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var envelope = ok!.Value as ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>;
        envelope.Should().NotBeNull();
        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().NotBeNull();
        envelope.Data!.Items.Should().OnlyContain(d => d.IsArchived == false);
        envelope.Data!.Items.Select(d => d.Name).Should().Contain(new[] { "ActiveDraft", "ActivePublished" });
    }

    [Fact]
    public async Task GetAll_IncludeArchived_ShouldReturnArchived()
    {
        _mockDefinitionService
            .Setup(s => s.GetAllAsync(
                It.Is<GetWorkflowDefinitionsRequestDto>(r => r.IncludeArchived == true),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.SuccessResult(
                Paged(
                    new WorkflowDefinitionDto
                    {
                        Id = 1,
                        Name = "ActiveDraft",
                        Version = 1,
                        JSONDefinition = "{}",
                        IsPublished = false,
                        IsArchived = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new WorkflowDefinitionDto
                    {
                        Id = 2,
                        Name = "ArchivedDraft",
                        Version = 1,
                        JSONDefinition = "{}",
                        IsPublished = false,
                        IsArchived = true,
                        ArchivedAt = DateTime.UtcNow.AddMinutes(-10),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                )));

        var result = await _controller.GetAll(includeArchived: true);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var envelope = ok!.Value as ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>;
        envelope.Should().NotBeNull();
        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().NotBeNull();

        var items = envelope.Data!.Items;
        items.Should().ContainSingle(d => d.IsArchived);
        items.Any(d => d.IsArchived).Should().BeTrue();
        items.Select(d => d.Name).Should().Contain(new[] { "ActiveDraft", "ArchivedDraft" });
    }
}
