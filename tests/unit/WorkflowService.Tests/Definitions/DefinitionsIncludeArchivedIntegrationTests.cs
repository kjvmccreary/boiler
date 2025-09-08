using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Controllers;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Services.Validation;
using WorkflowService.Engine.Validation;
using DTOs.Workflow;
using DTOs.Common;
using Contracts.Services;
using Xunit;

namespace WorkflowService.IntegrationTests.Definitions;

public class DefinitionsIncludeArchivedIntegrationTests
{
    private WorkflowDbContext CreateDbContext(ITenantProvider tenantProvider)
    {
        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase($"wf-defs-int-{Guid.NewGuid():N}")
            .EnableSensitiveDataLogging()
            .Options;

        var httpAccessor = new Mock<IHttpContextAccessor>();
        var logger = new Mock<ILogger<WorkflowDbContext>>();

        return new WorkflowDbContext(options, httpAccessor.Object, tenantProvider, logger.Object);
    }

    private IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        });
        return cfg.CreateMapper();
    }

    private async Task SeedAsync(WorkflowDbContext ctx, int tenantId = 1)
    {
        var now = DateTime.UtcNow;

        ctx.WorkflowDefinitions.AddRange(
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "ActiveDraft",
                Version = 1,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = false,
                IsArchived = false,
                CreatedAt = now,
                UpdatedAt = now
            },
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "ActivePublished",
                Version = 1,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = true,
                PublishedAt = now.AddMinutes(-30),
                IsArchived = false,
                CreatedAt = now,
                UpdatedAt = now
            },
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "ArchivedDraft",
                Version = 1,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = false,
                IsArchived = true,
                ArchivedAt = now.AddMinutes(-10),
                CreatedAt = now,
                UpdatedAt = now
            },
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "ArchivedPublished",
                Version = 2,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = true,
                PublishedAt = now.AddHours(-1),
                IsArchived = true,
                ArchivedAt = now.AddMinutes(-5),
                CreatedAt = now,
                UpdatedAt = now
            }
        );

        await ctx.SaveChangesAsync();
    }

    private (DefinitionService service, WorkflowDbContext ctx) CreateDefinitionService(int tenantId = 1)
    {
        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);

        var ctx = CreateDbContext(tenantProvider.Object);

        var mapper = CreateMapper();
        var eventPublisher = new Mock<IEventPublisher>();
        var graphValidation = new Mock<IGraphValidationService>();
        var publishValidator = new Mock<IWorkflowPublishValidator>();
        var logger = new Mock<ILogger<DefinitionService>>();

        var service = new DefinitionService(
            ctx,
            mapper,
            tenantProvider.Object,
            eventPublisher.Object,
            graphValidation.Object,
            logger.Object,
            publishValidator.Object);

        return (service, ctx);
    }

    private DefinitionsController CreateController(DefinitionService service, WorkflowDbContext ctx, int tenantId = 1)
    {
        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);

        var graphValidator = new Mock<IWorkflowGraphValidator>();
        var logger = new Mock<ILogger<DefinitionsController>>();

        return new DefinitionsController(
            service,
            tenantProvider.Object,
            ctx,
            logger.Object,
            graphValidator.Object);
    }

    [Fact]
    public async Task GetAll_Default_ShouldExcludeArchivedDefinitions()
    {
        var (service, ctx) = CreateDefinitionService();
        await SeedAsync(ctx);
        var controller = CreateController(service, ctx);

        var result = await controller.GetAll(
            search: null,
            published: null,
            includeArchived: false,
            page: 1,
            pageSize: 50,
            ct: CancellationToken.None);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var envelope = ok!.Value as ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>;
        envelope.Should().NotBeNull();
        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().NotBeNull();

        var items = envelope.Data!.Items;
        var names = items.Select(d => d.Name).ToList();
        names.Should().Contain("ActiveDraft");
        names.Should().Contain("ActivePublished");
        names.Should().NotContain("ArchivedDraft");
        names.Should().NotContain("ArchivedPublished");
    }

    [Fact]
    public async Task GetAll_IncludeArchived_ShouldReturnAll()
    {
        var (service, ctx) = CreateDefinitionService();
        await SeedAsync(ctx);
        var controller = CreateController(service, ctx);

        var result = await controller.GetAll(
            search: null,
            published: null,
            includeArchived: true,
            page: 1,
            pageSize: 50,
            ct: CancellationToken.None);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var envelope = ok!.Value as ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>;
        envelope.Should().NotBeNull();
        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().NotBeNull();
        envelope.Data!.Items.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetAll_Paging_RespectsPageSize_AfterFiltering()
    {
        var (service, ctx) = CreateDefinitionService();
        await SeedAsync(ctx);
        var controller = CreateController(service, ctx);

        var result = await controller.GetAll(
            search: null,
            published: null,
            includeArchived: false,
            page: 1,
            pageSize: 1,
            ct: CancellationToken.None);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var envelope = ok!.Value as ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>;
        envelope.Should().NotBeNull();
        envelope!.Data.Should().NotBeNull();
        envelope.Data!.Items.Should().HaveCount(1);
        envelope.Data!.Items.First().IsArchived.Should().BeFalse();
    }
}
