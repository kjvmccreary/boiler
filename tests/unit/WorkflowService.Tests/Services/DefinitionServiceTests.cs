using FluentAssertions;
using Moq;
using Xunit;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using DTOs.Workflow;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using Contracts.Services;
using Microsoft.Extensions.Logging;
using WorkflowService.Services.Validation;
using WorkflowService.Domain.Dsl;

namespace WorkflowService.Tests.Services;

public class DefinitionServiceTests : TestBase
{
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<IGraphValidationService> _mockGraphValidator;
    private readonly Mock<ILogger<DefinitionService>> _mockLogger;
    private readonly DefinitionService _definitionService;

    public DefinitionServiceTests()
    {
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _mockGraphValidator = new Mock<IGraphValidationService>();
        _mockLogger = CreateMockLogger<DefinitionService>();

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        // ✅ FIXED: Use the correct DTO type that the interface expects
        _mockGraphValidator.Setup(v => v.Validate(It.IsAny<string>(), It.IsAny<bool>()))
                          .Returns(new ValidationResultDto  // ✅ CHANGED: Use ValidationResultDto instead of ValidationResult
                          {
                              IsValid = true,               // ✅ CHANGED: Set IsValid directly (it's not computed)
                              Errors = new List<string>(),  // Empty = valid
                              Warnings = new List<string>()
                          });

        _definitionService = new DefinitionService(
            DbContext,
            Mapper,
            _mockTenantProvider.Object,
            _mockEventPublisher.Object,
            _mockGraphValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateDraftAsync_ValidRequest_ShouldCreateDraft()
    {
        // Arrange
        var request = new CreateWorkflowDefinitionDto
        {
            Name = "Test Workflow",
            Description = "Test workflow description",
            JSONDefinition = """
            {
                "id": "test-workflow",
                "name": "Test Workflow",
                "nodes": [
                    {
                        "id": "start",
                        "type": "Start",
                        "name": "Start Node",
                        "properties": {}
                    },
                    {
                        "id": "end",
                        "type": "End",
                        "name": "End Node",
                        "properties": {}
                    }
                ],
                "edges": [
                    {
                        "id": "edge1",
                        "source": "start",
                        "target": "end",
                        "condition": null
                    }
                ]
            }
            """
        };

        // Act
        var result = await _definitionService.CreateDraftAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Workflow");
        result.Data.IsPublished.Should().BeFalse();
        result.Data.Version.Should().Be(1);
    }

    [Fact]
    public async Task PublishAsync_ValidDraft_ShouldPublish()
    {
        // Arrange
        var draft = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Draft Workflow",
            Version = 1,
            JSONDefinition = """
            {
                "id": "draft-workflow",
                "name": "Draft Workflow",
                "nodes": [
                    {
                        "id": "start",
                        "type": "Start",
                        "name": "Start Node",
                        "properties": {}
                    },
                    {
                        "id": "task1",
                        "type": "HumanTask",
                        "name": "Approval Task",
                        "properties": {
                            "taskName": "Approval Task"
                        }
                    },
                    {
                        "id": "end",
                        "type": "End",
                        "name": "End Node",
                        "properties": {}
                    }
                ],
                "edges": [
                    {
                        "id": "edge1",
                        "source": "start",
                        "target": "task1",
                        "condition": null
                    },
                    {
                        "id": "edge2",
                        "source": "task1",
                        "target": "end",
                        "condition": null
                    }
                ]
            }
            """,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(draft);
        await DbContext.SaveChangesAsync();

        var request = new PublishDefinitionRequestDto
        {
            PublishNotes = "Initial publish"
        };

        // Act
        var result = await _definitionService.PublishAsync(draft.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsPublished.Should().BeTrue();
        result.Data.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task PublishAsync_AlreadyPublished_ShouldReturnError()
    {
        // Arrange
        var publishedDefinition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Published Workflow",
            Version = 1,
            JSONDefinition = """
            {
                "id":"published-workflow",
                "name":"Published Workflow",
                "nodes":[{"id":"start","type":"Start"},{"id":"end","type":"End"}],
                "edges":[{"id":"e","source":"start","target":"end"}]
            }
            """,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(publishedDefinition);
        await DbContext.SaveChangesAsync();

        var request = new PublishDefinitionRequestDto { PublishNotes = "Attempting republish" };

        // Act
        var result = await _definitionService.PublishAsync(publishedDefinition.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().NotBeNull();
        result.Message!.ToLowerInvariant().Should().Contain("already published");
    }

    [Fact]
    public async Task GetAllAsync_WithFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var validJson = """
        {
            "id": "test-workflow",
            "name": "Test Workflow",
            "nodes": [
                {
                    "id": "start",
                    "type": "Start",
                    "name": "Start Node",
                    "properties": {}
                },
                {
                    "id": "end",
                    "type": "End",
                    "name": "End Node",
                    "properties": {}
                }
            ],
            "edges": [
                {
                    "id": "edge1",
                    "source": "start",
                    "target": "end",
                    "condition": null
                }
            ]
        }
        """;

        var definitions = new[]
        {
            new WorkflowDefinition
            {
                TenantId = 1,
                Name = "Published Workflow",
                Version = 1,
                JSONDefinition = validJson,
                IsPublished = true,
                PublishedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new WorkflowDefinition
            {
                TenantId = 1,
                Name = "Draft Workflow",
                Version = 1,
                JSONDefinition = validJson,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        DbContext.WorkflowDefinitions.AddRange(definitions);
        await DbContext.SaveChangesAsync();

        var request = new GetWorkflowDefinitionsRequestDto
        {
            IsPublished = true,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _definitionService.GetAllAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().IsPublished.Should().BeTrue();
    }
}
