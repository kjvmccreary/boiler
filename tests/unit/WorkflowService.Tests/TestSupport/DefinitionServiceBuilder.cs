using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Persistence;
using Contracts.Services;
using WorkflowService.Engine.Validation;
using WorkflowService.Domain.Models;

namespace WorkflowService.Tests.TestSupport;

public class DefinitionServiceBuilder
{
    private readonly WorkflowDbContext _ctx;
    private readonly FakeEventPublisher _publisher = new();
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly IGraphValidationService _graph = new FakeGraphValidationService();
    private readonly IWorkflowPublishValidator _publishValidator = new FakeWorkflowPublishValidator();
    private readonly IMapper _mapper;
    private readonly ILogger<DefinitionService> _logger;
    private int _tenantId = 1;

    public DefinitionServiceBuilder(string dbName)
    {
        _ctx = TestDbContextFactory.Create(dbName);
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(() => _tenantId);

        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<WorkflowDefinition, DTOs.Workflow.WorkflowDefinitionDto>();
        });
        _mapper = cfg.CreateMapper();
        _logger = LoggerFactory.Create(b => b.AddConsole().SetMinimumLevel(LogLevel.Debug))
            .CreateLogger<DefinitionService>();
    }

    public DefinitionServiceBuilder WithTenant(int tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public DefinitionService Build() =>
        new(_ctx, _mapper, _tenant.Object, _publisher, _graph, _logger, _publishValidator);

    public WorkflowDbContext Context => _ctx;
    public FakeEventPublisher Publisher => _publisher;
    public int CurrentTenant => _tenantId;
}
