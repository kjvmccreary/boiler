// FILE: src/shared/Contracts/Repositories/IRefreshTokenRepository.cs
using RefreshTokenEntity = DTOs.Entities.RefreshToken; // Type alias to avoid namespace conflict

namespace Contracts.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshTokenEntity>
{
    Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshTokenEntity>> GetActiveTokensForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RefreshTokenEntity>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);
    Task RevokeTokenAsync(string token, string? revokedByIp = null, string? replacedByToken = null, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(int userId, string? revokedByIp = null, CancellationToken cancellationToken = default);
    Task CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
}
