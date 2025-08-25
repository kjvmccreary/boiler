using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Models;
using DTOs.Entities;
using Contracts.Services;

namespace WorkflowService.Persistence;

public class WorkflowDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantProvider _tenantProvider;

    public WorkflowDbContext(
        DbContextOptions<WorkflowDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        ITenantProvider tenantProvider) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantProvider = tenantProvider;
    }

    // Workflow entities
    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<WorkflowTask> WorkflowTasks { get; set; }
    public DbSet<WorkflowEvent> WorkflowEvents { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply global query filters for tenant isolation
        ApplyTenantQueryFilters(modelBuilder);

        // Configure workflow entities
        ConfigureWorkflowDefinition(modelBuilder);
        ConfigureWorkflowInstance(modelBuilder);
        ConfigureWorkflowTask(modelBuilder);
        ConfigureWorkflowEvent(modelBuilder);
        ConfigureOutboxMessage(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Apply tenant isolation query filters following the same pattern as ApplicationDbContext
    /// </summary>
    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        // Check if we're in a test environment (same pattern as ApplicationDbContext)
        var isTestEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
                               Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Testing" ||
                               AppDomain.CurrentDomain.GetAssemblies()
                                   .Any(a => a.FullName?.Contains("xunit") == true ||
                                            a.FullName?.Contains("Microsoft.VisualStudio.TestPlatform") == true);

        if (isTestEnvironment)
        {
            return;
        }

        // ✅ IMPROVED: Apply direct tenant filters (now that all entities have TenantId)
        modelBuilder.Entity<WorkflowDefinition>().HasQueryFilter(wd => 
            EF.Property<int>(wd, "TenantId") == GetCurrentTenantIdFromProvider());

        modelBuilder.Entity<WorkflowInstance>().HasQueryFilter(wi => 
            EF.Property<int>(wi, "TenantId") == GetCurrentTenantIdFromProvider());

        // ✅ FIXED: Now use direct TenantId instead of relationship-based filtering
        modelBuilder.Entity<WorkflowTask>().HasQueryFilter(wt => 
            EF.Property<int>(wt, "TenantId") == GetCurrentTenantIdFromProvider());

        modelBuilder.Entity<WorkflowEvent>().HasQueryFilter(we => 
            EF.Property<int>(we, "TenantId") == GetCurrentTenantIdFromProvider());

        // OutboxMessage doesn't need tenant filtering as it's infrastructure
    }

    /// <summary>
    /// Get current tenant ID for query filters (same pattern as ApplicationDbContext)
    /// </summary>
    private int? GetCurrentTenantIdFromProvider()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items.TryGetValue("TenantId", out var tenantIdObj) == true)
            {
                return tenantIdObj as int?;
            }

            var task = _tenantProvider.GetCurrentTenantIdAsync();
            if (task.IsCompleted)
            {
                return task.Result;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Override SaveChanges to set tenant context and auto-populate TenantId
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set database tenant context for RLS (same pattern as ApplicationDbContext)
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (tenantId.HasValue)
        {
            try
            {
                await Database.ExecuteSqlRawAsync(
                    "SELECT set_config('app.tenant_id', {0}, false)", 
                    tenantId.Value.ToString());
            }
            catch
            {
                // Continue if RLS context setting fails
            }

            // Auto-populate TenantId for new workflow entities
            SetTenantIdForNewEntities(tenantId.Value);
        }

        // Update timestamps
        UpdateTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Auto-populate TenantId for new workflow entities
    /// </summary>
    private void SetTenantIdForNewEntities(int tenantId)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                // ✅ IMPROVED: Set TenantId for all workflow entities
                if (entry.Entity is WorkflowDefinition wd && wd.TenantId == 0)
                {
                    wd.TenantId = tenantId;
                }
                else if (entry.Entity is WorkflowInstance wi && wi.TenantId == 0)
                {
                    wi.TenantId = tenantId;
                }
                else if (entry.Entity is WorkflowTask wt && wt.TenantId == 0)
                {
                    wt.TenantId = tenantId;
                }
                else if (entry.Entity is WorkflowEvent we && we.TenantId == 0)
                {
                    we.TenantId = tenantId;
                }
            }
        }
    }

    /// <summary>
    /// Update timestamps for all entities
    /// </summary>
    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
                
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }

    #region Entity Configurations

    private void ConfigureWorkflowDefinition(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Tenant isolation index
            entity.HasIndex(e => new { e.TenantId, e.Name, e.Version }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.IsPublished });
            
            // Properties
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Version).IsRequired();
            entity.Property(e => e.JSONDefinition).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.IsPublished).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Row-Level Security check constraint (same pattern as ApplicationDbContext)
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowDefinition_TenantId", 
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureWorkflowInstance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Tenant isolation and performance indexes
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.TenantId, e.WorkflowDefinitionId });
            entity.HasIndex(e => new { e.TenantId, e.StartedAt });
            
            // Properties
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.WorkflowDefinitionId).IsRequired();
            entity.Property(e => e.DefinitionVersion).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CurrentNodeIds).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.Context).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.StartedAt).IsRequired();
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Relationships
            entity.HasOne(e => e.WorkflowDefinition)
                  .WithMany(wd => wd.Instances)
                  .HasForeignKey(e => e.WorkflowDefinitionId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Row-Level Security check constraint
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowInstance_TenantId", 
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureWorkflowTask(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // ✅ ADD: Tenant isolation indexes
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.TenantId, e.AssignedToUserId });
            entity.HasIndex(e => new { e.TenantId, e.DueDate });
            entity.HasIndex(e => new { e.WorkflowInstanceId, e.Status });
            entity.HasIndex(e => new { e.AssignedToRole, e.Status });
            
            // Properties
            entity.Property(e => e.TenantId).IsRequired(); // ✅ ADD
            entity.Property(e => e.WorkflowInstanceId).IsRequired();
            entity.Property(e => e.NodeId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TaskName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.AssignedToRole).HasMaxLength(100);
            entity.Property(e => e.Data).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.CompletionData).HasMaxLength(1000);
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Relationships
            entity.HasOne(e => e.WorkflowInstance)
                  .WithMany(wi => wi.Tasks)
                  .HasForeignKey(e => e.WorkflowInstanceId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Row-Level Security check constraint
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowTask_TenantId", 
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureWorkflowEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // ✅ ADD: Tenant isolation indexes
            entity.HasIndex(e => new { e.TenantId, e.Type, e.OccurredAt });
            entity.HasIndex(e => new { e.TenantId, e.UserId });
            entity.HasIndex(e => new { e.WorkflowInstanceId, e.OccurredAt });
            
            // Properties
            entity.Property(e => e.TenantId).IsRequired(); // ✅ ADD
            entity.Property(e => e.WorkflowInstanceId).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Data).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.OccurredAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Relationships
            entity.HasOne(e => e.WorkflowInstance)
                  .WithMany(wi => wi.Events)
                  .HasForeignKey(e => e.WorkflowInstanceId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Row-Level Security check constraint
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowEvent_TenantId", 
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureOutboxMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Processing indexes
            entity.HasIndex(e => new { e.Processed, e.CreatedAt });
            entity.HasIndex(e => new { e.Type, e.Processed });
            entity.HasIndex(e => e.NextRetryAt);
            
            // Properties
            entity.Property(e => e.Type).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Payload).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.Processed).IsRequired();
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.Property(e => e.RetryCount).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // OutboxMessage is infrastructure - no tenant isolation needed
        });
    }

    #endregion
}
