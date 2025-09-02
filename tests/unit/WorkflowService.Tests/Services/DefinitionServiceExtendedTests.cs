using FluentAssertions;
using Moq;
using Xunit;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Domain.Models;
using DTOs.Workflow;
using DTOs.Common;
using Contracts.Services;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace WorkflowService.Tests.Services;

public class DefinitionServiceExtendedTests : TestBase
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IEventPublisher> _events = new();
    private readonly Mock<IGraphValidationService> _graph = new();
    private readonly Mock<ILogger<DefinitionService>> _logger = new();
    private readonly DefinitionService _service;

    public DefinitionServiceExtendedTests()
    {
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);

        _mapper.Setup(m => m.Map<WorkflowDefinitionDto>(It.IsAny<WorkflowDefinition>()))
            .Returns((WorkflowDefinition src) => new WorkflowDefinitionDto
            {
                Id = src.Id,
                Name = src.Name,
                Version = src.Version,
                JSONDefinition = src.JSONDefinition,
                IsPublished = src.IsPublished,
                CreatedAt = src.CreatedAt,
                UpdatedAt = src.UpdatedAt
            });

        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto { IsValid = true });

        _service = new DefinitionService(
            DbContext,
            _mapper.Object,
            _tenant.Object,
            _events.Object,
            _graph.Object,
            _logger.Object);
    }

    private WorkflowDefinition SeedDraft(string name = "Draft1", bool published = false)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = name,
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[{"id":"start","type":"Start"},{"id":"end","type":"End"}],
              "edges":[{"id":"e1","source":"start","target":"end"}]
            }
            """,
            IsPublished = published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();
        return def;
    }

    #region UpdateDraft

    [Fact]
    public async Task UpdateDraft_Success_ShouldPersistChanges()
    {
        var def = SeedDraft();
        var req = new UpdateWorkflowDefinitionDto
        {
            Name = "Updated Name",
            Description = "Desc",
            JSONDefinition = """
            {
              "nodes":[{"id":"start","type":"Start"},{"id":"mid","type":"HumanTask"},{"id":"end","type":"End"}],
              "edges":[
                {"id":"e1","source":"start","target":"mid"},
                {"id":"e2","source":"mid","target":"end"}
              ]
            }
            """
        };

        var resp = await _service.UpdateDraftAsync(def.Id, req);

        resp.Success.Should().BeTrue();
        resp.Data!.Name.Should().Be("Updated Name");
        DbContext.WorkflowDefinitions.Find(def.Id)!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateDraft_InvalidJson_ShouldFail()
    {
        var def = SeedDraft();
        var req = new UpdateWorkflowDefinitionDto
        {
            JSONDefinition = "{ invalid json"
        };

        var resp = await _service.UpdateDraftAsync(def.Id, req);

        resp.Success.Should().BeFalse("malformed JSON should not update the draft");
        resp.Data.Should().BeNull();
        resp.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UpdateDraft_Published_ShouldFail()
    {
        var def = SeedDraft(published: true);
        var req = new UpdateWorkflowDefinitionDto { Name = "X" };

        var resp = await _service.UpdateDraftAsync(def.Id, req);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Cannot modify a published");
    }

    #endregion

    #region CreateNewVersion

    [Fact]
    public async Task CreateNewVersion_Success_ShouldIncrementVersion()
    {
        var baseDef = SeedDraft();
        baseDef.IsPublished = true;
        DbContext.SaveChanges();

        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto { IsValid = true });

        var req = new CreateNewVersionRequestDto
        {
            Name = "Next Version",
            JSONDefinition = """
            {
              "nodes":[{"id":"start","type":"Start"},{"id":"end","type":"End"}],
              "edges":[{"id":"e1","source":"start","target":"end"}]
            }
            """,
            VersionNotes = "Notes"
        };

        var resp = await _service.CreateNewVersionAsync(baseDef.Id, req);

        resp.Success.Should().BeTrue();
        resp.Data!.Version.Should().Be(2);
        resp.Data.Name.Should().Be("Next Version");
    }

    [Fact]
    public async Task CreateNewVersion_InvalidDefinition_ShouldReturnError()
    {
        var baseDef = SeedDraft();
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<string> { "Bad edge" }
            });

        var req = new CreateNewVersionRequestDto
        {
            Name = "Next",
            JSONDefinition = "{}"
        };

        var resp = await _service.CreateNewVersionAsync(baseDef.Id, req);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Invalid workflow definition");
    }

    [Fact]
    public async Task CreateNewVersion_BaseMissing_ShouldReturnError()
    {
        var resp = await _service.CreateNewVersionAsync(9999, new CreateNewVersionRequestDto
        {
            Name = "NV",
            JSONDefinition = "{}"
        });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Match(x => x != null && x.Contains("not found", StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region GetById Version Selection

    [Fact]
    public async Task GetById_WithVersion_ShouldUseDistinctIds()
    {
        // Each version row has its own Id; service does not resolve by (baseId, version).
        var v1 = SeedDraft("Original");
        var v2 = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Original",
            Version = 2,
            ParentDefinitionId = v1.Id,
            JSONDefinition = v1.JSONDefinition,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(v2);
        DbContext.SaveChanges();

        var r1 = await _service.GetByIdAsync(v1.Id, null);
        r1.Success.Should().BeTrue();
        r1.Data!.Version.Should().Be(1);

        var r2 = await _service.GetByIdAsync(v2.Id, null);
        r2.Success.Should().BeTrue();
        r2.Data!.Version.Should().Be(2);

        var r1WithV2 = await _service.GetByIdAsync(v1.Id, 2);
        r1WithV2.Success.Should().BeFalse("GetByIdAsync does not traverse version lineage");
    }

    #endregion

    #region ValidateDefinitionAsync

    [Fact]
    public async Task ValidateDefinitionAsync_Valid_ShouldReturnSuccess()
    {
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto { IsValid = true });

        var resp = await _service.ValidateDefinitionAsync(new ValidateDefinitionRequestDto
        {
            JSONDefinition = "{}"
        });

        resp.Success.Should().BeTrue();
        resp.Data!.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateDefinitionAsync_Invalid_ShouldReturnErrors()
    {
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<string> { "Missing start" }
            });

        var resp = await _service.ValidateDefinitionAsync(new ValidateDefinitionRequestDto
        {
            JSONDefinition = "{}"
        });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Be("Invalid");
        resp.Errors.Should().Contain(e => e.Message.Contains("Missing start"));
    }

    #endregion
}
