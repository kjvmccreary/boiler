# Workflow Engine Development Plan
## Visual Studio + GitHub Copilot Implementation Guide

---

## Pre-Implementation Setup

### Required NuGet Packages to Add
```xml
<!-- Add to relevant .csproj files -->
<PackageReference Include="Elsa" Version="2.14.1" /> <!-- Optional: Workflow engine library -->
<PackageReference Include="Stateless" Version="5.15.0" /> <!-- State machine library -->
<PackageReference Include="Azure.AI.OpenAI" Version="1.0.0-beta.12" />
<PackageReference Include="Anthropic.SDK" Version="3.0.0" />
<PackageReference Include="Hangfire.AspNetCore" Version="1.8.6" /> <!-- Background jobs -->
<PackageReference Include="Hangfire.PostgreSql" Version="1.20.4" />
<PackageReference Include="JsonPath.Net" Version="0.6.0" /> <!-- JSON manipulation -->
<PackageReference Include="FluentValidation" Version="11.8.0" />
```

### Project Structure to Create
```
src/
├── services/
│   └── WorkflowService/           # New microservice
│       ├── WorkflowService.Api/
│       ├── WorkflowService.Core/
│       ├── WorkflowService.Infrastructure/
│       └── WorkflowService.Tests/
├── shared/
│   ├── DTOs/
│   │   └── Workflow/              # New folder
│   └── Common/
│       └── Workflow/              # New folder
```

---

## Phase 1: Database Schema & Entities (Week 1)

### Task 1.1: Create Workflow Entities

**File**: `src/shared/Common/Workflow/Entities/WorkflowEntities.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Workflow.Entities;

/// <summary>
/// Workflow template that defines the structure and rules of a workflow
/// Copilot: Generate constructor, validation methods, and factory methods
/// </summary>
public class WorkflowTemplate : BaseEntity
{
    [Required]
    public Guid TenantId { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    [Required, MaxLength(50)]
    public string Category { get; set; } // Contract, Approval, Onboarding, etc.
    
    public int Version { get; set; } = 1;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false;
    
    /// <summary>
    /// JSON schema defining the workflow structure
    /// Copilot: Add JSON serialization attributes and validation
    /// </summary>
    public string Definition { get; set; } // JSON workflow definition
    
    /// <summary>
    /// Variables that can be used in this workflow
    /// </summary>
    public string Variables { get; set; } // JSON schema of workflow variables
    
    // Navigation properties
    public virtual ICollection<WorkflowStep> Steps { get; set; }
    public virtual ICollection<WorkflowInstance> Instances { get; set; }
    
    // TODO: Add methods for version management
    // TODO: Add validation for Definition JSON schema
    // TODO: Add method to clone template for new version
}

/// <summary>
/// Individual step within a workflow template
/// Copilot: Generate builder pattern for complex step creation
/// </summary>
public class WorkflowStep : BaseEntity
{
    [Required]
    public Guid WorkflowTemplateId { get; set; }
    
    [Required, MaxLength(100)]
    public string StepIdentifier { get; set; } // Unique within workflow
    
    [Required, MaxLength(200)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    [Required]
    public StepType Type { get; set; }
    
    public int Order { get; set; }
    
    /// <summary>
    /// Permission required to execute this step
    /// Links to RBAC system - null means no permission required
    /// </summary>
    [MaxLength(100)]
    public string RequiredPermission { get; set; }
    
    /// <summary>
    /// Configuration specific to step type (JSON)
    /// For AI steps: includes prompts, model config
    /// For Human steps: includes form schema
    /// Copilot: Add type-specific configuration classes
    /// </summary>
    public string Configuration { get; set; }
    
    /// <summary>
    /// AI-specific configuration if this is an AI step
    /// </summary>
    public string AIConfiguration { get; set; }
    
    /// <summary>
    /// Retry policy configuration (JSON)
    /// </summary>
    public string RetryPolicy { get; set; }
    
    public int? TimeoutMinutes { get; set; }
    
    // Navigation properties
    public virtual WorkflowTemplate WorkflowTemplate { get; set; }
    public virtual ICollection<WorkflowTransition> OutgoingTransitions { get; set; }
    public virtual ICollection<WorkflowTransition> IncomingTransitions { get; set; }
    
    // TODO: Add method to validate configuration based on StepType
    // TODO: Add method to parse AI configuration
    // TODO: Add timeout handling logic
}

/// <summary>
/// Defines transitions between workflow steps
/// Copilot: Generate condition evaluation logic
/// </summary>
public class WorkflowTransition : BaseEntity
{
    [Required]
    public Guid FromStepId { get; set; }
    
    [Required]
    public Guid ToStepId { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; }
    
    /// <summary>
    /// Condition that must be met for this transition (JSON expression)
    /// Examples: "result == 'approved'", "context.amount > 10000"
    /// </summary>
    public string Condition { get; set; }
    
    public int Priority { get; set; } = 0; // For multiple transitions
    
    public bool IsDefault { get; set; } = false;
    
    // Navigation properties
    public virtual WorkflowStep FromStep { get; set; }
    public virtual WorkflowStep ToStep { get; set; }
    
    // TODO: Add condition evaluation method
    // TODO: Add validation for condition syntax
}

/// <summary>
/// Runtime instance of a workflow
/// Copilot: Generate state machine methods
/// </summary>
public class WorkflowInstance : BaseEntity
{
    [Required]
    public Guid WorkflowTemplateId { get; set; }
    
    [Required]
    public Guid TenantId { get; set; }
    
    [Required, MaxLength(100)]
    public string InstanceIdentifier { get; set; } // Unique identifier
    
    [Required]
    public WorkflowStatus Status { get; set; }
    
    public Guid? CurrentStepId { get; set; }
    
    /// <summary>
    /// Runtime context data (JSON)
    /// Stores all workflow variables and their current values
    /// </summary>
    public string Context { get; set; }
    
    /// <summary>
    /// History of all state transitions (JSON array)
    /// </summary>
    public string History { get; set; }
    
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    public string InitiatedBy { get; set; } // User ID or system
    
    // Navigation properties
    public virtual WorkflowTemplate WorkflowTemplate { get; set; }
    public virtual WorkflowStep CurrentStep { get; set; }
    public virtual ICollection<WorkflowTask> Tasks { get; set; }
    public virtual ICollection<WorkflowEvent> Events { get; set; }
    
    // TODO: Add method to get next possible steps
    // TODO: Add method to update context safely
    // TODO: Add method to record history entry
}

/// <summary>
/// Human tasks generated by workflow steps
/// Copilot: Generate task assignment logic
/// </summary>
public class WorkflowTask : BaseEntity
{
    [Required]
    public Guid WorkflowInstanceId { get; set; }
    
    [Required]
    public Guid WorkflowStepId { get; set; }
    
    [Required, MaxLength(200)]
    public string Title { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
    
    [Required]
    public TaskStatus Status { get; set; }
    
    public Guid? AssignedToUserId { get; set; }
    public Guid? AssignedToRoleId { get; set; }
    
    [MaxLength(100)]
    public string AssignedToGroup { get; set; } // For group assignments
    
    public DateTime? DueDate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Form data or task result (JSON)
    /// </summary>
    public string Data { get; set; }
    
    /// <summary>
    /// AI assistance data if applicable (JSON)
    /// </summary>
    public string AIAssistanceData { get; set; }
    
    public int Priority { get; set; } = 0; // 0=Normal, 1=High, 2=Critical
    
    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; set; }
    public virtual WorkflowStep WorkflowStep { get; set; }
    
    // TODO: Add method for task assignment logic
    // TODO: Add method for escalation
    // TODO: Add method to merge AI assistance data
}

/// <summary>
/// Audit trail of workflow events
/// Copilot: Generate event factory methods for different event types
/// </summary>
public class WorkflowEvent : BaseEntity
{
    [Required]
    public Guid WorkflowInstanceId { get; set; }
    
    public Guid? WorkflowStepId { get; set; }
    
    [Required]
    public WorkflowEventType EventType { get; set; }
    
    [Required, MaxLength(200)]
    public string EventName { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
    
    /// <summary>
    /// Event data payload (JSON)
    /// </summary>
    public string Data { get; set; }
    
    [MaxLength(100)]
    public string TriggeredBy { get; set; } // User ID or "system"
    
    public DateTime OccurredAt { get; set; }
    
    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; set; }
    public virtual WorkflowStep WorkflowStep { get; set; }
    
    // TODO: Add factory methods for common events
}

/// <summary>
/// AI processing history and results
/// Copilot: Generate methods for cost calculation and performance metrics
/// </summary>
public class WorkflowAIExecution : BaseEntity
{
    [Required]
    public Guid WorkflowInstanceId { get; set; }
    
    [Required]
    public Guid WorkflowStepId { get; set; }
    
    [Required, MaxLength(50)]
    public string Provider { get; set; } // OpenAI, Anthropic, Azure
    
    [Required, MaxLength(100)]
    public string Model { get; set; } // gpt-4, claude-3, etc.
    
    public string Prompt { get; set; }
    public string Response { get; set; }
    
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal Cost { get; set; }
    
    public int LatencyMs { get; set; }
    
    public decimal? ConfidenceScore { get; set; }
    
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    
    public DateTime ExecutedAt { get; set; }
    
    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; set; }
    public virtual WorkflowStep WorkflowStep { get; set; }
    
    // TODO: Add method to calculate cost based on token usage
    // TODO: Add method to extract structured data from response
}
```

### Task 1.2: Create Enums

**File**: `src/shared/Common/Workflow/Enums/WorkflowEnums.cs`

```csharp
namespace Common.Workflow.Enums;

/// <summary>
/// Types of workflow steps
/// Copilot: Generate extension methods for step type behaviors
/// </summary>
public enum StepType
{
    Manual,           // Human task
    Approval,         // Human approval decision
    Automatic,        // System action (API call, calculation)
    AIProcessing,     // AI analysis/generation
    AIReview,         // AI evaluation with human override
    AIDecision,       // AI-powered routing
    DocumentGeneration, // Generate documents from templates
    DataEnrichment,   // Enhance data via AI or APIs
    Notification,     // Send notifications
    Integration,      // External system call
    Parallel,         // Fork parallel branches
    Join,            // Wait for parallel branches
    Timer,           // Wait/delay
    Conditional,     // Branch based on conditions
    Loop,            // Repeat steps
    SubWorkflow      // Execute another workflow
}

public enum WorkflowStatus
{
    Draft,
    Active,
    Paused,
    Completed,
    Failed,
    Cancelled,
    Terminated
}

public enum TaskStatus
{
    Pending,
    Assigned,
    InProgress,
    Completed,
    Cancelled,
    Escalated,
    Expired
}

public enum WorkflowEventType
{
    Started,
    StepStarted,
    StepCompleted,
    StepFailed,
    StepSkipped,
    TaskCreated,
    TaskAssigned,
    TaskCompleted,
    TransitionExecuted,
    VariableUpdated,
    Paused,
    Resumed,
    Completed,
    Failed,
    Cancelled,
    AIExecutionStarted,
    AIExecutionCompleted,
    AIExecutionFailed,
    EscalationTriggered,
    NotificationSent,
    IntegrationCalled,
    Error,
    Warning,
    Info
}

// TODO: Add extension methods for enum behaviors
// TODO: Add display name attributes for UI
```

### Task 1.3: Create DbContext Configuration

**File**: `src/services/WorkflowService/WorkflowService.Infrastructure/Data/WorkflowDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Common.Workflow.Entities;

namespace WorkflowService.Infrastructure.Data;

/// <summary>
/// DbContext for workflow service
/// Copilot: Generate fluent API configurations for all entities
/// </summary>
public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<WorkflowTemplate> WorkflowTemplates { get; set; }
    public DbSet<WorkflowStep> WorkflowSteps { get; set; }
    public DbSet<WorkflowTransition> WorkflowTransitions { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<WorkflowTask> WorkflowTasks { get; set; }
    public DbSet<WorkflowEvent> WorkflowEvents { get; set; }
    public DbSet<WorkflowAIExecution> WorkflowAIExecutions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // WorkflowTemplate configuration
        modelBuilder.Entity<WorkflowTemplate>(entity =>
        {
            entity.ToTable("workflow_templates");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Definition).HasColumnType("jsonb");
            entity.Property(e => e.Variables).HasColumnType("jsonb");
            
            entity.HasIndex(e => new { e.TenantId, e.Name, e.Version }).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
            
            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
            
            // TODO: Add RLS policy for tenant isolation
        });
        
        // WorkflowStep configuration
        modelBuilder.Entity<WorkflowStep>(entity =>
        {
            entity.ToTable("workflow_steps");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.StepIdentifier).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Configuration).HasColumnType("jsonb");
            entity.Property(e => e.AIConfiguration).HasColumnType("jsonb");
            entity.Property(e => e.RetryPolicy).HasColumnType("jsonb");
            
            entity.HasIndex(e => new { e.WorkflowTemplateId, e.StepIdentifier }).IsUnique();
            entity.HasIndex(e => e.Type);
            
            entity.HasOne(e => e.WorkflowTemplate)
                .WithMany(t => t.Steps)
                .HasForeignKey(e => e.WorkflowTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // WorkflowTransition configuration
        modelBuilder.Entity<WorkflowTransition>(entity =>
        {
            entity.ToTable("workflow_transitions");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Condition).HasColumnType("jsonb");
            
            entity.HasIndex(e => new { e.FromStepId, e.Priority });
            
            entity.HasOne(e => e.FromStep)
                .WithMany(s => s.OutgoingTransitions)
                .HasForeignKey(e => e.FromStepId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.ToStep)
                .WithMany(s => s.IncomingTransitions)
                .HasForeignKey(e => e.ToStepId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // WorkflowInstance configuration
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.ToTable("workflow_instances");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.InstanceIdentifier).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Context).HasColumnType("jsonb");
            entity.Property(e => e.History).HasColumnType("jsonb");
            
            entity.HasIndex(e => new { e.TenantId, e.InstanceIdentifier }).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.StartedAt);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            
            entity.HasOne(e => e.WorkflowTemplate)
                .WithMany(t => t.Instances)
                .HasForeignKey(e => e.WorkflowTemplateId);
                
            entity.HasOne(e => e.CurrentStep)
                .WithMany()
                .HasForeignKey(e => e.CurrentStepId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // TODO: Add RLS policy for tenant isolation
        });
        
        // WorkflowTask configuration
        modelBuilder.Entity<WorkflowTask>(entity =>
        {
            entity.ToTable("workflow_tasks");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Data).HasColumnType("jsonb");
            entity.Property(e => e.AIAssistanceData).HasColumnType("jsonb");
            
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.AssignedToUserId);
            entity.HasIndex(e => e.AssignedToRoleId);
            entity.HasIndex(e => e.DueDate);
            entity.HasIndex(e => new { e.Status, e.DueDate });
            
            entity.HasOne(e => e.WorkflowInstance)
                .WithMany(i => i.Tasks)
                .HasForeignKey(e => e.WorkflowInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.WorkflowStep)
                .WithMany()
                .HasForeignKey(e => e.WorkflowStepId);
        });
        
        // WorkflowEvent configuration
        modelBuilder.Entity<WorkflowEvent>(entity =>
        {
            entity.ToTable("workflow_events");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.EventName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Data).HasColumnType("jsonb");
            
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.OccurredAt);
            entity.HasIndex(e => new { e.WorkflowInstanceId, e.OccurredAt });
            
            entity.HasOne(e => e.WorkflowInstance)
                .WithMany(i => i.Events)
                .HasForeignKey(e => e.WorkflowInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.WorkflowStep)
                .WithMany()
                .HasForeignKey(e => e.WorkflowStepId)
                .OnDelete(DeleteBehavior.SetNull);
        });
        
        // WorkflowAIExecution configuration
        modelBuilder.Entity<WorkflowAIExecution>(entity =>
        {
            entity.ToTable("workflow_ai_executions");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Prompt).HasColumnType("text");
            entity.Property(e => e.Response).HasColumnType("text");
            entity.Property(e => e.Cost).HasPrecision(10, 4);
            
            entity.HasIndex(e => e.ExecutedAt);
            entity.HasIndex(e => new { e.WorkflowInstanceId, e.ExecutedAt });
            
            entity.HasOne(e => e.WorkflowInstance)
                .WithMany()
                .HasForeignKey(e => e.WorkflowInstanceId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.WorkflowStep)
                .WithMany()
                .HasForeignKey(e => e.WorkflowStepId);
        });
    }
    
    // TODO: Override SaveChangesAsync to handle audit fields
    // TODO: Add method to apply tenant filter
    // TODO: Add seed data for common workflow templates
}
```

### Task 1.4: Create Initial Migration

```bash
# In Package Manager Console or CLI
cd src/services/WorkflowService/WorkflowService.Infrastructure
dotnet ef migrations add InitialWorkflowSchema -c WorkflowDbContext
dotnet ef database update
```

---

## Phase 2: Core Workflow Engine (Week 2)

### Task 2.1: Create Workflow Engine Service

**File**: `src/services/WorkflowService/WorkflowService.Core/Services/WorkflowEngine.cs`

```csharp
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stateless;

namespace WorkflowService.Core.Services;

/// <summary>
/// Core workflow execution engine using state machine pattern
/// Copilot: Implement state machine logic and transition handling
/// </summary>
public interface IWorkflowEngine
{
    Task<WorkflowInstance> StartWorkflowAsync(
        Guid templateId, 
        Guid tenantId, 
        string initiatedBy,
        Dictionary<string, object> initialContext);
        
    Task<WorkflowInstance> ExecuteStepAsync(
        Guid instanceId, 
        Dictionary<string, object> stepResult);
        
    Task<List<WorkflowTransition>> GetAvailableTransitionsAsync(Guid instanceId);
    
    Task<WorkflowInstance> TransitionToStepAsync(
        Guid instanceId, 
        string transitionName);
        
    Task<WorkflowInstance> PauseWorkflowAsync(Guid instanceId);
    Task<WorkflowInstance> ResumeWorkflowAsync(Guid instanceId);
    Task<WorkflowInstance> CancelWorkflowAsync(Guid instanceId, string reason);
}

public class WorkflowEngine : IWorkflowEngine
{
    private readonly ILogger<WorkflowEngine> _logger;
    private readonly WorkflowDbContext _context;
    private readonly IWorkflowTaskService _taskService;
    private readonly IWorkflowEventService _eventService;
    private readonly IStepExecutorFactory _stepExecutorFactory;
    private readonly IWorkflowContextService _contextService;
    
    public WorkflowEngine(
        ILogger<WorkflowEngine> logger,
        WorkflowDbContext context,
        IWorkflowTaskService taskService,
        IWorkflowEventService eventService,
        IStepExecutorFactory stepExecutorFactory,
        IWorkflowContextService contextService)
    {
        _logger = logger;
        _context = context;
        _taskService = taskService;
        _eventService = eventService;
        _stepExecutorFactory = stepExecutorFactory;
        _contextService = contextService;
    }
    
    public async Task<WorkflowInstance> StartWorkflowAsync(
        Guid templateId, 
        Guid tenantId, 
        string initiatedBy,
        Dictionary<string, object> initialContext)
    {
        // TODO: Validate template exists and is active
        // TODO: Check user has permission to start workflow
        // TODO: Create workflow instance
        // TODO: Initialize context with default values and initial context
        // TODO: Find and execute first step
        // TODO: Record workflow started event
        
        var template = await _context.WorkflowTemplates
            .Include(t => t.Steps)
            .ThenInclude(s => s.OutgoingTransitions)
            .FirstOrDefaultAsync(t => t.Id == templateId && t.TenantId == tenantId);
            
        if (template == null || !template.IsActive)
            throw new WorkflowException("Template not found or inactive");
            
        // Create instance
        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowTemplateId = templateId,
            TenantId = tenantId,
            InstanceIdentifier = GenerateInstanceIdentifier(),
            Status = WorkflowStatus.Active,
            StartedAt = DateTime.UtcNow,
            InitiatedBy = initiatedBy,
            Context = JsonSerializer.Serialize(initialContext)
        };
        
        // Find start step (first step by order)
        var startStep = template.Steps.OrderBy(s => s.Order).FirstOrDefault();
        if (startStep == null)
            throw new WorkflowException("No start step defined");
            
        instance.CurrentStepId = startStep.Id;
        
        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync();
        
        // Record event
        await _eventService.RecordEventAsync(instance.Id, WorkflowEventType.Started, 
            "Workflow started", new { initiatedBy, templateId });
            
        // Execute first step
        await ExecuteCurrentStepAsync(instance);
        
        return instance;
    }
    
    private async Task ExecuteCurrentStepAsync(WorkflowInstance instance)
    {
        var step = await _context.WorkflowSteps
            .FirstOrDefaultAsync(s => s.Id == instance.CurrentStepId);
            
        if (step == null) return;
        
        // Get appropriate executor based on step type
        var executor = _stepExecutorFactory.GetExecutor(step.Type);
        
        // Execute step
        var result = await executor.ExecuteAsync(instance, step);
        
        // Update context with result
        await _contextService.UpdateContextAsync(instance.Id, result.OutputVariables);
        
        // Handle based on step type
        if (step.Type == StepType.Manual || step.Type == StepType.Approval)
        {
            // Create task for human interaction
            await _taskService.CreateTaskAsync(instance, step);
        }
        else if (result.Success)
        {
            // Auto-transition for automatic steps
            await ProcessTransitionsAsync(instance, result);
        }
        
        // Record step execution event
        await _eventService.RecordEventAsync(instance.Id, WorkflowEventType.StepStarted,
            $"Step '{step.Name}' started", result);
    }
    
    private async Task ProcessTransitionsAsync(
        WorkflowInstance instance, 
        StepExecutionResult result)
    {
        var transitions = await _context.WorkflowTransitions
            .Where(t => t.FromStepId == instance.CurrentStepId)
            .OrderBy(t => t.Priority)
            .ToListAsync();
            
        foreach (var transition in transitions)
        {
            if (await EvaluateTransitionConditionAsync(transition, instance, result))
            {
                // Transition to next step
                instance.CurrentStepId = transition.ToStepId;
                await _context.SaveChangesAsync();
                
                // Record transition event
                await _eventService.RecordEventAsync(instance.Id, 
                    WorkflowEventType.TransitionExecuted,
                    $"Transitioned via '{transition.Name}'", 
                    new { fromStep = transition.FromStepId, toStep = transition.ToStepId });
                
                // Execute next step
                await ExecuteCurrentStepAsync(instance);
                break;
            }
        }
    }
    
    private async Task<bool> EvaluateTransitionConditionAsync(
        WorkflowTransition transition,
        WorkflowInstance instance,
        StepExecutionResult result)
    {
        if (transition.IsDefault) return true;
        if (string.IsNullOrEmpty(transition.Condition)) return true;
        
        // TODO: Implement condition evaluation using expression parser
        // TODO: Support JsonPath expressions
        // TODO: Support complex conditions
        
        // Placeholder - evaluate condition against context and result
        return true;
    }
    
    // TODO: Implement remaining interface methods
    // TODO: Add state machine configuration
    // TODO: Add parallel execution support
    // TODO: Add sub-workflow support
    // TODO: Add compensation/rollback logic
}
```

### Task 2.2: Create Step Executors

**File**: `src/services/WorkflowService/WorkflowService.Core/StepExecutors/StepExecutors.cs`

```csharp
namespace WorkflowService.Core.StepExecutors;

/// <summary>
/// Base interface for step executors
/// Copilot: Generate implementations for each step type
/// </summary>
public interface IStepExecutor
{
    StepType SupportedType { get; }
    Task<StepExecutionResult> ExecuteAsync(WorkflowInstance instance, WorkflowStep step);
}

public class StepExecutionResult
{
    public bool Success { get; set; }
    public string Output { get; set; }
    public Dictionary<string, object> OutputVariables { get; set; }
    public string ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}

/// <summary>
/// Factory to get appropriate step executor
/// </summary>
public interface IStepExecutorFactory
{
    IStepExecutor GetExecutor(StepType type);
}

public class StepExecutorFactory : IStepExecutorFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<StepType, Type> _executorMap;
    
    public StepExecutorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _executorMap = new Dictionary<StepType, Type>
        {
            { StepType.Manual, typeof(ManualStepExecutor) },
            { StepType.Automatic, typeof(AutomaticStepExecutor) },
            { StepType.AIProcessing, typeof(AIProcessingStepExecutor) },
            { StepType.DocumentGeneration, typeof(DocumentGenerationStepExecutor) },
            { StepType.Notification, typeof(NotificationStepExecutor) },
            // TODO: Add all step type mappings
        };
    }
    
    public IStepExecutor GetExecutor(StepType type)
    {
        if (!_executorMap.ContainsKey(type))
            throw new NotSupportedException($"No executor for step type: {type}");
            
        var executorType = _executorMap[type];
        return (IStepExecutor)_serviceProvider.GetService(executorType);
    }
}

/// <summary>
/// Executor for manual/human tasks
/// </summary>
public class ManualStepExecutor : IStepExecutor
{
    public StepType SupportedType => StepType.Manual;
    
    public async Task<StepExecutionResult> ExecuteAsync(
        WorkflowInstance instance, 
        WorkflowStep step)
    {
        // Manual steps create tasks and wait
        // The actual execution happens when the task is completed
        
        return new StepExecutionResult
        {
            Success = true,
            Output = "Task created for manual execution"
        };
    }
}

/// <summary>
/// Executor for automatic system actions
/// </summary>
public class AutomaticStepExecutor : IStepExecutor
{
    private readonly ILogger<AutomaticStepExecutor> _logger;
    
    public StepType SupportedType => StepType.Automatic;
    
    public async Task<StepExecutionResult> ExecuteAsync(
        WorkflowInstance instance, 
        WorkflowStep step)
    {
        try
        {
            // Parse configuration
            var config = JsonSerializer.Deserialize<AutomaticStepConfig>(step.Configuration);
            
            // TODO: Execute based on action type
            // TODO: Support API calls
            // TODO: Support database operations
            // TODO: Support calculations
            
            return new StepExecutionResult
            {
                Success = true,
                OutputVariables = new Dictionary<string, object>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute automatic step {StepId}", step.Id);
            return new StepExecutionResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

/// <summary>
/// Executor for AI processing steps
/// Copilot: Implement AI service integration
/// </summary>
public class AIProcessingStepExecutor : IStepExecutor
{
    private readonly IAIService _aiService;
    private readonly IWorkflowContextService _contextService;
    private readonly ILogger<AIProcessingStepExecutor> _logger;
    
    public StepType SupportedType => StepType.AIProcessing;
    
    public AIProcessingStepExecutor(
        IAIService aiService,
        IWorkflowContextService contextService,
        ILogger<AIProcessingStepExecutor> logger)
    {
        _aiService = aiService;
        _contextService = contextService;
        _logger = logger;
    }
    
    public async Task<StepExecutionResult> ExecuteAsync(
        WorkflowInstance instance, 
        WorkflowStep step)
    {
        try
        {
            // Parse AI configuration
            var aiConfig = JsonSerializer.Deserialize<AIStepConfiguration>(step.AIConfiguration);
            
            // Get context variables for prompt
            var context = await _contextService.GetContextAsync(instance.Id);
            
            // Build prompt from template and context
            var prompt = BuildPromptFromTemplate(aiConfig.PromptTemplate, context);
            
            // Execute AI call
            var aiResult = await _aiService.ProcessAsync(new AIRequest
            {
                Provider = aiConfig.ServiceType,
                Model = aiConfig.ModelId,
                Prompt = prompt,
                Parameters = aiConfig.Parameters
            });
            
            // Parse and map output
            var outputVariables = MapAIOutputToVariables(
                aiResult.Response, 
                aiConfig.OutputMapping);
            
            // Record AI execution for auditing
            await RecordAIExecutionAsync(instance.Id, step.Id, aiConfig, aiResult);
            
            return new StepExecutionResult
            {
                Success = aiResult.Success,
                Output = aiResult.Response,
                OutputVariables = outputVariables
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI processing failed for step {StepId}", step.Id);
            return new StepExecutionResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    
    private string BuildPromptFromTemplate(string template, Dictionary<string, object> context)
    {
        // TODO: Implement template variable replacement
        // TODO: Support nested object access
        // TODO: Support formatting options
        return template;
    }
    
    private Dictionary<string, object> MapAIOutputToVariables(
        string aiResponse, 
        Dictionary<string, string> mapping)
    {
        // TODO: Parse AI response (JSON, text, etc.)
        // TODO: Extract values based on mapping
        // TODO: Validate extracted values
        return new Dictionary<string, object>();
    }
    
    private async Task RecordAIExecutionAsync(
        Guid instanceId, 
        Guid stepId,
        AIStepConfiguration config, 
        AIResult result)
    {
        // TODO: Save to WorkflowAIExecution table
    }
}

// TODO: Implement remaining step executors
// TODO: Add retry logic with exponential backoff
// TODO: Add timeout handling
// TODO: Add compensation logic for rollback
```

---

## Phase 3: AI Integration (Week 3)

### Task 3.1: Create AI Service Interface

**File**: `src/services/WorkflowService/WorkflowService.Core/Services/AIService.cs`

```csharp
namespace WorkflowService.Core.Services;

/// <summary>
/// Unified AI service interface for multiple providers
/// Copilot: Implement provider-specific adapters
/// </summary>
public interface IAIService
{
    Task<AIResult> ProcessAsync(AIRequest request);
    Task<bool> ValidateConfigurationAsync(AIStepConfiguration config);
    Task<decimal> EstimateCostAsync(AIRequest request);
}

public class AIRequest
{
    public AIServiceType Provider { get; set; }
    public string Model { get; set; }
    public string Prompt { get; set; }
    public string SystemPrompt { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public int? MaxTokens { get; set; }
    public decimal? Temperature { get; set; }
    public List<AIMessage> MessageHistory { get; set; }
}

public class AIResult
{
    public bool Success { get; set; }
    public string Response { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal Cost { get; set; }
    public int LatencyMs { get; set; }
    public string ModelUsed { get; set; }
    public decimal? ConfidenceScore { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class AIService : IAIService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AIService> _logger;
    private readonly Dictionary<AIServiceType, IAIProvider> _providers;
    
    public AIService(
        IServiceProvider serviceProvider,
        ILogger<AIService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _providers = InitializeProviders();
    }
    
    private Dictionary<AIServiceType, IAIProvider> InitializeProviders()
    {
        return new Dictionary<AIServiceType, IAIProvider>
        {
            { AIServiceType.OpenAI, _serviceProvider.GetService<OpenAIProvider>() },
            { AIServiceType.Anthropic, _serviceProvider.GetService<AnthropicProvider>() },
            { AIServiceType.Azure, _serviceProvider.GetService<AzureOpenAIProvider>() },
            // TODO: Add more providers
        };
    }
    
    public async Task<AIResult> ProcessAsync(AIRequest request)
    {
        if (!_providers.ContainsKey(request.Provider))
            throw new NotSupportedException($"Provider {request.Provider} not supported");
            
        var provider = _providers[request.Provider];
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await provider.ProcessAsync(request);
            result.LatencyMs = (int)stopwatch.ElapsedMilliseconds;
            
            // Calculate cost
            result.Cost = CalculateCost(request.Provider, request.Model, 
                result.InputTokens, result.OutputTokens);
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI processing failed for provider {Provider}", request.Provider);
            throw;
        }
    }
    
    private decimal CalculateCost(
        AIServiceType provider, 
        string model, 
        int inputTokens, 
        int outputTokens)
    {
        // TODO: Implement cost calculation based on provider pricing
        // TODO: Store pricing in configuration
        return 0.01m; // Placeholder
    }
    
    // TODO: Implement ValidateConfigurationAsync
    // TODO: Implement EstimateCostAsync
}

/// <summary>
/// Provider interface for different AI services
/// </summary>
public interface IAIProvider
{
    Task<AIResult> ProcessAsync(AIRequest request);
}

/// <summary>
/// OpenAI provider implementation
/// Copilot: Complete the OpenAI API integration
/// </summary>
public class OpenAIProvider : IAIProvider
{
    private readonly OpenAIClient _client;
    private readonly ILogger<OpenAIProvider> _logger;
    
    public OpenAIProvider(IConfiguration configuration, ILogger<OpenAIProvider> logger)
    {
        var apiKey = configuration["AI:OpenAI:ApiKey"];
        _client = new OpenAIClient(apiKey);
        _logger = logger;
    }
    
    public async Task<AIResult> ProcessAsync(AIRequest request)
    {
        try
        {
            var chatRequest = new ChatCompletionCreateRequest
            {
                Model = request.Model ?? "gpt-4",
                Messages = BuildMessages(request),
                MaxTokens = request.MaxTokens ?? 1000,
                Temperature = request.Temperature ?? 0.7f
            };
            
            var response = await _client.Chat.CreateCompletionAsync(chatRequest);
            
            return new AIResult
            {
                Success = true,
                Response = response.Choices.FirstOrDefault()?.Message.Content,
                InputTokens = response.Usage.PromptTokens,
                OutputTokens = response.Usage.CompletionTokens,
                ModelUsed = response.Model
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI API call failed");
            return new AIResult
            {
                Success = false,
                Response = $"Error: {ex.Message}"
            };
        }
    }
    
    private List<ChatMessage> BuildMessages(AIRequest request)
    {
        var messages = new List<ChatMessage>();
        
        if (!string.IsNullOrEmpty(request.SystemPrompt))
            messages.Add(new ChatMessage("system", request.SystemPrompt));
            
        if (request.MessageHistory != null)
        {
            foreach (var msg in request.MessageHistory)
                messages.Add(new ChatMessage(msg.Role, msg.Content));
        }
        
        messages.Add(new ChatMessage("user", request.Prompt));
        
        return messages;
    }
}

// TODO: Implement AnthropicProvider
// TODO: Implement AzureOpenAIProvider
// TODO: Add retry logic with exponential backoff
// TODO: Add rate limiting
// TODO: Add response validation
```

---

## Phase 4: Workflow API (Week 4)

### Task 4.1: Create Workflow Controller

**File**: `src/services/WorkflowService/WorkflowService.Api/Controllers/WorkflowController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WorkflowService.Api.Controllers;

/// <summary>
/// API controller for workflow operations
/// Copilot: Generate OpenAPI documentation and examples
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowEngine _workflowEngine;
    private readonly IWorkflowTemplateService _templateService;
    private readonly IWorkflowQueryService _queryService;
    private readonly ILogger<WorkflowController> _logger;
    
    public WorkflowController(
        IWorkflowEngine workflowEngine,
        IWorkflowTemplateService templateService,
        IWorkflowQueryService queryService,
        ILogger<WorkflowController> logger)
    {
        _workflowEngine = workflowEngine;
        _templateService = templateService;
        _queryService = queryService;
        _logger = logger;
    }
    
    /// <summary>
    /// Start a new workflow instance
    /// </summary>
    [HttpPost("start")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), 200)]
    public async Task<IActionResult> StartWorkflow([FromBody] StartWorkflowRequest request)
    {
        try
        {
            // TODO: Validate user has permission to start this workflow type
            // TODO: Extract tenant ID from user context
            // TODO: Validate initial context against workflow variables schema
            
            var tenantId = GetTenantId();
            var userId = GetUserId();
            
            var instance = await _workflowEngine.StartWorkflowAsync(
                request.TemplateId,
                tenantId,
                userId,
                request.InitialContext);
                
            var dto = MapToDto(instance);
            return Ok(dto);
        }
        catch (WorkflowException ex)
        {
            _logger.LogError(ex, "Failed to start workflow");
            return BadRequest(new { error = ex.Message });
        }
    }
    
    /// <summary>
    /// Get workflow instance details
    /// </summary>
    [HttpGet("instances/{instanceId}")]
    [ProducesResponseType(typeof(WorkflowInstanceDetailDto), 200)]
    public async Task<IActionResult> GetInstance(Guid instanceId)
    {
        var tenantId = GetTenantId();
        var instance = await _queryService.GetInstanceAsync(instanceId, tenantId);
        
        if (instance == null)
            return NotFound();
            
        return Ok(instance);
    }
    
    /// <summary>
    /// Get available actions for current workflow state
    /// </summary>
    [HttpGet("instances/{instanceId}/actions")]
    [ProducesResponseType(typeof(List<WorkflowActionDto>), 200)]
    public async Task<IActionResult> GetAvailableActions(Guid instanceId)
    {
        var tenantId = GetTenantId();
        var actions = await _queryService.GetAvailableActionsAsync(instanceId, tenantId);
        return Ok(actions);
    }
    
    /// <summary>
    /// Execute a workflow action (complete task, make decision, etc.)
    /// </summary>
    [HttpPost("instances/{instanceId}/actions/{actionName}")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), 200)]
    public async Task<IActionResult> ExecuteAction(
        Guid instanceId, 
        string actionName,
        [FromBody] Dictionary<string, object> actionData)
    {
        // TODO: Validate user has permission for this action
        // TODO: Validate action data against schema
        // TODO: Execute action through workflow engine
        
        return Ok();
    }
    
    /// <summary>
    /// Get workflow history/audit trail
    /// </summary>
    [HttpGet("instances/{instanceId}/history")]
    [ProducesResponseType(typeof(List<WorkflowEventDto>), 200)]
    public async Task<IActionResult> GetHistory(Guid instanceId)
    {
        var tenantId = GetTenantId();
        var events = await _queryService.GetWorkflowHistoryAsync(instanceId, tenantId);
        return Ok(events);
    }
    
    // TODO: Add endpoint for getting user's tasks
    // TODO: Add endpoint for claiming/releasing tasks
    // TODO: Add endpoint for workflow templates CRUD
    // TODO: Add endpoint for workflow monitoring/analytics
    // TODO: Add SignalR hub for real-time updates
    
    private Guid GetTenantId()
    {
        // Extract from user claims
        return Guid.Parse(User.FindFirst("TenantId")?.Value ?? Guid.Empty.ToString());
    }
    
    private string GetUserId()
    {
        return User.FindFirst("UserId")?.Value ?? "system";
    }
}
```

### Task 4.2: Create DTOs

**File**: `src/shared/DTOs/Workflow/WorkflowDtos.cs`

```csharp
namespace DTOs.Workflow;

/// <summary>
/// DTOs for workflow API
/// Copilot: Add validation attributes and examples
/// </summary>
public class StartWorkflowRequest
{
    [Required]
    public Guid TemplateId { get; set; }
    
    public Dictionary<string, object> InitialContext { get; set; } = new();
    
    public Dictionary<string, string> Metadata { get; set; }
}

public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public string InstanceIdentifier { get; set; }
    public Guid TemplateId { get; set; }
    public string TemplateName { get; set; }
    public string Status { get; set; }
    public string CurrentStep { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Dictionary<string, object> Context { get; set; }
}

public class WorkflowTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public List<WorkflowStepDto> Steps { get; set; }
    public Dictionary<string, VariableDefinition> Variables { get; set; }
}

public class WorkflowTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; }
    public string AssignedTo { get; set; }
    public Dictionary<string, object> FormData { get; set; }
    public WorkflowInstanceSummary WorkflowInfo { get; set; }
    public List<AIAssistanceSuggestion> AISuggestions { get; set; }
}

// TODO: Add remaining DTOs
// TODO: Add mapper configuration
// TODO: Add validation rules
```

---

## Phase 5: Testing & Documentation (Week 5)

### Task 5.1: Create Unit Tests

**File**: `src/services/WorkflowService/WorkflowService.Tests/WorkflowEngineTests.cs`

```csharp
using Xunit;
using Moq;
using FluentAssertions;

namespace WorkflowService.Tests;

/// <summary>
/// Unit tests for workflow engine
/// Copilot: Generate comprehensive test cases
/// </summary>
public class WorkflowEngineTests
{
    private readonly Mock<ILogger<WorkflowEngine>> _loggerMock;
    private readonly Mock<WorkflowDbContext> _contextMock;
    private readonly WorkflowEngine _engine;
    
    public WorkflowEngineTests()
    {
        _loggerMock = new Mock<ILogger<WorkflowEngine>>();
        _contextMock = new Mock<WorkflowDbContext>();
        // TODO: Setup mocks
        // TODO: Initialize engine
    }
    
    [Fact]
    public async Task StartWorkflow_ValidTemplate_CreatesInstance()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        
        // TODO: Setup mock returns
        
        // Act
        var result = await _engine.StartWorkflowAsync(
            templateId, tenantId, "user123", new Dictionary<string, object>());
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(WorkflowStatus.Active);
        // TODO: Add more assertions
    }
    
    [Fact]
    public async Task ExecuteStep_AIProcessing_CallsAIService()
    {
        // TODO: Test AI step execution
    }
    
    [Fact]
    public async Task TransitionToStep_ValidCondition_MovesToNextStep()
    {
        // TODO: Test transition logic
    }
    
    // TODO: Add test for parallel execution
    // TODO: Add test for error handling
    // TODO: Add test for compensation/rollback
    // TODO: Add integration tests
}
```

### Task 5.2: Create Sample Workflows

**File**: `src/services/WorkflowService/WorkflowService.Infrastructure/SeedData/SampleWorkflows.cs`

```csharp
namespace WorkflowService.Infrastructure.SeedData;

/// <summary>
/// Sample workflow templates for testing and demonstration
/// Copilot: Create complete workflow definitions
/// </summary>
public static class SampleWorkflows
{
    public static WorkflowTemplate CreateContractApprovalWorkflow()
    {
        return new WorkflowTemplate
        {
            Name = "Contract Approval with AI Review",
            Category = "Contract",
            Description = "AI-assisted contract review and approval process",
            Definition = @"
            {
                'triggers': ['manual'],
                'variables': {
                    'contractUrl': 'string',
                    'contractValue': 'number',
                    'riskScore': 'number',
                    'aiSummary': 'string'
                },
                'steps': [...]
            }",
            // TODO: Complete workflow definition
        };
    }
    
    // TODO: Add more sample workflows
    // TODO: Create seed data migration
}
```

---

## Phase 6: React Workflow Builder (Week 6-7)

### Task 6.1: Setup React Flow and Dependencies

**File**: `src/frontend/react-app/package.json` (additions)

```json
{
  "dependencies": {
    "reactflow": "^11.10.1",
    "@reactflow/node-resizer": "^2.2.0",
    "beautiful-react-dnd": "^13.1.1",
    "react-hook-form": "^7.48.2",
    "@dnd-kit/core": "^6.1.0",
    "@dnd-kit/sortable": "^8.0.0",
    "uuid": "^9.0.1",
    "dagre": "^0.8.5",
    "elkjs": "^0.8.2",
    "@monaco-editor/react": "^4.6.0",
    "react-hot-toast": "^2.4.1",
    "@tanstack/react-query": "^5.17.0",
    "zustand": "^4.4.7",
    "react-json-view": "^1.21.3"
  }
}
```

### Task 6.2: Create Workflow Store (Zustand)

**File**: `src/frontend/react-app/src/stores/workflowStore.ts`

```typescript
import { create } from 'zustand';
import { Node, Edge, Connection, NodeChange, EdgeChange } from 'reactflow';

interface WorkflowStep {
  id: string;
  type: StepType;
  data: {
    label: string;
    description?: string;
    config: any;
    aiConfig?: AIStepConfiguration;
    requiredPermission?: string;
  };
  position: { x: number; y: number };
}

interface WorkflowState {
  // Current workflow being edited
  workflowId?: string;
  workflowName: string;
  workflowDescription: string;
  workflowCategory: string;
  
  // React Flow state
  nodes: Node[];
  edges: Edge[];
  
  // Selected element for property panel
  selectedNode: Node | null;
  selectedEdge: Edge | null;
  
  // Workflow variables
  variables: Record<string, VariableDefinition>;
  
  // Validation state
  validationErrors: ValidationError[];
  
  // Actions
  setNodes: (nodes: Node[]) => void;
  setEdges: (edges: Edge[]) => void;
  onNodesChange: (changes: NodeChange[]) => void;
  onEdgesChange: (changes: EdgeChange[]) => void;
  onConnect: (connection: Connection) => void;
  
  addNode: (type: StepType, position: { x: number; y: number }) => void;
  updateNodeData: (nodeId: string, data: any) => void;
  deleteNode: (nodeId: string) => void;
  
  updateEdgeData: (edgeId: string, data: any) => void;
  
  selectNode: (node: Node | null) => void;
  selectEdge: (edge: Edge | null) => void;
  
  addVariable: (name: string, definition: VariableDefinition) => void;
  updateVariable: (name: string, definition: VariableDefinition) => void;
  deleteVariable: (name: string) => void;
  
  validateWorkflow: () => ValidationError[];
  saveWorkflow: () => Promise<void>;
  loadWorkflow: (id: string) => Promise<void>;
  
  // Auto-layout
  autoLayout: () => void;
  
  // Export/Import
  exportWorkflow: () => WorkflowDefinition;
  importWorkflow: (definition: WorkflowDefinition) => void;
}

export const useWorkflowStore = create<WorkflowState>((set, get) => ({
  workflowName: '',
  workflowDescription: '',
  workflowCategory: '',
  nodes: [],
  edges: [],
  selectedNode: null,
  selectedEdge: null,
  variables: {},
  validationErrors: [],
  
  setNodes: (nodes) => set({ nodes }),
  setEdges: (edges) => set({ edges }),
  
  onNodesChange: (changes) => {
    // TODO: Implement node change handler
    // Copilot: Apply node changes to state
  },
  
  onEdgesChange: (changes) => {
    // TODO: Implement edge change handler
    // Copilot: Apply edge changes to state
  },
  
  onConnect: (connection) => {
    // TODO: Create new edge from connection
    // Validate connection is allowed
    // Add transition data
  },
  
  addNode: (type, position) => {
    const newNode: Node = {
      id: `node_${Date.now()}`,
      type: getNodeType(type), // Map to custom node component
      position,
      data: {
        label: getDefaultLabel(type),
        type,
        config: getDefaultConfig(type)
      }
    };
    
    set((state) => ({
      nodes: [...state.nodes, newNode]
    }));
  },
  
  updateNodeData: (nodeId, data) => {
    set((state) => ({
      nodes: state.nodes.map(node =>
        node.id === nodeId
          ? { ...node, data: { ...node.data, ...data } }
          : node
      )
    }));
  },
  
  validateWorkflow: () => {
    const errors: ValidationError[] = [];
    const { nodes, edges } = get();
    
    // TODO: Validate has start node
    // TODO: Validate all nodes are connected
    // TODO: Validate no circular dependencies
    // TODO: Validate required fields
    // TODO: Validate AI configurations
    
    set({ validationErrors: errors });
    return errors;
  },
  
  autoLayout: async () => {
    // TODO: Use dagre or elk.js for auto-layout
    // Copilot: Implement graph layout algorithm
  },
  
  // TODO: Implement remaining methods
}));
```

### Task 6.3: Create Custom Node Components

**File**: `src/frontend/react-app/src/components/WorkflowBuilder/nodes/index.tsx`

```tsx
import React, { memo, FC } from 'react';
import { Handle, Position, NodeProps } from 'reactflow';
import { 
  FiUser, FiCpu, FiFileText, FiMail, FiGitBranch,
  FiClock, FiRepeat, FiCheckCircle, FiAlertCircle
} from 'react-icons/fi';
import { BsRobot } from 'react-icons/bs';

// Base node component with common functionality
const BaseNode: FC<NodeProps> = memo(({ data, selected }) => {
  const getIcon = () => {
    switch (data.type) {
      case 'Manual': return <FiUser />;
      case 'Approval': return <FiCheckCircle />;
      case 'AIProcessing': return <BsRobot />;
      case 'Automatic': return <FiCpu />;
      case 'DocumentGeneration': return <FiFileText />;
      case 'Notification': return <FiMail />;
      case 'Parallel': return <FiGitBranch />;
      case 'Timer': return <FiClock />;
      case 'Loop': return <FiRepeat />;
      default: return <FiAlertCircle />;
    }
  };
  
  const getNodeColor = () => {
    switch (data.type) {
      case 'Manual':
      case 'Approval': return 'bg-blue-500';
      case 'AIProcessing': return 'bg-purple-500';
      case 'Automatic': return 'bg-green-500';
      case 'DocumentGeneration': return 'bg-yellow-500';
      case 'Notification': return 'bg-indigo-500';
      default: return 'bg-gray-500';
    }
  };
  
  return (
    <div className={`
      workflow-node rounded-lg shadow-lg p-4 min-w-[200px]
      ${selected ? 'ring-2 ring-blue-400' : ''}
      ${data.hasError ? 'ring-2 ring-red-400' : ''}
      bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700
    `}>
      <Handle
        type="target"
        position={Position.Top}
        className="w-3 h-3 bg-gray-400"
      />
      
      <div className="flex items-center gap-2 mb-2">
        <div className={`p-2 rounded ${getNodeColor()} text-white`}>
          {getIcon()}
        </div>
        <div className="flex-1">
          <div className="font-semibold text-sm">{data.label}</div>
          <div className="text-xs text-gray-500">{data.type}</div>
        </div>
      </div>
      
      {data.description && (
        <div className="text-xs text-gray-600 dark:text-gray-400 mb-2">
          {data.description}
        </div>
      )}
      
      {data.requiredPermission && (
        <div className="text-xs bg-yellow-100 dark:bg-yellow-900 p-1 rounded">
          🔒 {data.requiredPermission}
        </div>
      )}
      
      {data.aiModel && (
        <div className="text-xs bg-purple-100 dark:bg-purple-900 p-1 rounded mt-1">
          🤖 {data.aiModel}
        </div>
      )}
      
      <Handle
        type="source"
        position={Position.Bottom}
        className="w-3 h-3 bg-gray-400"
      />
    </div>
  );
});

// AI Processing Node with special visualization
export const AIProcessingNode: FC<NodeProps> = memo(({ data, selected }) => {
  return (
    <div className={`
      ai-node rounded-lg shadow-xl p-4 min-w-[250px]
      bg-gradient-to-br from-purple-500 to-indigo-600 text-white
      ${selected ? 'ring-4 ring-purple-300' : ''}
    `}>
      <Handle type="target" position={Position.Top} />
      
      <div className="flex items-center gap-3 mb-3">
        <BsRobot className="text-3xl" />
        <div>
          <div className="font-bold">{data.label}</div>
          <div className="text-xs opacity-90">AI Processing</div>
        </div>
      </div>
      
      {data.aiConfig && (
        <div className="space-y-2 text-xs">
          <div className="bg-white/20 rounded p-2">
            <div className="font-semibold">Model:</div>
            <div>{data.aiConfig.model || 'Not configured'}</div>
          </div>
          
          {data.aiConfig.prompt && (
            <div className="bg-white/20 rounded p-2">
              <div className="font-semibold">Prompt Preview:</div>
              <div className="truncate">{data.aiConfig.prompt}</div>
            </div>
          )}
          
          {data.aiConfig.confidence && (
            <div className="bg-white/20 rounded p-2">
              <div className="font-semibold">Min Confidence:</div>
              <div>{data.aiConfig.confidence}%</div>
            </div>
          )}
        </div>
      )}
      
      <Handle type="source" position={Position.Bottom} />
    </div>
  );
});

// Parallel Gateway Node
export const ParallelNode: FC<NodeProps> = memo(({ data, selected }) => {
  return (
    <div className={`
      parallel-node diamond-shape
      ${selected ? 'ring-4 ring-orange-300' : ''}
    `}>
      <Handle type="target" position={Position.Top} />
      <FiGitBranch className="text-2xl text-orange-500" />
      <Handle 
        type="source" 
        position={Position.Right}
        id="branch1"
        style={{ top: '30%' }}
      />
      <Handle 
        type="source" 
        position={Position.Right}
        id="branch2"
        style={{ top: '70%' }}
      />
      <Handle 
        type="source" 
        position={Position.Bottom}
        id="default"
      />
    </div>
  );
});

// Export node type mapping
export const nodeTypes = {
  default: BaseNode,
  aiProcessing: AIProcessingNode,
  parallel: ParallelNode,
  // TODO: Add more custom node types
};

// Copilot: Generate remaining custom node components
```

### Task 6.4: Create Main Workflow Builder Component

**File**: `src/frontend/react-app/src/components/WorkflowBuilder/WorkflowBuilder.tsx`

```tsx
import React, { useCallback, useRef, useState } from 'react';
import ReactFlow, {
  ReactFlowProvider,
  Background,
  Controls,
  MiniMap,
  Panel,
  useReactFlow,
  BackgroundVariant,
  ConnectionMode,
  ReactFlowInstance,
} from 'reactflow';
import 'reactflow/dist/style.css';

import { useWorkflowStore } from '../../stores/workflowStore';
import { nodeTypes } from './nodes';
import { edgeTypes } from './edges';
import Sidebar from './Sidebar';
import PropertyPanel from './PropertyPanel';
import WorkflowToolbar from './WorkflowToolbar';
import VariableManager from './VariableManager';
import ValidationPanel from './ValidationPanel';

export const WorkflowBuilder: React.FC = () => {
  const reactFlowWrapper = useRef<HTMLDivElement>(null);
  const [reactFlowInstance, setReactFlowInstance] = useState<ReactFlowInstance | null>(null);
  const [showVariables, setShowVariables] = useState(false);
  const [showValidation, setShowValidation] = useState(false);
  
  const {
    nodes,
    edges,
    selectedNode,
    selectedEdge,
    onNodesChange,
    onEdgesChange,
    onConnect,
    addNode,
    selectNode,
    selectEdge,
    validateWorkflow,
    autoLayout,
    saveWorkflow
  } = useWorkflowStore();
  
  // Handle drag and drop from sidebar
  const onDragOver = useCallback((event: React.DragEvent) => {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
  }, []);
  
  const onDrop = useCallback(
    (event: React.DragEvent) => {
      event.preventDefault();
      
      const type = event.dataTransfer.getData('nodeType') as StepType;
      
      if (!type || !reactFlowInstance || !reactFlowWrapper.current) {
        return;
      }
      
      const reactFlowBounds = reactFlowWrapper.current.getBoundingClientRect();
      const position = reactFlowInstance.project({
        x: event.clientX - reactFlowBounds.left,
        y: event.clientY - reactFlowBounds.top,
      });
      
      addNode(type, position);
    },
    [reactFlowInstance, addNode]
  );
  
  const onNodeClick = useCallback((event: React.MouseEvent, node: Node) => {
    selectNode(node);
    selectEdge(null);
  }, [selectNode, selectEdge]);
  
  const onEdgeClick = useCallback((event: React.MouseEvent, edge: Edge) => {
    selectEdge(edge);
    selectNode(null);
  }, [selectNode, selectEdge]);
  
  const onPaneClick = useCallback(() => {
    selectNode(null);
    selectEdge(null);
  }, [selectNode, selectEdge]);
  
  const handleSave = async () => {
    const errors = validateWorkflow();
    if (errors.length > 0) {
      setShowValidation(true);
      toast.error('Please fix validation errors before saving');
      return;
    }
    
    try {
      await saveWorkflow();
      toast.success('Workflow saved successfully');
    } catch (error) {
      toast.error('Failed to save workflow');
    }
  };
  
  return (
    <div className="workflow-builder h-screen flex">
      {/* Left Sidebar - Node Palette */}
      <Sidebar />
      
      {/* Main Canvas */}
      <div className="flex-1 relative" ref={reactFlowWrapper}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          onNodesChange={onNodesChange}
          onEdgesChange={onEdgesChange}
          onConnect={onConnect}
          onInit={setReactFlowInstance}
          onDrop={onDrop}
          onDragOver={onDragOver}
          onNodeClick={onNodeClick}
          onEdgeClick={onEdgeClick}
          onPaneClick={onPaneClick}
          nodeTypes={nodeTypes}
          edgeTypes={edgeTypes}
          connectionMode={ConnectionMode.Loose}
          fitView
        >
          <Background 
            variant={BackgroundVariant.Dots}
            gap={12}
            size={1}
          />
          <Controls />
          <MiniMap 
            nodeColor={(node) => {
              switch (node.data?.type) {
                case 'AIProcessing': return '#8B5CF6';
                case 'Manual': return '#3B82F6';
                case 'Automatic': return '#10B981';
                default: return '#6B7280';
              }
            }}
          />
          
          {/* Top Toolbar */}
          <Panel position="top-center">
            <WorkflowToolbar
              onSave={handleSave}
              onValidate={() => setShowValidation(true)}
              onAutoLayout={autoLayout}
              onToggleVariables={() => setShowVariables(!showVariables)}
            />
          </Panel>
        </ReactFlow>
      </div>
      
      {/* Right Panel - Properties */}
      {(selectedNode || selectedEdge) && (
        <PropertyPanel
          node={selectedNode}
          edge={selectedEdge}
          onClose={() => {
            selectNode(null);
            selectEdge(null);
          }}
        />
      )}
      
      {/* Variable Manager Modal */}
      {showVariables && (
        <VariableManager onClose={() => setShowVariables(false)} />
      )}
      
      {/* Validation Panel */}
      {showValidation && (
        <ValidationPanel onClose={() => setShowValidation(false)} />
      )}
    </div>
  );
};

// Wrap with ReactFlowProvider
export default function WorkflowBuilderWithProvider() {
  return (
    <ReactFlowProvider>
      <WorkflowBuilder />
    </ReactFlowProvider>
  );
}
```

### Task 6.5: Create Property Panel for Node Configuration

**File**: `src/frontend/react-app/src/components/WorkflowBuilder/PropertyPanel.tsx`

```tsx
import React, { useState, useEffect } from 'react';
import { Node, Edge } from 'reactflow';
import { useForm } from 'react-hook-form';
import MonacoEditor from '@monaco-editor/react';
import { FiX, FiSettings, FiCode, FiDatabase } from 'react-icons/fi';
import { BsRobot } from 'react-icons/bs';

interface PropertyPanelProps {
  node: Node | null;
  edge: Edge | null;
  onClose: () => void;
}

export const PropertyPanel: React.FC<PropertyPanelProps> = ({ 
  node, 
  edge, 
  onClose 
}) => {
  const [activeTab, setActiveTab] = useState<'general' | 'config' | 'ai' | 'advanced'>('general');
  const { register, handleSubmit, watch, setValue, reset } = useForm();
  const { updateNodeData, updateEdgeData } = useWorkflowStore();
  
  useEffect(() => {
    if (node) {
      reset(node.data);
    } else if (edge) {
      reset(edge.data || {});
    }
  }, [node, edge, reset]);
  
  const onSubmit = (data: any) => {
    if (node) {
      updateNodeData(node.id, data);
    } else if (edge) {
      updateEdgeData(edge.id, data);
    }
  };
  
  const renderNodeProperties = () => {
    if (!node) return null;
    
    return (
      <div className="h-full flex flex-col">
        {/* Header */}
        <div className="flex items-center justify-between p-4 border-b">
          <h3 className="font-semibold">Node Properties</h3>
          <button onClick={onClose}>
            <FiX className="text-gray-500 hover:text-gray-700" />
          </button>
        </div>
        
        {/* Tabs */}
        <div className="flex border-b">
          <TabButton
            active={activeTab === 'general'}
            onClick={() => setActiveTab('general')}
            icon={<FiSettings />}
            label="General"
          />
          <TabButton
            active={activeTab === 'config'}
            onClick={() => setActiveTab('config')}
            icon={<FiCode />}
            label="Configuration"
          />
          {node.data.type === 'AIProcessing' && (
            <TabButton
              active={activeTab === 'ai'}
              onClick={() => setActiveTab('ai')}
              icon={<BsRobot />}
              label="AI Settings"
            />
          )}
          <TabButton
            active={activeTab === 'advanced'}
            onClick={() => setActiveTab('advanced')}
            icon={<FiDatabase />}
            label="Advanced"
          />
        </div>
        
        {/* Tab Content */}
        <form onSubmit={handleSubmit(onSubmit)} className="flex-1 overflow-y-auto p-4">
          {activeTab === 'general' && (
            <GeneralTab register={register} node={node} />
          )}
          
          {activeTab === 'config' && (
            <ConfigurationTab 
              node={node}
              value={watch('config')}
              onChange={(value) => setValue('config', value)}
            />
          )}
          
          {activeTab === 'ai' && node.data.type === 'AIProcessing' && (
            <AISettingsTab
              value={watch('aiConfig')}
              onChange={(value) => setValue('aiConfig', value)}
            />
          )}
          
          {activeTab === 'advanced' && (
            <AdvancedTab register={register} node={node} />
          )}
          
          <button
            type="submit"
            className="mt-4 w-full bg-blue-500 text-white py-2 rounded hover:bg-blue-600"
          >
            Save Changes
          </button>
        </form>
      </div>
    );
  };
  
  const renderEdgeProperties = () => {
    if (!edge) return null;
    
    return (
      <div className="p-4">
        <h3 className="font-semibold mb-4">Transition Properties</h3>
        <form onSubmit={handleSubmit(onSubmit)}>
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium mb-1">
                Transition Name
              </label>
              <input
                {...register('label')}
                className="w-full p-2 border rounded"
                placeholder="e.g., Approved, Rejected"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">
                Condition (JavaScript Expression)
              </label>
              <MonacoEditor
                height="100px"
                language="javascript"
                value={watch('condition') || ''}
                onChange={(value) => setValue('condition', value)}
                options={{
                  minimap: { enabled: false },
                  lineNumbers: 'off',
                  scrollBeyondLastLine: false
                }}
              />
              <div className="text-xs text-gray-500 mt-1">
                Example: result === 'approved' && context.amount > 1000
              </div>
            </div>
            
            <div>
              <label className="flex items-center gap-2">
                <input
                  type="checkbox"
                  {...register('isDefault')}
                />
                <span className="text-sm">Default transition</span>
              </label>
            </div>
            
            <button
              type="submit"
              className="w-full bg-blue-500 text-white py-2 rounded hover:bg-blue-600"
            >
              Save Transition
            </button>
          </div>
        </form>
      </div>
    );
  };
  
  return (
    <div className="w-96 bg-white dark:bg-gray-800 border-l shadow-lg h-full">
      {node ? renderNodeProperties() : edge ? renderEdgeProperties() : null}
    </div>
  );
};

// AI Settings Tab Component
const AISettingsTab: React.FC<{
  value: any;
  onChange: (value: any) => void;
}> = ({ value = {}, onChange }) => {
  const [promptTemplate, setPromptTemplate] = useState(value.promptTemplate || '');
  
  return (
    <div className="space-y-4">
      <div>
        <label className="block text-sm font-medium mb-1">AI Provider</label>
        <select
          value={value.provider || 'openai'}
          onChange={(e) => onChange({ ...value, provider: e.target.value })}
          className="w-full p-2 border rounded"
        >
          <option value="openai">OpenAI</option>
          <option value="anthropic">Anthropic</option>
          <option value="azure">Azure OpenAI</option>
        </select>
      </div>
      
      <div>
        <label className="block text-sm font-medium mb-1">Model</label>
        <select
          value={value.model || 'gpt-4'}
          onChange={(e) => onChange({ ...value, model: e.target.value })}
          className="w-full p-2 border rounded"
        >
          <option value="gpt-4">GPT-4</option>
          <option value="gpt-3.5-turbo">GPT-3.5 Turbo</option>
          <option value="claude-3-opus">Claude 3 Opus</option>
          <option value="claude-3-sonnet">Claude 3 Sonnet</option>
        </select>
      </div>
      
      <div>
        <label className="block text-sm font-medium mb-1">System Prompt</label>
        <textarea
          value={value.systemPrompt || ''}
          onChange={(e) => onChange({ ...value, systemPrompt: e.target.value })}
          className="w-full p-2 border rounded h-24"
          placeholder="You are a helpful assistant..."
        />
      </div>
      
      <div>
        <label className="block text-sm font-medium mb-1">
          Prompt Template
          <span className="text-xs text-gray-500 ml-2">
            Use {'{'}variable{'}'} for variables
          </span>
        </label>
        <MonacoEditor
          height="200px"
          language="markdown"
          value={promptTemplate}
          onChange={(value) => {
            setPromptTemplate(value || '');
            onChange({ ...value, promptTemplate: value });
          }}
          options={{
            minimap: { enabled: false },
            wordWrap: 'on'
          }}
        />
      </div>
      
      <div>
        <label className="block text-sm font-medium mb-1">
          Output Mapping
        </label>
        <div className="text-xs text-gray-500 mb-2">
          Map AI response fields to workflow variables
        </div>
        <MonacoEditor
          height="150px"
          language="json"
          value={JSON.stringify(value.outputMapping || {}, null, 2)}
          onChange={(value) => {
            try {
              const mapping = JSON.parse(value || '{}');
              onChange({ ...value, outputMapping: mapping });
            } catch (e) {
              // Invalid JSON, don't update
            }
          }}
        />
      </div>
      
      <div className="grid grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium mb-1">
            Max Tokens
          </label>
          <input
            type="number"
            value={value.maxTokens || 1000}
            onChange={(e) => onChange({ ...value, maxTokens: parseInt(e.target.value) })}
            className="w-full p-2 border rounded"
          />
        </div>
        
        <div>
          <label className="block text-sm font-medium mb-1">
            Temperature
          </label>
          <input
            type="number"
            step="0.1"
            min="0"
            max="2"
            value={value.temperature || 0.7}
            onChange={(e) => onChange({ ...value, temperature: parseFloat(e.target.value) })}
            className="w-full p-2 border rounded"
          />
        </div>
      </div>
      
      <div>
        <label className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={value.requiresHumanValidation || false}
            onChange={(e) => onChange({ ...value, requiresHumanValidation: e.target.checked })}
          />
          <span className="text-sm">Requires human validation</span>
        </label>
      </div>
      
      <div>
        <label className="block text-sm font-medium mb-1">
          Minimum Confidence Score (%)
        </label>
        <input
          type="number"
          min="0"
          max="100"
          value={(value.confidenceThreshold || 0.8) * 100}
          onChange={(e) => onChange({ 
            ...value, 
            confidenceThreshold: parseInt(e.target.value) / 100 
          })}
          className="w-full p-2 border rounded"
        />
      </div>
    </div>
  );
};

// TODO: Implement GeneralTab, ConfigurationTab, AdvancedTab components
// Copilot: Generate the remaining tab components
```

### Task 6.6: Create Sidebar with Draggable Nodes

**File**: `src/frontend/react-app/src/components/WorkflowBuilder/Sidebar.tsx`

```tsx
import React from 'react';
import { 
  FiUser, FiCpu, FiFileText, FiMail, FiGitBranch,
  FiClock, FiRepeat, FiCheckCircle, FiAlertCircle,
  FiLayers, FiLink
} from 'react-icons/fi';
import { BsRobot } from 'react-icons/bs';

const nodeCategories = [
  {
    name: 'Human Tasks',
    nodes: [
      { type: 'Manual', label: 'Manual Task', icon: FiUser, color: 'blue' },
      { type: 'Approval', label: 'Approval', icon: FiCheckCircle, color: 'blue' }
    ]
  },
  {
    name: 'AI & Automation',
    nodes: [
      { type: 'AIProcessing', label: 'AI Processing', icon: BsRobot, color: 'purple' },
      { type: 'AIDecision', label: 'AI Decision', icon: BsRobot, color: 'purple' },
      { type: 'Automatic', label: 'System Action', icon: FiCpu, color: 'green' }
    ]
  },
  {
    name: 'Documents & Communication',
    nodes: [
      { type: 'DocumentGeneration', label: 'Generate Document', icon: FiFileText, color: 'yellow' },
      { type: 'Notification', label: 'Send Notification', icon: FiMail, color: 'indigo' }
    ]
  },
  {
    name: 'Flow Control',
    nodes: [
      { type: 'Parallel', label: 'Parallel Gateway', icon: FiGitBranch, color: 'orange' },
      { type: 'Conditional', label: 'Conditional', icon: FiAlertCircle, color: 'red' },
      { type: 'Timer', label: 'Timer/Delay', icon: FiClock, color: 'gray' },
      { type: 'Loop', label: 'Loop', icon: FiRepeat, color: 'teal' },
      { type: 'SubWorkflow', label: 'Sub-Workflow', icon: FiLayers, color: 'pink' }
    ]
  }
];

export const Sidebar: React.FC = () => {
  const onDragStart = (event: React.DragEvent, nodeType: string) => {
    event.dataTransfer.setData('nodeType', nodeType);
    event.dataTransfer.effectAllowed = 'move';
  };
  
  return (
    <div className="w-64 bg-gray-50 dark:bg-gray-900 border-r p-4 overflow-y-auto">
      <h2 className="text-lg font-semibold mb-4">Workflow Steps</h2>
      
      {nodeCategories.map((category) => (
        <div key={category.name} className="mb-6">
          <h3 className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
            {category.name}
          </h3>
          <div className="space-y-2">
            {category.nodes.map((node) => {
              const Icon = node.icon;
              return (
                <div
                  key={node.type}
                  className={`
                    flex items-center gap-2 p-2 rounded cursor-move
                    bg-white dark:bg-gray-800 border
                    hover:shadow-md transition-shadow
                    hover:border-${node.color}-400
                  `}
                  draggable
                  onDragStart={(e) => onDragStart(e, node.type)}
                >
                  <div className={`p-1.5 rounded bg-${node.color}-100 text-${node.color}-600`}>
                    <Icon className="w-4 h-4" />
                  </div>
                  <span className="text-sm">{node.label}</span>
                </div>
              );
            })}
          </div>
        </div>
      ))}
      
      <div className="mt-6 p-3 bg-blue-50 dark:bg-blue-900/20 rounded">
        <h4 className="text-sm font-medium mb-2">💡 Pro Tips</h4>
        <ul className="text-xs space-y-1 text-gray-600 dark:text-gray-400">
          <li>• Drag nodes to the canvas</li>
          <li>• Connect nodes by dragging handles</li>
          <li>• Double-click to edit properties</li>
          <li>• Use Ctrl+Z to undo</li>
        </ul>
      </div>
    </div>
  );
};

export default Sidebar;
```

## Development Checklist

### Week 1: Database & Entities
- [ ] Create all entity classes with Copilot assistance
- [ ] Configure DbContext with relationships
- [ ] Add database migrations
- [ ] Create repository interfaces and implementations
- [ ] Add tenant isolation logic
- [ ] Write entity unit tests

### Week 2: Core Engine
- [ ] Implement workflow engine with state machine
- [ ] Create step executor framework
- [ ] Implement basic step types (Manual, Automatic)
- [ ] Add workflow context management
- [ ] Implement transition evaluation
- [ ] Add event recording

### Week 3: AI Integration
- [ ] Integrate OpenAI SDK
- [ ] Integrate Anthropic SDK
- [ ] Create unified AI service interface
- [ ] Implement AI step executor
- [ ] Add prompt template system
- [ ] Add AI cost tracking

### Week 4: API & Services
- [ ] Create REST API controllers
- [ ] Implement workflow query service
- [ ] Add task management endpoints
- [ ] Create SignalR hub for real-time updates
- [ ] Add authentication/authorization
- [ ] Generate OpenAPI documentation

### Week 5: Testing & Polish
- [ ] Write comprehensive unit tests
- [ ] Add integration tests
- [ ] Create sample workflows
- [ ] Write API documentation
- [ ] Performance optimization
- [ ] Security review

### Week 6-7: React Workflow Builder
- [ ] Setup React Flow and dependencies
- [ ] Create workflow state management (Zustand)
- [ ] Build custom node components for each step type
- [ ] Implement drag-and-drop from sidebar
- [ ] Create property panel for node/edge configuration
- [ ] Build AI configuration UI
- [ ] Add variable management system
- [ ] Implement workflow validation UI
- [ ] Add auto-layout functionality
- [ ] Create workflow testing/preview mode
- [ ] Implement save/load functionality
- [ ] Add real-time collaboration features (optional)
- [ ] Build workflow templates library
- [ ] Add workflow versioning UI
- [ ] Implement keyboard shortcuts

---

## Copilot Prompts for Implementation

Use these prompts with GitHub Copilot for faster implementation:

1. **Entity Generation**: "Generate a complete entity class with validation, navigation properties, and audit fields"

2. **Service Implementation**: "Implement this service interface with error handling, logging, and retry logic"

3. **Test Generation**: "Generate comprehensive unit tests for this method including edge cases"

4. **API Documentation**: "Add XML documentation comments with examples for this API endpoint"

5. **Mapping Code**: "Generate AutoMapper profile for mapping between entity and DTO"

6. **Validation**: "Add FluentValidation rules for this DTO with custom error messages"

7. **Error Handling**: "Add try-catch with appropriate logging and error response"

8. **Async Pattern**: "Convert this to async/await pattern with cancellation token support"

---

## Configuration Files

### appsettings.json Addition
```json
{
  "Workflow": {
    "MaxParallelSteps": 10,
    "DefaultTimeoutMinutes": 60,
    "EnableAIAssistance": true,
    "AI": {
      "OpenAI": {
        "ApiKey": "your-key",
        "DefaultModel": "gpt-4",
        "MaxTokens": 2000
      },
      "Anthropic": {
        "ApiKey": "your-key",
        "DefaultModel": "claude-3-opus"
      }
    },
    "Redis": {
      "ConnectionString": "localhost:6379",
      "CacheExpiration": "00:05:00"
    }
  }
}
```

---

## Notes for GitHub Copilot Usage

1. **Comment First**: Write detailed comments about what you want to implement, then let Copilot generate the code

2. **Use TODO Comments**: Mark places where you need to return with TODO comments - Copilot will help complete these

3. **Test-Driven**: Write test method signatures first, let Copilot generate test implementation

4. **Incremental Building**: Build complex methods incrementally - write the signature and initial structure, then add complexity

5. **Pattern Matching**: Once you implement one pattern (like a step executor), Copilot will help replicate it for similar classes

Remember: Review all Copilot suggestions carefully, especially for security-sensitive code like authentication and authorization.