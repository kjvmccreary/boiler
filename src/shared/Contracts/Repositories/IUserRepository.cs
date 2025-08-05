// FILE: src/shared/Contracts/Repositories/IUserRepository.cs
using UserEntity = DTOs.Entities.User; // Type alias to avoid namespace conflict

namespace Contracts.Repositories;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetWithTenantRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserEntity>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailConfirmationTokenAsync(string token, CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default);
    Task IncrementFailedLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task ResetFailedLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task LockUserAsync(int userId, DateTime lockoutEnd, CancellationToken cancellationToken = default);
}
