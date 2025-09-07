using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Persistence;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Domain.Models;
using DTOs.Workflow;
using DTOs.Common;
using Contracts.Services;
using WorkflowService.Engine.Validation;
using Xunit;

namespace WorkflowService.Tests.Definitions;

public class DefinitionServiceTests
{
    private static WorkflowDbContext CreateContext(ITenantProvider tenantProvider)
    {
        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase($"defs-db-{Guid.NewGuid():N}")
            .EnableSensitiveDataLogging()
            .Options;

        var httpAccessor = new Mock<IHttpContextAccessor>();
        var logger = new Mock<ILogger<WorkflowDbContext>>();

        return new WorkflowDbContext(options, httpAccessor.Object, tenantProvider, logger.Object);
    }

    private static IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        });
        return cfg.CreateMapper();
    }

    private DefinitionService CreateService(out WorkflowDbContext ctx, int tenantId = 1)
    {
        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);

        ctx = CreateContext(tenantProvider.Object);

        var mapper = CreateMapper();
        var eventPublisher = new Mock<IEventPublisher>();
        var graphValidation = new Mock<IGraphValidationService>();
        var publishValidator = new Mock<IWorkflowPublishValidator>();
        var logger = new Mock<ILogger<DefinitionService>>();

        return new DefinitionService(
            ctx,
            mapper,
            tenantProvider.Object,
            eventPublisher.Object,
            graphValidation.Object,
            logger.Object,
            publishValidator.Object);
    }

    private async Task SeedAsync(WorkflowDbContext ctx, int tenantId = 1)
    {
        var now = DateTime.UtcNow;

        ctx.WorkflowDefinitions.AddRange(
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "Active One",
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
                Name = "Archived One",
                Version = 1,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = false,
                IsArchived = true,
                ArchivedAt = now.AddMinutes(-5),
                CreatedAt = now,
                UpdatedAt = now
            },
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "Published Active",
                Version = 2,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = true,
                PublishedAt = now.AddMinutes(-10),
                IsArchived = false,
                CreatedAt = now,
                UpdatedAt = now
            },
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "Published Archived",
                Version = 3,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = true,
                PublishedAt = now.AddMinutes(-20),
                IsArchived = true,
                ArchivedAt = now.AddMinutes(-2),
                CreatedAt = now,
                UpdatedAt = now
            }
        );

        await ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_Default_ExcludesArchived()
    {
        var svc = CreateService(out var ctx);
        await SeedAsync(ctx);
        var req = new GetWorkflowDefinitionsRequestDto
        {
            IncludeArchived = false,
            Page = 1,
            PageSize = 50
        };

        var resp = await svc.GetAllAsync(req, CancellationToken.None);
        resp.Success.Should().BeTrue();
        var names = resp.Data!.Items.Select(i => i.Name).ToList();
        names.Should().Contain("Active One");
        names.Should().Contain("Published Active");
        names.Should().NotContain("Archived One");
        names.Should().NotContain("Published Archived");
    }

    [Fact]
    public async Task GetAllAsync_IncludeArchived_ReturnsAll()
    {
        var svc = CreateService(out var ctx);
        await SeedAsync(ctx);
        var req = new GetWorkflowDefinitionsRequestDto
        {
            IncludeArchived = true,
            Page = 1,
            PageSize = 50
        };

        var resp = await svc.GetAllAsync(req, CancellationToken.None);
        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().HaveCount(4);
        resp.Data.Items.Any(d => d.IsArchived).Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_Paging_RespectsPageSize()
    {
        var svc = CreateService(out var ctx);
        await SeedAsync(ctx);
        var req = new GetWorkflowDefinitionsRequestDto
        {
            IncludeArchived = true,
            Page = 1,
            PageSize = 2
        };

        var resp = await svc.GetAllAsync(req, CancellationToken.None);
        resp.Data!.Items.Should().HaveCount(2);
        resp.Data.TotalCount.Should().Be(4);
    }
}
