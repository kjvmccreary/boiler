using System.Security.Claims;
using DTOs.Common;
using DTOs.Workflow;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Controllers;
using WorkflowService.Services.Interfaces;
using WorkflowService.Persistence;
using Common.Constants;
using Xunit;
using WorkflowService.Services.Validation;
using WorkflowService.Engine;

namespace WorkflowService.Tests.Controllers;

public class DefinitionsControllerTests : TestBase
{
    private readonly Mock<IDefinitionService> _mockDefinitionService;
    private readonly Mock<ILogger<DefinitionsController>> _mockLogger;
    private readonly Mock<IWorkflowGraphValidator> _mockGraphValidator;
    private readonly DefinitionsController _controller;

    public DefinitionsControllerTests()
    {
        _mockDefinitionService = new Mock<IDefinitionService>();
        _mockLogger = CreateMockLogger<DefinitionsController>();
        _mockGraphValidator = new Mock<IWorkflowGraphValidator>();

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
            new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "1"),
            new Claim("tenantId", "1"),
            new Claim("permission", Permissions.Workflow.ViewDefinitions),
            new Claim("permission", Permissions.Workflow.CreateDefinitions),
            new Claim("permission", Permissions.Workflow.EditDefinitions),
            new Claim("permission", Permissions.Workflow.PublishDefinitions),
            new Claim("permission", Permissions.Workflow.ManageInstances)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var http = new DefaultHttpContext { User = principal };
        http.Request.Headers["X-Tenant-ID"] = "1";
        controller.ControllerContext = new ControllerContext { HttpContext = http };
    }

    private WorkflowService.Domain.Models.WorkflowDefinition SeedDefinition(
        bool published = false,
        bool archived = false,
        string name = "Test Def")
    {
        var def = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = 1,
            Name = name,
            Version = 1,
            JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
            IsPublished = published,
            IsArchived = archived,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();
        return def;
    }

    private WorkflowService.Domain.Models.WorkflowInstance SeedRunningInstance(int defId)
    {
        var inst = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = defId,
            DefinitionVersion = 1,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(inst);
        DbContext.SaveChanges();
        return inst;
    }

    #endregion

    #region List & Get

    [Fact]
    public async Task GetAll_ShouldReturnDefinitions()
    {
        var defs = new List<WorkflowDefinitionDto>
        {
            new() { Id = 1, Name = "Def1", Version = 1, JSONDefinition = "{}", IsPublished = true, CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow },
            new() { Id = 2, Name = "Def2", Version = 1, JSONDefinition = "{}", IsPublished = false, CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow }
        };

        _mockDefinitionService
            .Setup(s => s.GetAllAsync(It.IsAny<GetWorkflowDefinitionsRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.SuccessResult(
                new PagedResultDto<WorkflowDefinitionDto>
                {
                    Items = defs,
                    TotalCount = defs.Count,
                    Page = 1,
                    PageSize = 100
                }));

        var result = await _controller.GetAll();
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var payload = ok!.Value as ApiResponseDto<List<WorkflowDefinitionDto>>;
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_Existing_ShouldReturnOk()
    {
        var dto = new WorkflowDefinitionDto
        {
            Id = 10,
            Name = "Def10",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDefinitionService
            .Setup(s => s.GetByIdAsync(10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto));

        var result = await _controller.Get(10, default);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task Get_NotFound_ShouldReturn404()
    {
        _mockDefinitionService
            .Setup(s => s.GetByIdAsync(999, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Not found"));

        var result = await _controller.Get(999, default);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Create / Update / Delete

    [Fact]
    public async Task CreateDraft_ShouldReturnCreated()
    {
        var dto = new WorkflowDefinitionDto
        {
            Id = 101,
            Name = "Draft101",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDefinitionService
            .Setup(s => s.CreateDraftAsync(It.IsAny<CreateWorkflowDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto));

        var response = await _controller.CreateDraft(new CreateWorkflowDefinitionDto
        {
            Name = "Draft101",
            JSONDefinition = "{}"
        }, default);

        response.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task UpdateDraft_ServiceError_ShouldReturnBadRequest()
    {
        _mockDefinitionService
            .Setup(s => s.UpdateDraftAsync(55, It.IsAny<UpdateWorkflowDefinitionDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("error"));

        var result = await _controller.UpdateDraft(55, new UpdateWorkflowDefinitionDto { Name = "X" }, default);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteDraft_ShouldReturnOk()
    {
        _mockDefinitionService
            .Setup(s => s.DeleteDraftAsync(77, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<bool>.SuccessResult(true));

        var result = await _controller.DeleteDraft(77, default);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Publish

    [Fact]
    public async Task Publish_Success_ShouldReturnOk()
    {
        var dto = new WorkflowDefinitionDto
        {
            Id = 5,
            Name = "Pub5",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // ✅ FIXED: Create ValidationResult properly - IsValid is computed from Errors.Count
        _mockGraphValidator
            .Setup(v => v.ValidateForPublish(It.IsAny<WorkflowService.Domain.Dsl.WorkflowDefinitionJson>()))
            .Returns(new WorkflowService.Domain.Dsl.ValidationResult 
            { 
                Errors = new List<string>(), // Empty = valid
                Warnings = new List<string>() 
            });

        _mockDefinitionService
            .Setup(s => s.GetByIdAsync(5, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto));

        _mockDefinitionService
            .Setup(s => s.PublishAsync(5, It.IsAny<PublishDefinitionRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto));

        var result = await _controller.Publish(5, new PublishDefinitionRequestDto(), default);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task Publish_Failure_ShouldReturnBadRequest()
    {
        // Existing definition returned first
        var existingDto = new WorkflowDefinitionDto
        {
            Id = 6,
            Name = "Def6",
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[{"id":"start","type":"Start"},{"id":"end","type":"End"}],
              "edges":[{"id":"e","source":"start","target":"end"}]
            }
            """,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDefinitionService
            .Setup(s => s.GetByIdAsync(6, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(existingDto));

        // Validation passes so PublishAsync is reached
        _mockGraphValidator
            .Setup(v => v.ValidateForPublish(It.IsAny<WorkflowService.Domain.Dsl.WorkflowDefinitionJson>()))
            .Returns(new WorkflowService.Domain.Dsl.ValidationResult
            {
                Errors = new List<string>(),
                Warnings = new List<string>()
            });

        // Publishing fails
        _mockDefinitionService
            .Setup(s => s.PublishAsync(6, It.IsAny<PublishDefinitionRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Invalid"));

        var result = await _controller.Publish(6, new PublishDefinitionRequestDto(), default);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region New Version / Revalidate / Validate

    [Fact]
    public async Task CreateNewVersion_ShouldReturnOk()
    {
        var dto = new WorkflowDefinitionDto
        {
            Id = 201,
            Name = "NewVer",
            Version = 2,
            JSONDefinition = "{}",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDefinitionService
            .Setup(s => s.CreateNewVersionAsync(10, It.IsAny<CreateNewVersionRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto));

        var result = await _controller.CreateNewVersion(10, new CreateNewVersionRequestDto
        {
            Name = "NewVer",
            JSONDefinition = "{}"
        }, default);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Revalidate_ShouldReturnOk()
    {
        var dto = new WorkflowDefinitionDto
        {
            Id = 12,
            Name = "Def12",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDefinitionService
            .Setup(s => s.GetByIdAsync(12, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto));

        _mockDefinitionService
            .Setup(s => s.ValidateDefinitionAsync(It.IsAny<ValidateDefinitionRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<ValidationResultDto>.SuccessResult(new ValidationResultDto { IsValid = true }));

        var result = await _controller.Revalidate(12, default);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Validate_ShouldReturnBadRequest_WhenInvalid()
    {
        _mockDefinitionService
            .Setup(s => s.ValidateDefinitionAsync(It.IsAny<ValidateDefinitionRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<ValidationResultDto>.ErrorResult("Invalid JSON"));

        var result = await _controller.Validate(new ValidateDefinitionRequestDto { JSONDefinition = "{" }, default);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Unpublish / Archive / Terminate (direct DB)

    [Fact]
    public async Task Unpublish_ShouldSetIsPublishedFalse()
    {
        var def = SeedDefinition(published: true);
        var result = await _controller.Unpublish(def.Id, default);
        result.Result.Should().BeOfType<OkObjectResult>();
        (await DbContext.WorkflowDefinitions.FindAsync(def.Id))!.IsPublished.Should().BeFalse();
    }

    [Fact]
    public async Task Archive_ShouldSetIsArchivedTrue()
    {
        var def = SeedDefinition();
        var result = await _controller.Archive(def.Id, default);
        result.Result.Should().BeOfType<OkObjectResult>();
        (await DbContext.WorkflowDefinitions.FindAsync(def.Id))!.IsArchived.Should().BeTrue();
    }

    [Fact]
    public async Task TerminateRunning_ShouldCancelInstances()
    {
        var def = SeedDefinition(published: true);
        var inst = SeedRunningInstance(def.Id);
        var result = await _controller.TerminateRunning(def.Id, default);
        result.Result.Should().BeOfType<OkObjectResult>();
        (await DbContext.WorkflowInstances.FirstAsync(i => i.Id == inst.Id)).Status
            .Should().Be(DTOs.Workflow.Enums.InstanceStatus.Cancelled);
    }

    #endregion
}
