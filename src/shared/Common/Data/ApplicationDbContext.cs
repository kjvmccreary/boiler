// FILE: src/shared/Common/Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DTOs.Entities;
using Contracts.Services; // ADDED: Import for ITenantProvider

namespace Common.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantProvider _tenantProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor,
        ITenantProvider tenantProvider) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantProvider = tenantProvider;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TenantUser> TenantUsers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureTenant(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureTenantUser(modelBuilder);
        ConfigureRefreshToken(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    // Configuration methods follow...
    private void ConfigureTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Domain).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Domain).HasMaxLength(255);
            entity.Property(e => e.SubscriptionPlan).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Settings).HasColumnType("jsonb");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
    }

    private void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EmailConfirmationToken).HasMaxLength(255);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Primary tenant relationship (optional - user can exist without a primary tenant)
            entity.HasOne(e => e.PrimaryTenant)
                  .WithMany(t => t.Users)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.SetNull); // If tenant is deleted, set TenantId to null
        });
    }

    private void ConfigureTenantUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Permissions).HasColumnType("jsonb");
            entity.Property(e => e.InvitedBy).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Relationships
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.TenantUsers)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.TenantUsers)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureRefreshToken(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.UserId });
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ExpiryDate).IsRequired();
            entity.Property(e => e.CreatedByIp).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RevokedByIp).HasMaxLength(50);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(255);
            entity.Property(e => e.DeviceInfo).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Relationships
            entity.HasOne(e => e.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}





//// FILE: src/shared/Common/Data/ApplicationDbContext.cs
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Http;
//using DTOs.Entities;
//using Common.Services;

//namespace Common.Data;

//public class ApplicationDbContext : DbContext
//{
//    private readonly IHttpContextAccessor _httpContextAccessor;
//    private readonly ITenantProvider _tenantProvider;

//    public ApplicationDbContext(
//        DbContextOptions<ApplicationDbContext> options,
//        IHttpContextAccessor httpContextAccessor,
//        ITenantProvider tenantProvider) : base(options)
//    {
//        _httpContextAccessor = httpContextAccessor;
//        _tenantProvider = tenantProvider;
//    }

//    public DbSet<Tenant> Tenants { get; set; }
//    public DbSet<User> Users { get; set; }
//    public DbSet<TenantUser> TenantUsers { get; set; }
//    public DbSet<RefreshToken> RefreshTokens { get; set; }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        ConfigureTenant(modelBuilder);
//        ConfigureUser(modelBuilder);
//        ConfigureTenantUser(modelBuilder);
//        ConfigureRefreshToken(modelBuilder);

//        base.OnModelCreating(modelBuilder);
//    }

//    private void ConfigureTenant(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<Tenant>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.HasIndex(e => e.Domain).IsUnique();
//            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
//            entity.Property(e => e.Domain).HasMaxLength(255);
//            entity.Property(e => e.SubscriptionPlan).IsRequired().HasMaxLength(50);
//            entity.Property(e => e.Settings).HasColumnType("jsonb");
//            entity.Property(e => e.CreatedAt).IsRequired();
//            entity.Property(e => e.UpdatedAt).IsRequired();
//        });
//    }

//    private void ConfigureUser(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<User>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
//            entity.Property(e => e.TenantId).IsRequired();
//            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
//            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
//            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
//            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
//            entity.Property(e => e.EmailConfirmationToken).HasMaxLength(255);
//            entity.Property(e => e.CreatedAt).IsRequired();
//            entity.Property(e => e.UpdatedAt).IsRequired();

//            // Tenant relationship
//            entity.HasOne(e => e.Tenant)
//                  .WithMany(t => t.Users)
//                  .HasForeignKey(e => e.TenantId)
//                  .OnDelete(DeleteBehavior.Restrict);

//            // Row-level security via check constraint (will be added in migration)
//            entity.HasCheckConstraint("CK_User_TenantId",
//                "\"TenantId\"::text = current_setting('app.current_tenant', true)");
//        });
//    }

//    private void ConfigureTenantUser(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<TenantUser>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
//            entity.Property(e => e.TenantId).IsRequired();
//            entity.Property(e => e.UserId).IsRequired();
//            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
//            entity.Property(e => e.Permissions).HasColumnType("jsonb");
//            entity.Property(e => e.InvitedBy).HasMaxLength(255);
//            entity.Property(e => e.CreatedAt).IsRequired();
//            entity.Property(e => e.UpdatedAt).IsRequired();

//            // Relationships
//            entity.HasOne(e => e.Tenant)
//                  .WithMany(t => t.TenantUsers)
//                  .HasForeignKey(e => e.TenantId)
//                  .OnDelete(DeleteBehavior.Cascade);

//            entity.HasOne(e => e.User)
//                  .WithMany(u => u.TenantUsers)
//                  .HasForeignKey(e => e.UserId)
//                  .OnDelete(DeleteBehavior.Cascade);
//        });
//    }

//    private void ConfigureRefreshToken(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<RefreshToken>(entity =>
//        {
//            entity.HasKey(e => e.Id);
//            entity.HasIndex(e => e.Token).IsUnique();
//            entity.HasIndex(e => new { e.TenantId, e.UserId });
//            entity.Property(e => e.TenantId).IsRequired();
//            entity.Property(e => e.UserId).IsRequired();
//            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
//            entity.Property(e => e.ExpiryDate).IsRequired();
//            entity.Property(e => e.CreatedByIp).IsRequired().HasMaxLength(50);
//            entity.Property(e => e.RevokedByIp).HasMaxLength(50);
//            entity.Property(e => e.ReplacedByToken).HasMaxLength(255);
//            entity.Property(e => e.DeviceInfo).HasMaxLength(500);
//            entity.Property(e => e.CreatedAt).IsRequired();
//            entity.Property(e => e.UpdatedAt).IsRequired();

//            // Relationships
//            entity.HasOne(e => e.User)
//                  .WithMany(u => u.RefreshTokens)
//                  .HasForeignKey(e => e.UserId)
//                  .OnDelete(DeleteBehavior.Cascade);

//            entity.HasOne(e => e.Tenant)
//                  .WithMany()
//                  .HasForeignKey(e => e.TenantId)
//                  .OnDelete(DeleteBehavior.Restrict);

//            // Row-level security
//            entity.HasCheckConstraint("CK_RefreshToken_TenantId",
//                "\"TenantId\"::text = current_setting('app.current_tenant', true)");
//        });
//    }

//    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//    {
//        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
//        if (tenantId.HasValue)
//        {
//            await Database.ExecuteSqlRawAsync(
//                "SELECT set_config('app.current_tenant', {0}, false)",
//                tenantId.ToString());
//        }

//        // Update timestamps
//        var entries = ChangeTracker.Entries<BaseEntity>();
//        foreach (var entry in entries)
//        {
//            switch (entry.State)
//            {
//                case EntityState.Added:
//                    entry.Entity.CreatedAt = DateTime.UtcNow;
//                    entry.Entity.UpdatedAt = DateTime.UtcNow;
//                    break;
//                case EntityState.Modified:
//                    entry.Entity.UpdatedAt = DateTime.UtcNow;
//                    break;
//            }
//        }

//        return await base.SaveChangesAsync(cancellationToken);
//    }

//    public override int SaveChanges()
//    {
//        return SaveChangesAsync().GetAwaiter().GetResult();
//    }
//}
