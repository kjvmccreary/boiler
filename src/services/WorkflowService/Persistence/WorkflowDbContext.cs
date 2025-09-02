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

        if (isTestEnvironment) return;

        modelBuilder.Entity<WorkflowDefinition>().HasQueryFilter(wd =>
            EF.Property<int>(wd, "TenantId") == GetCurrentTenantIdFromProvider());
        modelBuilder.Entity<WorkflowInstance>().HasQueryFilter(wi =>
            EF.Property<int>(wi, "TenantId") == GetCurrentTenantIdFromProvider());
        modelBuilder.Entity<WorkflowTask>().HasQueryFilter(wt =>
            EF.Property<int>(wt, "TenantId") == GetCurrentTenantIdFromProvider());
        modelBuilder.Entity<WorkflowEvent>().HasQueryFilter(we =>
            EF.Property<int>(we, "TenantId") == GetCurrentTenantIdFromProvider());
        // OutboxMessages intentionally not tenant filtered (diagnostics / cross-scope ops)
    }

    private int? GetCurrentTenantIdFromProvider()
    {
        try
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Items.TryGetValue("TenantId", out var tenantIdObj) == true)
                return tenantIdObj as int?;
            var task = _tenantProvider.GetCurrentTenantIdAsync();
            if (task.IsCompleted) return task.Result;
            return null;
        }
        catch { return null; }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("🔍 WorkflowDbContext: SaveChangesAsync starting");
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();

        if (!tenantId.HasValue)
        {
            _logger.LogWarning("No tenant ID – proceeding (may fail on constraints).");
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            if (Database.GetDbConnection().State != ConnectionState.Open)
                await Database.GetDbConnection().OpenAsync(cancellationToken);

            await using var tx = await Database.BeginTransactionAsync(cancellationToken);

            await Database.ExecuteSqlRawAsync(
                "SELECT set_config('app.tenant_id', {0}, true)",
                tenantId.Value.ToString());

            SetTenantIdForNewEntities(tenantId.Value);
            UpdateTimestamps();

            var result = await base.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return result;
        });
    }

    private void SetTenantIdForNewEntities(int tenantId)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added) continue;
            switch (entry.Entity)
            {
                case WorkflowDefinition wd when wd.TenantId == 0:
                    wd.TenantId = tenantId;
                    break;
                case WorkflowInstance wi when wi.TenantId == 0:
                    wi.TenantId = tenantId; break;
                case WorkflowTask wt when wt.TenantId == 0:
                    wt.TenantId = tenantId; break;
                case WorkflowEvent we when we.TenantId == 0:
                    we.TenantId = tenantId; break;
                case OutboxMessage om when om.TenantId == 0:
                    om.TenantId = tenantId; break;
            }
        }
    }

    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }

    private void ConfigureWorkflowDefinition(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Name, e.Version }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.IsPublished });
            entity.Property(e => e.JSONDefinition).IsRequired().HasColumnType("jsonb");
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowDefinition_TenantId",
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureWorkflowInstance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.TenantId, e.WorkflowDefinitionId });
            entity.Property(e => e.CurrentNodeIds).HasColumnType("jsonb");
            entity.Property(e => e.Context).HasColumnType("jsonb");
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
            entity.HasIndex(e => new { e.TenantId, e.DueDate });
            entity.Property(e => e.Data).HasColumnType("jsonb");
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
            entity.Property(e => e.Data).HasColumnType("jsonb");
            entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowEvent_TenantId",
                "\"TenantId\" = current_setting('app.tenant_id')::int"));
        });
    }

    private void ConfigureOutboxMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.IsProcessed, e.CreatedAt });
            entity.HasIndex(e => new { e.EventType, e.IsProcessed });
            entity.HasIndex(e => e.NextRetryAt);
            entity.HasIndex(e => new { e.TenantId, e.IdempotencyKey }).IsUnique();
            entity.Property(e => e.EventData).HasColumnType("jsonb");
        });
    }
}
