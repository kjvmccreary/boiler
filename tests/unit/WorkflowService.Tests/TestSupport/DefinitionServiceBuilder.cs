using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Persistence;
using Contracts.Services;
using WorkflowService.Engine.Validation;
using WorkflowService.Domain.Models;
using Xunit.Abstractions;

namespace WorkflowService.Tests.TestSupport;

public class DefinitionServiceBuilder
{
    private readonly string _dbName;
    private WorkflowDbContext _ctx;
    private readonly FakeEventPublisher _publisher = new();
    private readonly Mock<ITenantProvider> _tenant = new();
    private IGraphValidationService _graph = new FakeGraphValidationService();
    private readonly IWorkflowPublishValidator _publishValidator = new FakeWorkflowPublishValidator();
    private readonly IMapper _mapper;
    private ILoggerFactory _loggerFactory;
    private ILogger<DefinitionService> _logger;
    private int _tenantId = 1;

    public DefinitionServiceBuilder(string dbName)
    {
        _dbName = dbName;
        _ctx = TestDbContextFactory.Create(dbName, _tenantId);

        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(() => _tenantId);

        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<WorkflowDefinition, DTOs.Workflow.WorkflowDefinitionDto>();
        });
        _mapper = cfg.CreateMapper();

        _loggerFactory = LoggerFactory.Create(b => b.AddConsole().SetMinimumLevel(LogLevel.Debug));
        _logger = _loggerFactory.CreateLogger<DefinitionService>();
    }

    public DefinitionServiceBuilder WithTenant(int tenantId)
    {
        _tenantId = tenantId;
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(() => _tenantId);
        _ctx = TestDbContextFactory.Create(_dbName, _tenantId);
        return this;
    }

    public DefinitionServiceBuilder WithOutput(ITestOutputHelper output)
    {
        _loggerFactory.Dispose();
        _loggerFactory = LoggerFactory.Create(b =>
        {
            b.ClearProviders();
            b.SetMinimumLevel(LogLevel.Debug);
            b.AddProvider(new XunitOutputLoggerProvider(output));
        });
        _logger = _loggerFactory.CreateLogger<DefinitionService>();
        return this;
    }

    public DefinitionService Build() =>
        new(_ctx, _mapper, _tenant.Object, _publisher, _graph, _logger, _publishValidator);

    public WorkflowDbContext Context => _ctx;
    public FakeEventPublisher Publisher => _publisher;
    public int CurrentTenant => _tenantId;
}
