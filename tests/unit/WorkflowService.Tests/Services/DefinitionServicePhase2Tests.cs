using AutoMapper;
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
using System.Linq;

namespace WorkflowService.Tests.Services;

public class DefinitionServicePhase2Tests : TestBase
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IEventPublisher> _events = new();
    private readonly Mock<IGraphValidationService> _graph = new();
    private readonly Mock<ILogger<DefinitionService>> _logger = new();
    private DefinitionService CreateService() => new(
        DbContext, _mapper.Object, _tenant.Object, _events.Object, _graph.Object, _logger.Object);

    public DefinitionServicePhase2Tests()
    {
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);

        // Removed Tags mapping (WorkflowDefinitionDto does not expose Tags)
        _mapper.Setup(m => m.Map<WorkflowDefinitionDto>(It.IsAny<WorkflowDefinition>()))
            .Returns((WorkflowDefinition def) => new WorkflowDefinitionDto
            {
                Id = def.Id,
                Name = def.Name,
                Version = def.Version,
                JSONDefinition = def.JSONDefinition,
                IsPublished = def.IsPublished,
                CreatedAt = def.CreatedAt,
                UpdatedAt = def.UpdatedAt,
                PublishedAt = def.PublishedAt
            });

        // Collection mapping – without this, _mapper.Map<List<WorkflowDefinitionDto>>(...) returns null causing all GetAll tests to fail.
        _mapper
            .Setup(m => m.Map<List<WorkflowDefinitionDto>>(It.IsAny<List<WorkflowDefinition>>()))
            .Returns((List<WorkflowDefinition> defs) =>
                defs.Select(d => new WorkflowDefinitionDto
                {
                    Id          = d.Id,
                    Name        = d.Name,
                    Version     = d.Version,
                    JSONDefinition = d.JSONDefinition,
                    IsPublished = d.IsPublished,
                    CreatedAt   = d.CreatedAt,
                    UpdatedAt   = d.UpdatedAt,
                    PublishedAt = d.PublishedAt
                }).ToList());

        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto { IsValid = true });
    }

    private WorkflowDefinition SeedDefinition(
        string name,
        int version = 1,
        bool published = false,
        string? tags = null,
        string json = """{"nodes":[{"id":"start","type":"Start"},{"id":"end","type":"End"}],"edges":[{"id":"e1","source":"start","target":"end"}]}""")
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = name,
            Version = version,
            JSONDefinition = json,
            IsPublished = published,
            Tags = tags,
            PublishedAt = published ? DateTime.UtcNow.AddMinutes(-version) : null,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10 - version),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-5 - version)
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();
        return def;
    }

    [Fact]
    public async Task DeleteDraft_Success()
    {
        var svc = CreateService();
        var def = SeedDefinition("DraftA");
        var resp = await svc.DeleteDraftAsync(def.Id);
        resp.Success.Should().BeTrue();
        (await DbContext.WorkflowDefinitions.FindAsync(def.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteDraft_Published_ShouldFail()
    {
        var svc = CreateService();
        var def = SeedDefinition("Published", published: true);
        var resp = await svc.DeleteDraftAsync(def.Id);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("published");
    }

    [Fact]
    public async Task DeleteDraft_WithInstances_ShouldFail()
    {
        var svc = CreateService();
        var def = SeedDefinition("HasInstances");
        DbContext.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await DbContext.SaveChangesAsync();
        var resp = await svc.DeleteDraftAsync(def.Id);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("existing instances");
    }

    [Fact]
    public async Task DeleteDraft_NotFound_ShouldFail()
    {
        var svc = CreateService();
        var resp = await svc.DeleteDraftAsync(9999);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task CreateNewVersion_InvalidStructure_ShouldReturnError()
    {
        var svc = CreateService();
        var baseDef = SeedDefinition("BaseDef", published: true);
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<string> { "No Start node" }
            });

        var resp = await svc.CreateNewVersionAsync(baseDef.Id, new CreateNewVersionRequestDto
        {
            Name = "Next",
            JSONDefinition = "{}"
        });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Invalid workflow definition");
    }

    [Fact]
    public async Task CreateNewVersion_ValidatorThrows_ShouldReturnFailure()
    {
        var svc = CreateService();
        var baseDef = SeedDefinition("BaseDef2", published: true);
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Throws(new InvalidOperationException("Parse exploded"));

        var resp = await svc.CreateNewVersionAsync(baseDef.Id, new CreateNewVersionRequestDto
        {
            Name = "Next",
            JSONDefinition = "{}"
        });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Failed");
    }

    [Fact]
    public async Task ValidateDefinition_Invalid_ShouldReturnErrorDtos()
    {
        var svc = CreateService();
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<string> { "Edge e1 source 'x' does not exist", "Unreachable nodes: orphan" }
            });

        var resp = await svc.ValidateDefinitionAsync(new ValidateDefinitionRequestDto
        {
            JSONDefinition = "{}"
        });

        resp.Success.Should().BeFalse();
        resp.Errors.Should().HaveCount(2);
        resp.Errors![0].Code.Should().Be("Validation");
    }

    [Fact]
    public async Task ValidateDefinition_Exception_ShouldReturnFailure()
    {
        var svc = CreateService();
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Throws(new Exception("boom"));

        var resp = await svc.ValidateDefinitionAsync(new ValidateDefinitionRequestDto
        {
            JSONDefinition = "{}"
        });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Failed");
    }

    public static IEnumerable<object[]> SortCases()
    {
        yield return new object[] { "name", false, "Alpha" };
        yield return new object[] { "name", true, "Zulu" };
        yield return new object[] { "version", true, "AlphaV2" };
        yield return new object[] { "version", false, "Alpha" };
        yield return new object[] { "publishedAt", true,  "PublishedRecent" };
        yield return new object[] { "publishedAt", false, "PublishedOld" };
        yield return new object[] { "createdAt", true, "Late" };
        // createdAt ascending returns the OLDEST (earliest timestamp) first -> PublishedOld (-40 mins)
        yield return new object[] { "createdAt", false, "PublishedOld" };
    }

    private void SeedForSorting()
    {
        foreach (var d in DbContext.WorkflowDefinitions.ToList())
            DbContext.WorkflowDefinitions.Remove(d);
        DbContext.SaveChanges();

        var now = DateTime.UtcNow;

        // Base set
        var early          = SeedDefinition("Early");
        var late           = SeedDefinition("Late");
        var publishedOld   = SeedDefinition("PublishedOld", published: true);
        var publishedRecent= SeedDefinition("PublishedRecent", published: true);
        var alphaV1        = SeedDefinition("Alpha",   version: 1);
        var alphaV2        = SeedDefinition("AlphaV2", version: 2);
        var zulu           = SeedDefinition("Zulu");

        // Explicit, deterministic CreatedAt ordering (latest -> oldest):
        // Late (most recent), Zulu, AlphaV2, Alpha, PublishedRecent, Early, PublishedOld
        late.CreatedAt            = now.AddMinutes(-5);
        zulu.CreatedAt            = now.AddMinutes(-10);
        alphaV2.CreatedAt         = now.AddMinutes(-15);
        alphaV1.CreatedAt         = now.AddMinutes(-20);
        publishedRecent.CreatedAt = now.AddMinutes(-25);
        early.CreatedAt           = now.AddMinutes(-30);
        publishedOld.CreatedAt    = now.AddMinutes(-40);

        // Keep PublishedAt semantics (older vs recent)
        publishedOld.PublishedAt    = now.AddHours(-6);
        publishedRecent.PublishedAt = now.AddHours(-1);

        DbContext.SaveChanges();
    }

    [Theory]
    [MemberData(nameof(SortCases))]
    public async Task GetAll_Sorting_ShouldReturnExpectedFirst(string sortBy, bool desc, string expectedFirst)
    {
        SeedForSorting();
        var svc = CreateService();

        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            Page = 1,
            PageSize = 50,
            SortBy = sortBy,
            SortDescending = desc
        });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().NotBeEmpty();
        resp.Data.Items.First().Name.Should().Be(expectedFirst);
    }

    [Fact]
    public async Task GetAll_TagFilter_ShouldReturnOnlyTagged()
    {
        SeedDefinition("TagA", tags: "billing,core");
        SeedDefinition("TagB", tags: "reports");
        SeedDefinition("NoTag");

        var svc = CreateService();
        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            Tags = "reports",
            Page = 1,
            PageSize = 20
        });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().HaveCount(1);
        resp.Data.Items[0].Name.Should().Be("TagB");
    }

    [Fact]
    public async Task GetAll_SearchTerm_CaseInsensitive()
    {
        SeedDefinition("InvoiceProcessor");
        SeedDefinition("invoice-audit");
        SeedDefinition("Payments");

        var svc = CreateService();
        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            SearchTerm = "INVOICE",
            Page = 1,
            PageSize = 20
        });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().HaveCount(2);
        resp.Data.Items.Select(i => i.Name).Should().Contain(new[] { "InvoiceProcessor", "invoice-audit" });
    }

    [Fact]
    public async Task GetAll_PaginationBeyondRange_ShouldReturnEmpty()
    {
        SeedDefinition("OnlyOne");
        var svc = CreateService();

        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            Page = 5,
            PageSize = 10
        });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().BeEmpty();
        resp.Data.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_ShouldApplyGatewayEnrichmentInDto()
    {
        var rawJson = """
        {
          "nodes":[
            {"id":"start","type":"Start"},
            {"id":"g1","type":"Gateway"},
            {"id":"end","type":"End"}
          ],
          "edges":[
            {"id":"e1","source":"start","target":"g1"},
            {"id":"e2","source":"g1","target":"end"}
          ]
        }
        """;
        var def = SeedDefinition("GatewayWF", json: rawJson);

        var svc = CreateService();
        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            Page = 1,
            PageSize = 10
        });

        resp.Success.Should().BeTrue();
        var enriched = resp.Data!.Items.Single(i => i.Id == def.Id);
        enriched.JSONDefinition.Should().NotBe(rawJson);
    }

    [Fact]
    public async Task GetAll_TenantMissing_ShouldReturnError()
    {
        var tenantMissing = new Mock<ITenantProvider>();
        tenantMissing.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync((int?)null);

        var svc = new DefinitionService(
            DbContext,
            _mapper.Object,
            tenantMissing.Object,
            _events.Object,
            _graph.Object,
            _logger.Object);

        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            Page = 1,
            PageSize = 10
        });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Tenant");
    }

    [Fact]
    public async Task GetAll_IsPublishedFilter_ShouldReturnOnlyPublished()
    {
        // Arrange
        SeedDefinition("Draft1", published: false);
        SeedDefinition("Draft2", published: false);
        var pub1 = SeedDefinition("Pub1", published: true);
        var pub2 = SeedDefinition("Pub2", published: true);

        var svc = CreateService();

        // Act
        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            IsPublished = true,
            Page = 1,
            PageSize = 20
        });

        // Assert
        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().OnlyContain(d => d.IsPublished);
        resp.Data.Items.Select(i => i.Name).Should().BeEquivalentTo(new[] { "Pub1", "Pub2" });
    }

    [Fact]
    public async Task CreateNewVersion_MalformedJson_ShouldReturnInvalidDefinitionError()
    {
        // Arrange
        var svc = CreateService();
        var baseDef = SeedDefinition("BaseMalformed", published: true);

        // Simulate validator returning JSON parse failure (distinct from structural error & from thrown exception case)
        _graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<string> { "Invalid JSON: Unexpected character" }
            });

        // Act
        var resp = await svc.CreateNewVersionAsync(baseDef.Id, new CreateNewVersionRequestDto
        {
            Name = "Next",
            JSONDefinition = "{ invalid json"
        });

        // Assert
        resp.Success.Should().BeFalse();
        resp.Message.Should().StartWith("Invalid workflow definition");
        resp.Message.Should().Contain("Invalid JSON", "should surface parse failure coming from validator");
    }

    [Fact]
    public async Task UpdateDraft_FailurePath_ShouldReturnError()
    {
        // Arrange: create a draft
        var draft = SeedDefinition("UpdFailDraft");
        // Force mapper to throw at mapping time (end of try block)
        var throwingMapper = new Mock<IMapper>();
        throwingMapper.Setup(m => m.Map<WorkflowDefinitionDto>(It.IsAny<WorkflowDefinition>()))
            .Throws(new Exception("map explode"));

        var svc = new DefinitionService(
            DbContext,
            throwingMapper.Object,
            _tenant.Object,
            _events.Object,
            _graph.Object,
            _logger.Object);

        // Act
        var resp = await svc.UpdateDraftAsync(draft.Id, new UpdateWorkflowDefinitionDto
        {
            Name = "NewName"
        });

        // Assert: generic failure message from catch
        resp.Success.Should().BeFalse();
        resp.Message.Should().Be("Failed to update workflow definition");
    }

    [Fact]
    public async Task DeleteDraft_FailurePath_ShouldReturnError()
    {
        // Arrange: create a draft to delete
        var draft = SeedDefinition("DelFailDraft");
        // Create a tenant provider that returns the tenant once, then throws on second call
        // (service only calls it once, so instead we simulate a SaveChanges failure
        // by disposing the context before the call).
        var disposedContext = DbContext;
        disposedContext.Dispose();

        var svc = new DefinitionService(
            disposedContext, // this will cause SaveChangesAsync to throw
            _mapper.Object,
            _tenant.Object,
            _events.Object,
            _graph.Object,
            _logger.Object);

        // Act
        var resp = await svc.DeleteDraftAsync(draft.Id);

        // Assert
        resp.Success.Should().BeFalse();
        resp.Message.Should().Be("Failed to delete workflow definition");
    }
}
