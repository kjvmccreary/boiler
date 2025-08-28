using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Models;
using DTOs.Entities;
using Contracts.Services;
using Microsoft.Extensions.Logging;
using System.Data;

namespace WorkflowService.Persistence;

public class WorkflowDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<WorkflowDbContext> _logger;

    public WorkflowDbContext(
        DbContextOptions<WorkflowDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        ITenantProvider tenantProvider,
        ILogger<WorkflowDbContext> logger) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    // Workflow entities
    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<WorkflowTask> WorkflowTasks { get; set; }
    public DbSet<WorkflowEvent> WorkflowEvents { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ApplyTenantQueryFilters(modelBuilder);

        ConfigureWorkflowDefinition(modelBuilder);
        ConfigureWorkflowInstance(modelBuilder);
        ConfigureWorkflowTask(modelBuilder);
        ConfigureWorkflowEvent(modelBuilder);
        ConfigureOutboxMessage(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        var isTestEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
                               Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Testing" ||
                               AppDomain.CurrentDomain.GetAssemblies()
                                   .Any(a => a.FullName?.Contains("xunit") == true ||
                                            a.FullName?.Contains("Microsoft.VisualStudio.TestPlatform") == true);

        if (isTestEnvironment)
        {
            return;
        }

        modelBuilder.Entity<WorkflowDefinition>().HasQueryFilter(wd =>
            EF.Property<int>(wd, "TenantId") == GetCurrentTenantIdFromProvider());

        modelBuilder.Entity<WorkflowInstance>().HasQueryFilter(wi =>
            EF.Property<int>(wi, "TenantId") == GetCurrentTenantIdFromProvider());

        modelBuilder.Entity<WorkflowTask>().HasQueryFilter(wt =>
            EF.Property<int>(wt, "TenantId") == GetCurrentTenantIdFromProvider());

        modelBuilder.Entity<WorkflowEvent>().HasQueryFilter(we =>
            EF.Property<int>(we, "TenantId") == GetCurrentTenantIdFromProvider());
    }

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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🔍 WorkflowDbContext: Starting SaveChangesAsync");

        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        _logger.LogInformation("🏢 WorkflowDbContext: Tenant ID resolved as: {TenantId}", tenantId);

        if (!tenantId.HasValue)
        {
            _logger.LogWarning("❌ WorkflowDbContext: No tenant ID available - DB constraint will likely fail");
            // Still stamp timestamps to keep behavior consistent
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            // Ensure one connection and a transaction for GUC + INSERT
            if (Database.GetDbConnection().State != ConnectionState.Open)
            {
                await Database.GetDbConnection().OpenAsync(cancellationToken);
            }

            await using var tx = await Database.BeginTransactionAsync(cancellationToken);

            // Transaction-local tenant context (RLS)
            await Database.ExecuteSqlRawAsync(
                "SELECT set_config('app.tenant_id', {0}, true)",
                tenantId.Value.ToString());

            _logger.LogDebug("✅ WorkflowDbContext: Transaction-local tenant set to {TenantId}", tenantId.Value);

            // Optional: verify GUC (useful while stabilizing)
            // var guc = await Database.SqlQueryRaw<string>("SELECT current_setting('app.tenant_id', true)").SingleAsync();
            // _logger.LogDebug("🔎 Verified app.tenant_id GUC = '{Guc}'", guc ?? "<null>");

            // Auto-populate and timestamps just before saving
            SetTenantIdForNewEntities(tenantId.Value);
            UpdateTimestamps();

            var result = await base.SaveChangesAsync(cancellationToken);

            await tx.CommitAsync(cancellationToken);
            return result;
        });
    }

    private void SetTenantIdForNewEntities(int tenantId)
    {
        var entitiesUpdated = 0;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added) continue;

            if (entry.Entity is WorkflowDefinition wd)
            {
                if (wd.TenantId == 0)
                {
                    wd.TenantId = tenantId;
                    entitiesUpdated++;
                    _logger.LogDebug("✅ Set TenantId={TenantId} for WorkflowDefinition: {Name}", tenantId, wd.Name);
                }

                // Normalize optional strings to NULL
                foreach (var property in entry.Properties)
                {
                    var value = property.CurrentValue;
                    var propertyName = property.Metadata.Name;
                    var clrType = property.Metadata.ClrType;

                    if (clrType == typeof(string) && value is string s && s == "" &&
                        (propertyName == "Tags" || propertyName == "PublishNotes" || propertyName == "VersionNotes" || propertyName == "Description"))
                    {
                        property.CurrentValue = null;
                    }
                }

                _logger.LogInformation("🔧 Null enforcement applied to WorkflowDefinition: {Name}", wd.Name);
            }
            else if (entry.Entity is WorkflowInstance wi && wi.TenantId == 0)
            {
                wi.TenantId = tenantId; entitiesUpdated++;
            }
            else if (entry.Entity is WorkflowTask wt && wt.TenantId == 0)
            {
                wt.TenantId = tenantId; entitiesUpdated++;
            }
            else if (entry.Entity is WorkflowEvent we && we.TenantId == 0)
            {
                we.TenantId = tenantId; entitiesUpdated++;
            }
        }

        _logger.LogInformation("✅ WorkflowDbContext: Updated TenantId for {Count} entities", entitiesUpdated);
    }

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

            entity.HasIndex(e => new { e.TenantId, e.Name, e.Version }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.IsPublished });

            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Version).IsRequired();
            entity.Property(e => e.JSONDefinition).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.IsPublished).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowDefinition_TenantId",
                "\"TenantId\" = current_setting('app.tenant_id')::int"));

            entity.Property(e => e.Tags).IsRequired(false).HasMaxLength(500);
            entity.Property(e => e.PublishNotes).IsRequired(false).HasMaxLength(1000);
            entity.Property(e => e.VersionNotes).IsRequired(false).HasMaxLength(1000);
            entity.Property(e => e.PublishedByUserId).IsRequired(false);
            entity.Property(e => e.PublishedAt).IsRequired(false);
            entity.Property(e => e.ParentDefinitionId).IsRequired(false); // keep mapped for versioning
        });
    }

    private void ConfigureWorkflowInstance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.TenantId, e.WorkflowDefinitionId });
            entity.HasIndex(e => new { e.TenantId, e.StartedAt });

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

            entity.HasOne(e => e.WorkflowDefinition)
                  .WithMany(wd => wd.Instances)
                  .HasForeignKey(e => e.WorkflowDefinitionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowInstance_TenantId",
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureWorkflowTask(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowTask>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.TenantId, e.AssignedToUserId });
            entity.HasIndex(e => new { e.TenantId, e.DueDate });
            entity.HasIndex(e => new { e.WorkflowInstanceId, e.Status });
            entity.HasIndex(e => new { e.AssignedToRole, e.Status });

            entity.Property(e => e.TenantId).IsRequired();
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

            entity.HasOne(e => e.WorkflowInstance)
                  .WithMany(wi => wi.Tasks)
                  .HasForeignKey(e => e.WorkflowInstanceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowTask_TenantId",
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureWorkflowEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowEvent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.TenantId, e.Type, e.OccurredAt });
            entity.HasIndex(e => new { e.TenantId, e.UserId });
            entity.HasIndex(e => new { e.WorkflowInstanceId, e.OccurredAt });

            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.WorkflowInstanceId).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Data).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.OccurredAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.WorkflowInstance)
                  .WithMany(wi => wi.Events)
                  .HasForeignKey(e => e.WorkflowInstanceId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowEvent_TenantId",
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureOutboxMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.Processed, e.CreatedAt });
            entity.HasIndex(e => new { e.Type, e.Processed });
            entity.HasIndex(e => e.NextRetryAt);

            entity.Property(e => e.Type).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Payload).IsRequired().HasColumnType("jsonb");
            entity.Property(e => e.Processed).IsRequired();
            entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
            entity.Property(e => e.RetryCount).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
    }

    #endregion
}
