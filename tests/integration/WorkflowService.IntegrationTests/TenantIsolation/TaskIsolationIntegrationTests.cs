using System.Net;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Persistence;
using WorkflowService.IntegrationTests.Infrastructure;
using DTOs.Workflow.Enums;

namespace WorkflowService.IntegrationTests.TenantIsolation;

[Collection("WorkflowServiceIntegration")]
public class TaskIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public TaskIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private TenantTestClient C() => new(_fx.CreateClient(), _fx.GetTenantTokenAsync);

    [Fact]
    public async Task T5_TaskList_Should_Return_Only_Tenant_Tasks()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var (d1, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var i1 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d1, 1);
        var i2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        await MultiTenantWorkflowSeeder.SeedTaskAsync(db, i1, 1, "T1-A");
        await MultiTenantWorkflowSeeder.SeedTaskAsync(db, i2, 2, "T2-A");

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync("/api/workflow/tasks");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await resp.Content.ReadAsStringAsync();
        json.Should().Contain("T1-A");
        json.Should().NotContain("T2-A");
    }

    [Fact]
    public async Task T5_MyTasks_Should_Not_Include_AnotherTenant()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var (d1, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var i1 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d1, 1);
        var i2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        // Assign user 1 a tenant 1 task, user 1 should not see tenant2
        await MultiTenantWorkflowSeeder.SeedTaskAsync(db, i1, 1, "Mine", TaskStatus.Assigned, assignedUserId: 1);
        await MultiTenantWorkflowSeeder.SeedTaskAsync(db, i2, 2, "Other", TaskStatus.Assigned, assignedUserId: 999);

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync("/api/workflow/tasks/my");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("Mine");
        body.Should().NotContain("Other");
    }

    [Fact]
    public async Task T6_ClaimTask_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var (d1, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var i2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);
        var t2 = await MultiTenantWorkflowSeeder.SeedTaskAsync(db, i2, 2, "ForeignTask");

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.PostJsonAsync($"/api/workflow/tasks/{t2.Id}/claim", new { });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task T6_CompleteTask_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var i2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);
        var t2 = await MultiTenantWorkflowSeeder.SeedTaskAsync(db, i2, 2, "ForeignTask");

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.PostJsonAsync($"/api/workflow/tasks/{t2.Id}/complete",
            new { completionData = "{}", completionNotes = "done" });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task T6_CancelTask_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var i2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);
        var t2 = await MultiTenantWorkflowSeeder.SeedTaskAsync(db, i2, 2, "ForeignTask");

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.PostJsonAsync($"/api/workflow/tasks/{t2.Id}/cancel", new { reason = "x" });
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
