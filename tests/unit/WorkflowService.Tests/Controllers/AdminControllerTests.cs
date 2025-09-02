using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using DTOs.Workflow;
using DTOs.Common;
using Microsoft.Extensions.Logging;
using WFTaskStatus = DTOs.Workflow.Enums.TaskStatus; // ✅ Alias to disambiguate from System.Threading.Tasks.TaskStatus

namespace WorkflowService.Tests.Controllers;

public class AdminControllerTests : TestBase
{
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _controller = new AdminController(DbContext, CreateMockLogger<AdminController>().Object);
        SetUser(_controller);
    }

    private static void SetUser(ControllerBase c)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,"1"),
            new Claim("permission","workflow.admin")
        };
        c.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims,"TestAuth"))
            }
        };
    }

    private (WorkflowDefinition def, WorkflowInstance inst) SeedInstance(InstanceStatus status)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "WF",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();

        var inst = new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = 1,
            Status = status,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(inst);
        DbContext.SaveChanges();

        return (def, inst);
    }

    private WorkflowTask SeedTask(WorkflowInstance inst, WFTaskStatus status = WFTaskStatus.Created)
    {
        var t = new WorkflowTask
        {
            TenantId = inst.TenantId,
            WorkflowInstanceId = inst.Id,
            NodeId = "n1",
            TaskName = "Task",
            NodeType = "human",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowTasks.Add(t);
        DbContext.SaveChanges();
        return t;
    }

    [Fact]
    public async Task GetWorkflowStats_ShouldReturnOk()
    {
        var result = await _controller.GetWorkflowStats();
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task RetryInstance_Success()
    {
        var (_, inst) = SeedInstance(InstanceStatus.Failed);
        var dto = new RetryInstanceRequestDto { RetryReason = "Test" };

        var result = await _controller.RetryInstance(inst.Id, dto);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task RetryInstance_WrongStatus_ShouldReturnBadRequest()
    {
        var (_, inst) = SeedInstance(InstanceStatus.Running);
        var dto = new RetryInstanceRequestDto { RetryReason = "Test" };

        var result = await _controller.RetryInstance(inst.Id, dto);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task MoveInstanceToNode_Success()
    {
        var (_, inst) = SeedInstance(InstanceStatus.Running);
        var dto = new MoveToNodeRequestDto { TargetNodeId = "newNode", Reason = "Adjust" };

        var result = await _controller.MoveInstanceToNode(inst.Id, dto);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task MoveInstanceToNode_Completed_ShouldReturnBadRequest()
    {
        var (_, inst) = SeedInstance(InstanceStatus.Completed);
        var dto = new MoveToNodeRequestDto { TargetNodeId = "newNode" };

        var result = await _controller.MoveInstanceToNode(inst.Id, dto);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ResetTask_Success()
    {
        var (_, inst) = SeedInstance(InstanceStatus.Running);
        var task = SeedTask(inst, WFTaskStatus.Assigned);

        var dto = new ResetTaskRequestDto { NewStatus = WFTaskStatus.Created, Reason = "Reset" };

        var result = await _controller.ResetTask(task.Id, dto);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task ResetTask_NotFound_ShouldReturnNotFound()
    {
        var result = await _controller.ResetTask(999, new ResetTaskRequestDto { NewStatus = WFTaskStatus.Created });
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetWorkflowEvents_Empty_ShouldReturnOk()
    {
        var result = await _controller.GetWorkflowEvents();
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task BulkCancelInstances_Success()
    {
        var (_, instRunning) = SeedInstance(InstanceStatus.Running);
        var dto = new BulkCancelInstancesRequestDto { Reason = "Ops", WorkflowDefinitionId = instRunning.WorkflowDefinitionId };

        var result = await _controller.BulkCancelInstances(dto);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task BulkCancelInstances_NoMatches_ShouldReturnBadRequest()
    {
        var dto = new BulkCancelInstancesRequestDto { Reason = "None", WorkflowDefinitionId = 9999 };
        var result = await _controller.BulkCancelInstances(dto);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
