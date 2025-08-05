// FILE: src/shared/Common/Repositories/RefreshTokenRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;
using DTOs.Entities;
using Contracts.Repositories;
using Contracts.Services;

namespace Common.Repositories;

public class RefreshTokenRepository : TenantRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger<RefreshTokenRepository> logger)
        : base(context, tenantProvider, logger)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(rt => rt.ExpiryDate <= DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeTokenAsync(string token, string? revokedByIp = null, string? replacedByToken = null, CancellationToken cancellationToken = default)
    {
        var refreshToken = await GetByTokenAsync(token, cancellationToken);
        if (refreshToken != null && !refreshToken.IsRevoked)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = revokedByIp;
            refreshToken.ReplacedByToken = replacedByToken;
            refreshToken.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RevokeAllUserTokensAsync(int userId, string? revokedByIp = null, CancellationToken cancellationToken = default)
    {
        var activeTokens = await GetActiveTokensForUserAsync(userId, cancellationToken);

        foreach (var refreshToken in activeTokens)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = revokedByIp;
            refreshToken.UpdatedAt = DateTime.UtcNow;
        }

        if (activeTokens.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var expiredTokens = await GetExpiredTokensAsync(cancellationToken);

        // Only delete tokens that have been expired for more than a certain period
        var cutoffDate = DateTime.UtcNow.AddDays(-30); // Keep expired tokens for 30 days for auditing
        var tokensToDelete = expiredTokens.Where(rt => rt.ExpiryDate < cutoffDate).ToList();

        if (tokensToDelete.Any())
        {
            _dbSet.RemoveRange(tokensToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cleaned up {Count} expired refresh tokens", tokensToDelete.Count);
        }
    }
}
