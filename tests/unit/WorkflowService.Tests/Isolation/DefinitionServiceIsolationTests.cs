using Xunit;
using WorkflowService.Tests.TestSupport;
using DTOs.Workflow;
using WorkflowService.Services;
using AutoMapper;
using WorkflowService.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Contracts.Services;
using WorkflowService.Engine.Validation;
using WorkflowService.Services.Interfaces; // IEventPublisher
using DTOs.Common; // ValidationResultDto
using System;

namespace WorkflowService.Tests.Isolation;

public class DefinitionServiceIsolationTests
{
    private const string ValidJson = """
    {
      "nodes":[{"id":"start","type":"start"},{"id":"end","type":"end"}],
      "edges":[{"id":"e1","from":"start","to":"end"}]
    }
    """;

    private DefinitionService CreateService(string dbName, int tenantId)
    {
        var ctx = TenantFilteredServiceContextFactory.Create(dbName, tenantId);

        var mapperCfg = new MapperConfiguration(c =>
            c.CreateMap<WorkflowDefinition, WorkflowDefinitionDto>());
        var mapper = mapperCfg.CreateMapper();

        var tenant = new Mock<ITenantProvider>();
        tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);

        var ev = new Mock<IEventPublisher>();

        var validator = new Mock<IGraphValidationService>();
        validator.Setup(v => v.Validate(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(new ValidationResultDto { IsValid = true, Errors = new List<string>() });

        var publishValidator = new Mock<IWorkflowPublishValidator>();
        publishValidator.Setup(p => p.Validate(It.IsAny<WorkflowDefinition>(), It.IsAny<IEnumerable<WorkflowService.Domain.Dsl.WorkflowNode>>()))
            .Returns(Array.Empty<string>());

        var logger = LoggerFactory.Create(b => b.AddDebug()).CreateLogger<DefinitionService>();

        return new DefinitionService(ctx, mapper, tenant.Object, ev.Object, validator.Object, logger, publishValidator.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Filter_Out_Other_Tenant_Definitions()
    {
        var dbName = nameof(GetAllAsync_Should_Filter_Out_Other_Tenant_Definitions) + "_" + Guid.NewGuid().ToString("N");

        // Tenant 1
        var s1 = CreateService(dbName, 1);
        await s1.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "T1-Flow", JSONDefinition = ValidJson });
        await s1.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "T1-Flow-2", JSONDefinition = ValidJson });

        // Tenant 2 (same underlying DB name)
        var s2 = CreateService(dbName, 2);
        await s2.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "T2-Flow", JSONDefinition = ValidJson });

        var resp = await s1.GetAllAsync(new GetWorkflowDefinitionsRequestDto { PageSize = 50 });
        Assert.True(resp.Success);
        Assert.All(resp.Data!.Items, d => Assert.StartsWith("T1-", d.Name));
        Assert.DoesNotContain(resp.Data.Items, d => d.Name.StartsWith("T2-"));
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_NotFound_For_Foreign_Definition()
    {
        var dbName = nameof(GetByIdAsync_Should_Return_NotFound_For_Foreign_Definition) + "_" + Guid.NewGuid().ToString("N");

        var s1 = CreateService(dbName, 1);
        var s2 = CreateService(dbName, 2);

        var created = await s2.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "T2-Only", JSONDefinition = ValidJson });
        Assert.True(created.Success);

        var fetch = await s1.GetByIdAsync(created.Data!.Id);
        Assert.False(fetch.Success);
        Assert.Contains("not found", fetch.Message!, StringComparison.OrdinalIgnoreCase);
    }
}
