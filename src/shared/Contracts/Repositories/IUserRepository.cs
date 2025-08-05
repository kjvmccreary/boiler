// FILE: src/shared/Contracts/Repositories/IUserRepository.cs
using Common.Entities;

namespace Contracts.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetWithTenantRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailConfirmationTokenAsync(string token, CancellationToken cancellationToken = default);
    Task UpdateLastLoginAsync(int userId, CancellationToken cancellationToken = default);
    Task IncrementFailedLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task ResetFailedLoginAttemptsAsync(int userId, CancellationToken cancellationToken = default);
    Task LockUserAsync(int userId, DateTime lockoutEnd, CancellationToken cancellationToken = default);
}
