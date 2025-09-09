using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Controllers;
using WorkflowService.Engine;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using WorkflowService.Services.Validation;
using Xunit;
using Contracts.Services;
using DTOs.Common;
using DTOs.Workflow;

namespace WorkflowService.Tests.Controllers;

public class DefinitionsControllerTagFilterTests
{
    private DefinitionsController CreateController()
    {
        var defs = new Mock<IDefinitionService>();
        defs.Setup(d => d.GetAllAsync(It.IsAny<GetWorkflowDefinitionsRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.SuccessResult(
                new PagedResultDto<WorkflowDefinitionDto>
                {
                    Items = new(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 20
                }));

        var tenant = new Mock<ITenantProvider>();
        
        // DbContext dependencies
        var httpAccessor = new Mock<IHttpContextAccessor>();
        httpAccessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());
        var dbLogger = new Mock<ILogger<WorkflowDbContext>>();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new WorkflowDbContext(options, httpAccessor.Object, tenant.Object, dbLogger.Object);
 
        var logger = new Mock<ILogger<DefinitionsController>>();
        var graphValidator = new Mock<IWorkflowGraphValidator>();

        var controller = new DefinitionsController(
            defs.Object,
            tenant.Object,
            ctx,
            logger.Object,
            graphValidator.Object);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    [Fact]
    public async Task GetAll_InvalidAnyTags_Returns422()
    {
        var controller = CreateController();
        var tooMany = string.Join(",", Enumerable.Range(1, 20).Select(i => $"T{i}"));

        var result = await controller.GetAll(anyTags: tooMany);

        var objectResult = result.Result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task GetAll_ValidFilters_PassesThrough()
    {
        var controller = CreateController();
        var result = await controller.GetAll(anyTags: "Alpha,Beta", allTags: "Core,Platform");
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(StatusCodes.Status200OK);
    }
}
