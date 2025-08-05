// FILE: src/shared/Common/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;
using DTOs.Entities;
using Contracts.Repositories;
using Contracts.Services;

namespace Common.Repositories;

public class UserRepository : TenantRepository<User>, IUserRepository
{
    public UserRepository(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger<UserRepository> logger)
        : base(context, tenantProvider, logger)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<User?> GetWithTenantRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(u => u.TenantUsers)
            .ThenInclude(tu => tu.Tenant)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(u => u.TenantUsers)
            .Where(u => u.TenantUsers.Any(tu => tu.Role == role && tu.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(cancellationToken);

        var term = searchTerm.ToLower();
        return await Query()
            .Where(u => u.Email.ToLower().Contains(term) ||
                       u.FirstName.ToLower().Contains(term) ||
                       u.LastName.ToLower().Contains(term))
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailConfirmationTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token, cancellationToken);
    }

    public async Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task IncrementFailedLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.FailedLoginAttempts++;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task ResetFailedLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.FailedLoginAttempts = 0;
            user.LockedOutUntil = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task LockUserAsync(int userId, DateTime lockoutEnd, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.LockedOutUntil = lockoutEnd;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
