using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Persistence;
using Contracts.Services;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;

namespace WorkflowService.Tests.TestSupport;

public class InstanceServiceBuilder
{
    private readonly WorkflowDbContext _ctx;
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IWorkflowRuntime> _runtime = new();
    private readonly Mock<IEventPublisher> _events = new();
    private readonly IMapper _mapper;
    private readonly ILogger<InstanceService> _logger;

    public InstanceServiceBuilder(string dbName, int tenantId)
    {
        _ctx = TenantFilteredServiceContextFactory.Create(dbName, tenantId);
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);

        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<WorkflowInstance, DTOs.Workflow.WorkflowInstanceDto>()
                .ForMember(d => d.WorkflowDefinitionName, m => m.MapFrom(s => s.WorkflowDefinition.Name));
            c.CreateMap<WorkflowDefinition, DTOs.Workflow.WorkflowDefinitionDto>();
            c.CreateMap<WorkflowEvent, DTOs.Workflow.WorkflowEventDto>();
        });
        _mapper = cfg.CreateMapper();
        _logger = LoggerFactory.Create(b => b.AddDebug()).CreateLogger<InstanceService>();
    }

    public IInstanceService Build() =>
        new InstanceService(_ctx, _mapper, _tenant.Object, _runtime.Object, _events.Object, _logger);

    public WorkflowDbContext Context => _ctx;

    public InstanceServiceBuilder SeedDefinition(int tenantId, string name, bool published = true)
    {
        // Only add if context tenant matches to avoid filter hiding issues
        var ctxTenant = _tenant.Object.GetCurrentTenantIdAsync().Result;
        if (ctxTenant != tenantId) return this;

        _ctx.WorkflowDefinitions.Add(new WorkflowDefinition
        {
            TenantId = tenantId,
            Name = name,
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[{"id":"start","type":"start"},{"id":"end","type":"end"}],
              "edges":[{"id":"e1","from":"start","to":"end"}]
            }
            """,
            IsPublished = published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = published ? DateTime.UtcNow : null
        });
        _ctx.SaveChanges();
        return this;
    }

    public InstanceServiceBuilder SeedInstance(int tenantId, string defName, InstanceStatus status = InstanceStatus.Running)
    {
        var ctxTenant = _tenant.Object.GetCurrentTenantIdAsync().Result;
        if (ctxTenant != tenantId) return this; // avoid failing queries due to filter

        var def = _ctx.WorkflowDefinitions.First(d => d.TenantId == tenantId && d.Name == defName);
        _ctx.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = tenantId,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = status,
            CurrentNodeIds = "[\"start\"]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        _ctx.SaveChanges();
        return this;
    }

    public int GetDefinitionId(string name, int tenantId) =>
        _ctx.WorkflowDefinitions.First(d => d.TenantId == tenantId && d.Name == name).Id;

    public int GetForeignInstanceId(int tenantId) =>
        _ctx.WorkflowInstances.First(i => i.TenantId == tenantId).Id;
}
