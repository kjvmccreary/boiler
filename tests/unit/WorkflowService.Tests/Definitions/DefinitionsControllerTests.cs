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
using WorkflowService.Services.Validation; // <-- ensure this matches actual namespace
using DTOs.Workflow;
using DTOs.Common;
using Contracts.Services;
using WorkflowService.Engine.Validation;
using Xunit;

namespace WorkflowService.Tests.Definitions;

public class DefinitionsControllerTests
{
    private WorkflowDbContext CreateContext(ITenantProvider tenantProvider)
    {
        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase($"defs-ctrl-{Guid.NewGuid():N}")
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

    private async Task SeedAsync(WorkflowDbContext ctx, int tenantId = 1)
    {
        var now = DateTime.UtcNow;
        ctx.WorkflowDefinitions.AddRange(
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "A1",
                Version = 1,
                JSONDefinition = "{}",
                IsArchived = false,
                CreatedAt = now,
                UpdatedAt = now
            },
            new WorkflowDefinition
            {
                TenantId = tenantId,
                Name = "A2-Archived",
                Version = 1,
                JSONDefinition = "{}",
                IsArchived = true,
                ArchivedAt = now.AddMinutes(-1),
                CreatedAt = now,
                UpdatedAt = now
            }
        );
        await ctx.SaveChangesAsync();
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
    public async Task GetAll_Default_ShouldExcludeArchived()
    {
        var svc = CreateService(out var ctx);
        await SeedAsync(ctx);
        var controller = CreateController(svc, ctx);

        var result = await controller.GetAll(search: null, published: null, includeArchived: false, page: 1, pageSize: 50, ct: CancellationToken.None);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var envelope = ok!.Value as ApiResponseDto<System.Collections.Generic.List<WorkflowDefinitionDto>>;
        envelope.Should().NotBeNull();
        var names = envelope!.Data.Select(d => d.Name).ToList();
        names.Should().Contain("A1");
        names.Should().NotContain("A2-Archived");
    }

    [Fact]
    public async Task GetAll_IncludeArchived_ShouldReturnArchived()
    {
        var svc = CreateService(out var ctx);
        await SeedAsync(ctx);
        var controller = CreateController(svc, ctx);

        var result = await controller.GetAll(search: null, published: null, includeArchived: true, page: 1, pageSize: 50, ct: CancellationToken.None);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();

        var envelope = ok!.Value as ApiResponseDto<System.Collections.Generic.List<WorkflowDefinitionDto>>;
        envelope.Should().NotBeNull();
        envelope!.Data.Should().HaveCount(2);
    }
}
