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
    private readonly int? _tenantFilter;

    public WorkflowDbContext(
        DbContextOptions<WorkflowDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        ITenantProvider tenantProvider,
        ILogger<WorkflowDbContext> logger) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantProvider = tenantProvider;
        _logger = logger;
        _tenantFilter = _tenantProvider.GetCurrentTenantIdAsync().GetAwaiter().GetResult();
    }

    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; } = default!;
    public DbSet<WorkflowInstance> WorkflowInstances { get; set; } = default!;
    public DbSet<WorkflowTask> WorkflowTasks { get; set; } = default!;
    public DbSet<WorkflowEvent> WorkflowEvents { get; set; } = default!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ApplyTenantQueryFilters(modelBuilder);

        ConfigureWorkflowDefinition(modelBuilder);
        ConfigureWorkflowInstance(modelBuilder);
        ConfigureWorkflowTask(modelBuilder);
        ConfigureWorkflowEvent(modelBuilder);
        ConfigureOutboxMessage(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkflowInstance>()
            .HasQueryFilter(e => !_tenantFilter.HasValue || e.TenantId == _tenantFilter.Value);

        modelBuilder.Entity<WorkflowTask>()
            .HasQueryFilter(e => !_tenantFilter.HasValue || e.TenantId == _tenantFilter.Value);

        modelBuilder.Entity<WorkflowDefinition>()
            .HasQueryFilter(e => !_tenantFilter.HasValue || e.TenantId == _tenantFilter.Value);

        // (Optional – if you also want Outbox / Events isolation uncomment)
        // modelBuilder.Entity<OutboxMessage>()
        //     .HasQueryFilter(e => !_tenantFilter.HasValue || e.TenantId == _tenantFilter.Value);
        // modelBuilder.Entity<WorkflowEvent>()
        //     .HasQueryFilter(e => !_tenantFilter.HasValue || e.TenantId == _tenantFilter.Value);
    }

    private static bool ShouldEnableTenantFiltersInTests()
    {
        var flag = Environment.GetEnvironmentVariable("ENABLE_TENANT_FILTERS_IN_TESTS");
        return string.Equals(flag, "true", StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        var isTestEnvironment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
            Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Testing" ||
            AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName?.Contains("xunit") == true ||
                          a.FullName?.Contains("Microsoft.VisualStudio.TestPlatform") == true);

        if (isTestEnvironment && !ShouldEnableTenantFiltersInTests())
            return;

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
                return tenantIdObj as int?;
            var task = _tenantProvider.GetCurrentTenantIdAsync();
            if (task.IsCompleted) return task.Result;
            return null;
        }
        catch
        {
            return null;
        }
    }

    private bool IsPostgresProvider =>
        Database.ProviderName?.IndexOf("Npgsql", StringComparison.OrdinalIgnoreCase) >= 0;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnforceDefinitionJsonImmutability();

        // NEW: auto-assign missing OutboxMessage IdempotencyKeys (legacy safety)
        AssignMissingOutboxIdempotencyKeys();

        _logger.LogInformation("🔍 WorkflowDbContext: SaveChangesAsync starting");
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();

        if (!tenantId.HasValue)
        {
            _logger.LogWarning("No tenant ID – proceeding (may fail on constraints).");
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        try
        {
            var connection = Database.GetDbConnection();
            var strategy = Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken);

                await using var tx = await Database.BeginTransactionAsync(cancellationToken);

                if (IsPostgresProvider)
                {
                    await Database.ExecuteSqlRawAsync(
                        "SELECT set_config('app.tenant_id', {0}, true)",
                        tenantId.Value.ToString());
                }

                SetTenantIdForNewEntities(tenantId.Value);
                UpdateTimestamps();

                var result = await base.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
                return result;
            });
        }
        catch (InvalidOperationException)
        {
            SetTenantIdForNewEntities(tenantId.Value);
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    private void EnforceDefinitionJsonImmutability()
    {
        var modifiedDefs = ChangeTracker.Entries<WorkflowDefinition>()
            .Where(e => e.State == EntityState.Modified)
            .ToList();

        foreach (var entry in modifiedDefs)
        {
            var wasPublished = (bool)entry.OriginalValues["IsPublished"];
            if (!wasPublished) continue;

            var originalJson = (string)entry.OriginalValues["JSONDefinition"];
            var currentJson = entry.CurrentValues.GetValue<string>("JSONDefinition") ?? "";

            if (!string.Equals(originalJson, currentJson, StringComparison.Ordinal))
            {
                _logger.LogError("IMMUTABILITY_VIOLATION defId={Id}", entry.Entity.Id);
                throw new InvalidOperationException("Published workflow JSONDefinition is immutable. Create a new version.");
            }
        }
    }

    private void SetTenantIdForNewEntities(int tenantId)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added) continue;
            switch (entry.Entity)
            {
                case WorkflowDefinition wd when wd.TenantId == 0:
                    wd.TenantId = tenantId; break;
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
            entity.Property(e => e.JSONDefinition).IsRequired();

            var isTest = IsTestEnvironment();
            if (!isTest)
            {
                entity.Property(e => e.JSONDefinition).HasColumnType("jsonb");
                entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowDefinition_TenantId",
                    "\"TenantId\" = current_setting('app.tenant_id')::int"));
            }
        });
    }

    private void ConfigureWorkflowInstance(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.TenantId, e.WorkflowDefinitionId });
            entity.Property(e => e.CurrentNodeIds);
            entity.Property(e => e.Context);

            var isTest = IsTestEnvironment();
            if (!isTest)
            {
                entity.Property(e => e.CurrentNodeIds).HasColumnType("jsonb");
                entity.Property(e => e.Context).HasColumnType("jsonb");
                entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowInstance_TenantId",
                    "\"TenantId\" = current_setting('app.tenant_id')::int"));
            }
        });
    }

    private void ConfigureWorkflowTask(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasIndex(e => new { e.TenantId, e.DueDate });
            entity.Property(e => e.Data);

            var isTest = IsTestEnvironment();
            if (!isTest)
            {
                entity.Property(e => e.Data).HasColumnType("jsonb");
                entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowTask_TenantId",
                    "\"TenantId\" = current_setting('app.tenant_id')::int"));
            }
        });
    }

    private void ConfigureWorkflowEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Type, e.OccurredAt });
            entity.Property(e => e.Data);

            var isTest = IsTestEnvironment();
            if (!isTest)
            {
                entity.Property(e => e.Data).HasColumnType("jsonb");
                entity.ToTable(t => t.HasCheckConstraint("CK_WorkflowEvent_TenantId",
                    "\"TenantId\" = current_setting('app.tenant_id')::int"));
            }
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
            entity.HasIndex(e => new { e.DeadLetter, e.CreatedAt }); // NEW: quick filter for dead-letter review

            entity.HasIndex(nameof(OutboxMessage.TenantId), nameof(OutboxMessage.CreatedAt))
                  .HasDatabaseName("IDX_Outbox_Unprocessed")
                  .HasFilter("\"ProcessedAt\" IS NULL");

            entity.Property(e => e.EventData);

            var isTest = IsTestEnvironment();
            if (!isTest)
            {
                entity.Property(e => e.EventData).HasColumnType("jsonb");
            }
        });
    }

    private bool IsTestEnvironment() =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Testing" ||
        AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName?.Contains("xunit") == true ||
                      a.FullName?.Contains("Microsoft.VisualStudio.TestPlatform") == true);

    private void AssignMissingOutboxIdempotencyKeys()
    {
        foreach (var entry in ChangeTracker.Entries<OutboxMessage>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                if (entry.Entity.IdempotencyKey == Guid.Empty)
                {
                    entry.Entity.IdempotencyKey = Guid.NewGuid();
                }
            }
        }
    }
}
