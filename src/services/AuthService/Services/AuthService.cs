// FILE: src/services/AuthService/Services/AuthService.cs
using AutoMapper;
using Common.Configuration;
using Common.Data;
using Contracts.Auth;
using Contracts.Repositories; 
using Contracts.Services; // NEW: Add for IPermissionService and ITenantProvider
using DTOs.Auth;
using DTOs.Common;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services;

public class AuthServiceImplementation : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthServiceImplementation> _logger;
    private readonly JwtSettings _jwtSettings;
    private readonly ITenantManagementRepository _tenantRepository;
    private readonly IPermissionService _permissionService;
    private readonly ITenantProvider _tenantProvider;
    private readonly IServiceProvider _serviceProvider; // 🔧 ADD: For scoped role template service

    public AuthServiceImplementation(
        ApplicationDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<AuthServiceImplementation> logger,
        JwtSettings jwtSettings,
        ITenantManagementRepository tenantRepository,
        IPermissionService permissionService,
        ITenantProvider tenantProvider,
        IServiceProvider serviceProvider) // 🔧 ADD: Service provider for scoped services
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
        _jwtSettings = jwtSettings;
        _tenantRepository = tenantRepository;
        _permissionService = permissionService;
        _tenantProvider = tenantProvider;
        _serviceProvider = serviceProvider; // 🔧 ADD: Initialize service provider
    }

    public async Task<ApiResponseDto<TokenResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _context.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            // 🔧 SIMPLIFIED: If user exists, direct them to login
            if (existingUser != null)
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult(
                    "An account with this email already exists. Please sign in and create additional organizations from your dashboard."
                );
            }

            // Continue with normal new user registration...
            var tenant = await GetTenantForRegistrationAsync(request, cancellationToken);
            
            var user = new User
            {
                TenantId = tenant.Id,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = _passwordService.HashPassword(request.Password),
                IsActive = true,
                EmailConfirmed = true
            };

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                // Create default roles for new tenant (if it's new)
                if (!string.IsNullOrEmpty(request.TenantName))
                {
                    using var roleServiceScope = _serviceProvider.CreateScope();
                    var roleTemplateService = roleServiceScope.ServiceProvider.GetRequiredService<IRoleTemplateService>();
                    await roleTemplateService.CreateDefaultRolesForTenantAsync(tenant.Id);

                    // Find the "Tenant Admin" role
                    var tenantAdminRole = await _context.Roles
                        .Where(r => r.TenantId == tenant.Id && r.Name == "Tenant Admin")
                        .FirstOrDefaultAsync(cancellationToken);

                    if (tenantAdminRole != null)
                    {
                        // Create RBAC UserRole assignment
                        var userRole = new UserRole
                        {
                            UserId = user.Id,
                            RoleId = tenantAdminRole.Id,
                            TenantId = tenant.Id,
                            IsActive = true,
                            AssignedAt = DateTime.UtcNow,
                            AssignedBy = "System",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.UserRoles.Add(userRole);
                    }
                }

                // Create TenantUser relationship
                var tenantUser = new TenantUser
                {
                    TenantId = tenant.Id,
                    UserId = user.Id,
                    Role = string.IsNullOrEmpty(request.TenantName) ? "User" : "TenantAdmin", 
                    IsActive = true,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.TenantUsers.Add(tenantUser);

                // Generate tokens
                var accessToken = await _tokenService.GenerateAccessTokenAsync(user, tenant);
                var refreshTokenEntity = await _tokenService.CreateRefreshTokenAsync(user);
                refreshTokenEntity.TenantId = tenant.Id;
                refreshTokenEntity.CreatedByIp = GetClientIpAddress();

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                // Create response
                var userDto = _mapper.Map<DTOs.User.UserDto>(user);
                var tenantDto = _mapper.Map<DTOs.Tenant.TenantDto>(tenant);

                var response = new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenEntity.Token,
                    ExpiresAt = refreshTokenEntity.ExpiryDate,
                    TokenType = "Bearer",
                    User = userDto,
                    Tenant = tenantDto
                };

                _logger.LogInformation("New user {Email} registered successfully for tenant {TenantName}", 
                    request.Email, tenant.Name);
                
                return ApiResponseDto<TokenResponseDto>.SuccessResult(response, 
                    $"Welcome to {tenant.Name}! Your account has been created successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for {Email}", request.Email);
            return ApiResponseDto<TokenResponseDto>.ErrorResult("Registration failed. Please try again.");
        }
    }

    public async Task<ApiResponseDto<TokenResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            // ➕ ADD: Debug logging
            _logger.LogWarning("🔍 LOGIN DEBUG: Starting login for {Email}", request.Email);
            
            // ✅ CRITICAL FIX: Use IgnoreQueryFilters() for login to bypass tenant isolation
            // During login, we don't have tenant context yet, so we need to find the user first
            // ✅ CRITICAL FIX: Load UserRoles collection for RBAC system
            var user = await _context.Users
                .IgnoreQueryFilters() // ✅ BYPASS tenant filtering during login
                .Include(u => u.PrimaryTenant)
                .Include(u => u.TenantUsers)  // Legacy roles (fallback)
                .Include(u => u.UserRoles)    // ✅ ADD: Modern RBAC roles
                    .ThenInclude(ur => ur.Role) // ✅ ADD: Include role details
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            // ➕ ADD: Debug user lookup
            _logger.LogWarning("🔍 LOGIN DEBUG: User found: {UserExists}, IsActive: {IsActive}, TenantId: {TenantId}", 
                user != null, user?.IsActive, user?.TenantId);

            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent user: {Email}", request.Email);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Email}", request.Email);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Account is disabled.");
            }

            if (user.LockedOutUntil.HasValue && user.LockedOutUntil > DateTime.UtcNow)
            {
                _logger.LogWarning("Login attempt for locked user: {Email}", request.Email);
                return ApiResponseDto<TokenResponseDto>.ErrorResult($"Account is locked until {user.LockedOutUntil}.");
            }

            // ➕ ADD: Debug password verification with null safety
            _logger.LogWarning("🔍 LOGIN DEBUG: Attempting password verification for {Email}", request.Email);
            _logger.LogWarning("🔍 LOGIN DEBUG: Password length: {PasswordLength}, Hash starts with: {HashStart}", 
                request.Password?.Length, user.PasswordHash?.Substring(0, Math.Min(10, user.PasswordHash?.Length ?? 0)));

            // FIX: Add null checks before calling VerifyPassword
            if (string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("Login attempt with null password or hash for {Email}", request.Email);
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockedOutUntil = DateTime.UtcNow.AddMinutes(30);
                    _logger.LogWarning("User {Email} locked out due to failed login attempts", request.Email);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Invalid email or password.");
            }

            var passwordVerified = _passwordService.VerifyPassword(request.Password, user.PasswordHash);
            _logger.LogWarning("🔍 LOGIN DEBUG: Password verification result: {PasswordVerified}", passwordVerified);

            if (!passwordVerified)
            {
                // Increment failed login attempts
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockedOutUntil = DateTime.UtcNow.AddMinutes(30);
                    _logger.LogWarning("User {Email} locked out due to failed login attempts", request.Email);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Invalid email or password.");
            }

            // ➕ ADD: Debug tenant lookup
            _logger.LogWarning("🔍 LOGIN DEBUG: Checking tenant relationship. PrimaryTenant: {HasPrimaryTenant}, TenantId: {TenantId}", 
                user.PrimaryTenant != null, user.TenantId);

            // Ensure user has a tenant
            if (user.PrimaryTenant == null && user.TenantId.HasValue)
            {
                user.PrimaryTenant = await _tenantRepository.GetTenantByIdAsync(user.TenantId.Value, cancellationToken);
            }

            if (user.PrimaryTenant == null)
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult("User tenant not found.");
            }

            // Reset failed login attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockedOutUntil = null;
            user.LastLoginAt = DateTime.UtcNow;

            // Generate new tokens
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, user.PrimaryTenant);
            var refreshTokenEntity = await _tokenService.CreateRefreshTokenAsync(user);

            // Set additional properties
            refreshTokenEntity.TenantId = user.TenantId ?? user.PrimaryTenant.Id;
            refreshTokenEntity.CreatedByIp = GetClientIpAddress();

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            // Map to DTOs
            var userDto = _mapper.Map<DTOs.User.UserDto>(user);
            userDto.TenantId = user.TenantId ?? user.PrimaryTenant.Id;
            var tenantDto = _mapper.Map<DTOs.Tenant.TenantDto>(user.PrimaryTenant);

            var response = new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token,
                ExpiresAt = refreshTokenEntity.ExpiryDate,
                TokenType = "Bearer",
                User = userDto,
                Tenant = tenantDto
            };

            _logger.LogInformation("User {Email} logged in successfully", request.Email);
            return ApiResponseDto<TokenResponseDto>.SuccessResult(response, "Login successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return ApiResponseDto<TokenResponseDto>.ErrorResult("Login failed. Please try again.");
        }
    }

    public async Task<ApiResponseDto<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.PrimaryTenant)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                _logger.LogWarning("Invalid refresh token used: {Token}", request.RefreshToken);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Invalid refresh token.");
            }

            var user = refreshToken.User;
            if (user == null || !user.IsActive || user.PrimaryTenant == null)
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult("User not found or inactive.");
            }

            // Revoke the old refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = GetClientIpAddress();

            // Generate new tokens
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, user.PrimaryTenant);
            var newRefreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            // Set new refresh token details
            newRefreshToken.TenantId = user.TenantId ?? user.PrimaryTenant.Id;
            newRefreshToken.CreatedByIp = GetClientIpAddress();
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Map to DTOs
            var userDto = _mapper.Map<DTOs.User.UserDto>(user);
            userDto.TenantId = user.TenantId ?? user.PrimaryTenant.Id;
            var tenantDto = _mapper.Map<DTOs.Tenant.TenantDto>(user.PrimaryTenant);

            var response = new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = newRefreshToken.ExpiryDate,
                TokenType = "Bearer",
                User = userDto,
                Tenant = tenantDto
            };

            _logger.LogInformation("Token refreshed for user {UserId}", user.Id);
            return ApiResponseDto<TokenResponseDto>.SuccessResult(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return ApiResponseDto<TokenResponseDto>.ErrorResult("Token refresh failed. Please login again.");
        }
    }

    public async Task<ApiResponseDto<bool>> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

            if (token != null && !token.IsRevoked)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = GetClientIpAddress();

                await _context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("User logged out successfully");
            return ApiResponseDto<bool>.SuccessResult(true, "Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return ApiResponseDto<bool>.ErrorResult("Logout failed");
        }
    }

    public async Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                return ApiResponseDto<bool>.ErrorResult("User not found.");
            }

            if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return ApiResponseDto<bool>.ErrorResult("Current password is incorrect.");
            }

            user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            // Revoke all refresh tokens to force re-login
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = GetClientIpAddress();
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Password changed for user {UserId}", userId);
            return ApiResponseDto<bool>.SuccessResult(true, "Password changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return ApiResponseDto<bool>.ErrorResult("Password change failed");
        }
    }

    public async Task<ApiResponseDto<bool>> ResetPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null)
            {
                // Don't reveal if user exists for security
                return ApiResponseDto<bool>.SuccessResult(true, "If the email exists, a reset link has been sent.");
            }

            // Generate password reset token
            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // TODO: Send email with reset link
            _logger.LogInformation("Password reset token for {Email}: {Token}", email, resetToken);

            return ApiResponseDto<bool>.SuccessResult(true, "If the email exists, a reset link has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for {Email}", email);
            return ApiResponseDto<bool>.ErrorResult("Password reset failed");
        }
    }

    public async Task<ApiResponseDto<bool>> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(
                u => u.Email == email && u.EmailConfirmationToken == token, cancellationToken);

            if (user == null || user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
            {
                return ApiResponseDto<bool>.ErrorResult("Invalid or expired confirmation token.");
            }

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Email confirmed for user {Email}", email);
            return ApiResponseDto<bool>.SuccessResult(true, "Email confirmed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for {Email}", email);
            return ApiResponseDto<bool>.ErrorResult("Email confirmation failed");
        }
    }

    public async Task<ApiResponseDto<bool>> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                return ApiResponseDto<bool>.ErrorResult("Invalid token");
            }

            return ApiResponseDto<bool>.SuccessResult(true, "Token is valid");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return ApiResponseDto<bool>.ErrorResult("Token validation failed");
        }
    }

    // NEW: RBAC-related methods for Enhanced Phase 4
    public async Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissions = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);
            return permissions.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
            return new List<string>();
        }
    }

    public async Task<List<string>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return new List<string>();
            }

            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId.Value)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync(cancellationToken);

            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return new List<string>();
        }
    }

    // 🔧 NEW: Tenant switching with new JWT issuance
    public async Task<ApiResponseDto<TokenResponseDto>> SwitchTenantAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogWarning("🔍 TENANT SWITCH DEBUG: User {UserId} requesting to switch to tenant {TenantId}", userId, tenantId);

            // Load user with tenant relationships - bypass global query filters
            var user = await _context.Users
                .IgnoreQueryFilters() // Bypass tenant filtering to load user
                .Include(u => u.TenantUsers.Where(tu => tu.IsActive))
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found during tenant switch", userId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("User not found");
            }

            _logger.LogWarning("🔍 TENANT SWITCH DEBUG: User loaded. TenantUsers count: {Count}", user.TenantUsers?.Count ?? 0);

            // Verify user has access to target tenant
            var hasAccess = user.TenantUsers.Any(tu => tu.TenantId == tenantId && tu.IsActive);
            
            _logger.LogWarning("🔍 TENANT SWITCH DEBUG: User has access to tenant {TenantId}: {HasAccess}", tenantId, hasAccess);
            
            if (!hasAccess)
            {
                _logger.LogWarning("User {UserId} attempted to switch to unauthorized tenant {TenantId}", 
                    userId, tenantId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Access denied to tenant");
            }

            // Load target tenant
            _logger.LogWarning("🔍 TENANT SWITCH DEBUG: Loading tenant {TenantId} from repository", tenantId);
            var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId, cancellationToken);
            
            if (tenant == null)
            {
                _logger.LogWarning("🔍 TENANT SWITCH DEBUG: Tenant {TenantId} not found in repository", tenantId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant not found");
            }

            _logger.LogWarning("🔍 TENANT SWITCH DEBUG: Tenant loaded - ID: {ActualId}, Name: {Name}, IsActive: {IsActive}", 
                tenant.Id, tenant.Name, tenant.IsActive);

            if (!tenant.IsActive)
            {
                _logger.LogWarning("Tenant {TenantId} is inactive", tenantId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant not found or inactive");
            }

            // Revoke existing refresh tokens for security (optional but recommended)
            var existingTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var existingToken in existingTokens)
            {
                existingToken.IsRevoked = true;
                existingToken.RevokedAt = DateTime.UtcNow;
                existingToken.RevokedByIp = GetClientIpAddress();
            }

            // 🔧 CRITICAL DEBUG: Log what we're passing to token generation
            _logger.LogWarning("🔍 TENANT SWITCH DEBUG: About to generate token with - User ID: {UserId}, Tenant ID: {TenantId}, Tenant Name: {TenantName}", 
                user.Id, tenant.Id, tenant.Name);

            // Generate new JWT for the target tenant with tenant-specific permissions
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, tenant);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);
            
            // Set tenant context for refresh token
            refreshToken.TenantId = tenant.Id;
            refreshToken.CreatedByIp = GetClientIpAddress();

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Create response DTOs
            var userDto = _mapper.Map<DTOs.User.UserDto>(user);
            userDto.TenantId = tenant.Id; // Ensure correct tenant ID in response

            var tenantDto = _mapper.Map<DTOs.Tenant.TenantDto>(tenant);

            _logger.LogWarning("🔍 TENANT SWITCH DEBUG: Response DTOs created - UserDto.TenantId: {UserTenantId}, TenantDto.Id: {TenantDtoId}, TenantDto.Name: {TenantDtoName}", 
                userDto.TenantId, tenantDto.Id, tenantDto.Name);

            var response = new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiryDate,
                TokenType = "Bearer",
                User = userDto,
                Tenant = tenantDto
            };

            _logger.LogInformation("User {UserId} successfully switched to tenant {TenantId} ({TenantName})", 
                userId, tenant.Id, tenant.Name);

            return ApiResponseDto<TokenResponseDto>.SuccessResult(response, 
                $"Successfully switched to {tenant.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error switching tenant for user {UserId} to tenant {TenantId}", 
                userId, tenantId);
            return ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant switch failed");
        }
    }

    // Helper methods - FIXED: Properly implemented async method
    private async Task<Tenant> GetTenantForRegistrationAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.TenantName))
        {
            var existingTenant = await _tenantRepository.GetTenantByNameAsync(request.TenantName, cancellationToken);

            if (existingTenant != null)
            {
                return existingTenant;
            }

            var newTenant = new Tenant
            {
                Name = request.TenantName,
                Domain = request.TenantDomain,
                SubscriptionPlan = "Basic",
                IsActive = true,
                Settings = "{}"
            };

            return await _tenantRepository.CreateTenantAsync(newTenant, cancellationToken);
        }

        // Get existing tenant or create default
        var allTenants = await _tenantRepository.GetAllTenantsAsync(cancellationToken);
        var firstTenant = allTenants.FirstOrDefault();

        if (firstTenant != null)
        {
            return firstTenant;
        }

        // Create default tenant if none exists
        var defaultTenant = new Tenant
        {
            Name = "Default",
            SubscriptionPlan = "Basic",
            IsActive = true,
            Settings = "{}"
        };

        return await _tenantRepository.CreateTenantAsync(defaultTenant, cancellationToken);
    }

    private string GetClientIpAddress()
    {
        // This would normally get the client IP from HttpContext
        // For now, return a placeholder
        return "127.0.0.1"; // TODO: Implement proper IP resolution with IHttpContextAccessor
    }
}
