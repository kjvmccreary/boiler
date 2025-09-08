using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using Moq;
using AutoMapper;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Contracts.Services;
using WorkflowService.Engine.Validation;

namespace WorkflowService.Tests.Definitions;

public class DefinitionListArchivedFilterTests : TestBase
{
    private DefinitionService BuildService(int tenantId = 1)
    {
        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);

        var mapperCfg = new MapperConfiguration(c =>
        {
            c.CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        });
        var mapper = mapperCfg.CreateMapper();

        var events = new Mock<IEventPublisher>();
        var graph = new Mock<IGraphValidationService>();
        graph.Setup(g => g.Validate(It.IsAny<string>(), true))
            .Returns(new ValidationResultDto { IsValid = true });
        var publishValidator = new Mock<IWorkflowPublishValidator>();
        var logger = new Mock<ILogger<DefinitionService>>();

        return new DefinitionService(
            DbContext,
            mapper,
            tenantProvider.Object,
            events.Object,
            graph.Object,
            logger.Object,
            publishValidator.Object);
    }

    [Fact]
    public async Task GetAll_Default_ShouldExcludeArchived()
    {
        DbContext.WorkflowDefinitions.AddRange(
            new WorkflowDefinition
            {
                TenantId = 1,
                Name = "ActiveA",
                Version = 1,
                JSONDefinition = "{}",
                IsPublished = false,
                IsArchived = false
            },
            new WorkflowDefinition
            {
                TenantId = 1,
                Name = "ArchivedA",
                Version = 1,
                JSONDefinition = "{}",
                IsPublished = false,
                IsArchived = true
            }
        );
        await DbContext.SaveChangesAsync();

        var svc = BuildService();

        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto { PageSize = 10 });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().Contain(d => d.Name == "ActiveA");
        resp.Data.Items.Should().NotContain(d => d.Name == "ArchivedA");
    }

    [Fact]
    public async Task GetAll_IncludeArchived_ShouldReturnBoth()
    {
        DbContext.WorkflowDefinitions.AddRange(
            new WorkflowDefinition
            {
                TenantId = 1,
                Name = "ActiveB",
                Version = 1,
                JSONDefinition = "{}",
                IsPublished = false,
                IsArchived = false
            },
            new WorkflowDefinition
            {
                TenantId = 1,
                Name = "ArchivedB",
                Version = 1,
                JSONDefinition = "{}",
                IsPublished = false,
                IsArchived = true
            }
        );
        await DbContext.SaveChangesAsync();

        var svc = BuildService();

        var resp = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto
        {
            PageSize = 10,
            IncludeArchived = true
        });

        resp.Success.Should().BeTrue();
        var names = resp.Data!.Items.Select(i => i.Name).ToList();
        names.Should().Contain("ActiveB");
        names.Should().Contain("ArchivedB");
    }
}
