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
    // NEW: Add missing dependencies for Enhanced Phase 4
    private readonly IPermissionService _permissionService;
    private readonly ITenantProvider _tenantProvider;

    public AuthServiceImplementation(
        ApplicationDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<AuthServiceImplementation> logger,
        JwtSettings jwtSettings,
        ITenantManagementRepository tenantRepository,
        // NEW: Add missing constructor parameters
        IPermissionService permissionService,
        ITenantProvider tenantProvider)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
        _jwtSettings = jwtSettings;
        _tenantRepository = tenantRepository;
        // NEW: Initialize missing dependencies
        _permissionService = permissionService;
        _tenantProvider = tenantProvider;
    }

    public async Task<ApiResponseDto<TokenResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult("User with this email already exists.");
            }

            // Handle tenant - for now, create a default tenant or use an existing one
            var tenant = await GetTenantForRegistrationAsync(request, cancellationToken);

            // Create user
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

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // Create tenant-user relationship
            var tenantUser = new TenantUser
            {
                TenantId = tenant.Id,
                UserId = user.Id,
                Role = "User",
                IsActive = true
            };

            _context.TenantUsers.Add(tenantUser);

            // Generate tokens
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, tenant);
            var refreshTokenEntity = await _tokenService.CreateRefreshTokenAsync(user);

            // Set additional properties for RefreshToken
            refreshTokenEntity.TenantId = tenant.Id;
            refreshTokenEntity.CreatedByIp = GetClientIpAddress();

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            // Map to DTOs
            var userDto = _mapper.Map<DTOs.User.UserDto>(user);
            userDto.TenantId = tenant.Id; // Ensure TenantId is set
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

            _logger.LogInformation("User {Email} registered successfully", request.Email);
            return ApiResponseDto<TokenResponseDto>.SuccessResult(response, "Registration successful");
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
            
            var user = await _context.Users
                .Include(u => u.PrimaryTenant)
                .Include(u => u.TenantUsers)  // ➕ ADD: Load role information
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
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
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Email == email && u.EmailConfirmationToken == token,
                cancellationToken);

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

    // NEW: Enhanced Phase 4 RBAC methods
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
