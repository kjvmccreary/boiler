// FILE: src services/AuthService/Services/AuthService.cs
using AutoMapper;
using Common.Configuration;
using Common.Data;
using Contracts.Auth;
using Contracts.Repositories; 
using Contracts.Services;
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
    private readonly IServiceProvider _serviceProvider;

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
        IServiceProvider serviceProvider)
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
        _serviceProvider = serviceProvider;
    }

    public async Task<ApiResponseDto<TokenResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting registration for {Email}", request.Email);

            var existingUser = await _context.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult(
                    "An account with this email already exists. Please sign in and create additional organizations from your dashboard."
                );
            }

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            try
            {
                _logger.LogInformation("Getting tenant for registration");

                // Get or create tenant
                var tenant = await GetTenantForRegistrationAsync(request, cancellationToken);
                
                _logger.LogInformation("Tenant obtained - ID: {TenantId}, Name: {TenantName}", 
                    tenant.Id, tenant.Name);

                // Save tenant first to get the ID
                if (tenant.Id == 0)
                {
                    _logger.LogInformation("Saving new tenant to get ID");
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Tenant saved with ID: {TenantId}", tenant.Id);
                }

                _logger.LogInformation("Creating user");
                
                var user = new User
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PasswordHash = _passwordService.HashPassword(request.Password),
                    IsActive = true,
                    EmailConfirmed = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User created with ID: {UserId}", user.Id);

                // Create default roles for new tenant (if it's new)
                if (!string.IsNullOrEmpty(request.TenantName))
                {
                    _logger.LogInformation("Creating Tenant Admin role with full permissions for new user");

                    try
                    {
                        // 🔧 USE COMPREHENSIVE METHOD: Full permissions including role management
                        await CreateTenantAdminRoleAndAssignToUserAsync(tenant.Id, user.Id, cancellationToken);
                        _logger.LogInformation("Tenant Admin role created and assigned successfully with full permissions");
                    }
                    catch (Exception roleEx)
                    {
                        _logger.LogError(roleEx, "FAILED to create Tenant Admin role - continuing without admin permissions");
                        // Don't throw - let registration continue even if role creation fails
                    }
                }

                _logger.LogInformation("Creating TenantUser relationship");

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

                _logger.LogInformation("Generating tokens");

                // Generate tokens
                var accessToken = await _tokenService.GenerateAccessTokenAsync(user, tenant);
                var refreshTokenEntity = await _tokenService.CreateRefreshTokenAsync(user);
                refreshTokenEntity.TenantId = tenant.Id;
                refreshTokenEntity.CreatedByIp = GetClientIpAddress();

                _context.RefreshTokens.Add(refreshTokenEntity);
                
                _logger.LogInformation("Saving final changes");
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Committing transaction");
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Creating response DTOs");

                // Create response
                var userDto = _mapper.Map<DTOs.User.UserDto>(user);
                userDto.TenantId = tenant.Id;
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

                _logger.LogInformation("User {Email} registered successfully for tenant {TenantName}", 
                    request.Email, tenant.Name);
                
                return ApiResponseDto<TokenResponseDto>.SuccessResult(response, 
                    $"Welcome to {tenant.Name}! Your account has been created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in registration transaction, rolling back - {Message}", ex.Message);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed: {Message}", ex.Message);
            return ApiResponseDto<TokenResponseDto>.ErrorResult("Registration failed. Please try again.");
        }
    }

    public async Task<ApiResponseDto<TokenResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting login for {Email}", request.Email);
            
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.TenantUsers)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            _logger.LogInformation("User found: {UserExists}, IsActive: {IsActive}", 
                user != null, user?.IsActive);

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

            // Validate password
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
            _logger.LogInformation("Password verification result: {PasswordVerified}", passwordVerified);

            if (!passwordVerified)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockedOutUntil = DateTime.UtcNow.AddMinutes(30);
                    _logger.LogWarning("User {Email} locked out due to failed login attempts", request.Email);
                }
                await _context.SaveChangesAsync(cancellationToken);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Invalid email or password.");
            }

            // Reset failed login attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockedOutUntil = null;
            user.LastLoginAt = DateTime.UtcNow;

            // Generate JWT WITHOUT tenant initially for tenant selection
            _logger.LogInformation("Generating tenant-less JWT for initial login");
            var accessToken = await _tokenService.GenerateAccessTokenWithoutTenantAsync(user);
            var refreshTokenEntity = await _tokenService.CreateRefreshTokenAsync(user);

            refreshTokenEntity.TenantId = null; // No tenant context yet
            refreshTokenEntity.CreatedByIp = GetClientIpAddress();

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<DTOs.User.UserDto>(user);

            var response = new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token,
                ExpiresAt = refreshTokenEntity.ExpiryDate,
                TokenType = "Bearer",
                User = userDto,
                Tenant = null // No tenant in initial login response
            };

            _logger.LogInformation("User {Email} logged in successfully (no tenant selected yet)", request.Email);
            return ApiResponseDto<TokenResponseDto>.SuccessResult(response, "Login successful - please select tenant");
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
                    .ThenInclude(u => u.TenantUsers)
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                _logger.LogWarning("Invalid refresh token used: {Token}", request.RefreshToken);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Invalid refresh token.");
            }

            var user = refreshToken.User;
            if (user == null || !user.IsActive)
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult("User not found or inactive.");
            }

            if (!refreshToken.TenantId.HasValue)
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult("No tenant context available for refresh.");
            }

            var tenant = await _tenantRepository.GetTenantByIdAsync(refreshToken.TenantId.Value, cancellationToken);
            if (tenant == null)
            {
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant not found or inactive.");
            }

            // Revoke the old refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = GetClientIpAddress();

            // Generate new tokens
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, tenant);
            var newRefreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            newRefreshToken.TenantId = tenant.Id;
            newRefreshToken.CreatedByIp = GetClientIpAddress();
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            var userDto = _mapper.Map<DTOs.User.UserDto>(user);
            userDto.TenantId = tenant.Id;
            var tenantDto = _mapper.Map<DTOs.Tenant.TenantDto>(tenant);

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
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            
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
                return ApiResponseDto<bool>.SuccessResult(true, "If the email exists, a reset link has been sent.");
            }

            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

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
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId.Value && ur.IsActive)
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
            _logger.LogInformation("User {UserId} requesting to switch to tenant {TenantId}", userId, tenantId);

            // Load user with tenant relationships - bypass global query filters
            var user = await _context.Users
                .IgnoreQueryFilters() // Bypass tenant filtering to load user
                .Include(u => u.TenantUsers!.Where(tu => tu.IsActive))
                .Include(u => u.UserRoles!.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found during tenant switch", userId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("User not found");
            }

            _logger.LogWarning("User loaded. TenantUsers count: {Count}", user.TenantUsers?.Count ?? 0);

            // Verify user has access to target tenant
            var hasAccess = user.TenantUsers?.Any(tu => tu.TenantId == tenantId && tu.IsActive) ?? false;
            
            _logger.LogWarning("User has access to tenant {TenantId}: {HasAccess}", tenantId, hasAccess);
            
            if (!hasAccess)
            {
                _logger.LogWarning("User {UserId} attempted to switch to unauthorized tenant {TenantId}", 
                    userId, tenantId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Access denied to tenant");
            }

            // Load target tenant
            _logger.LogWarning("Loading tenant {TenantId} from repository", tenantId);
            var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId, cancellationToken);
            
            if (tenant == null)
            {
                _logger.LogWarning("Tenant {TenantId} not found in repository", tenantId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant not found");
            }

            _logger.LogWarning("Tenant loaded - ID: {ActualId}, Name: {Name}, IsActive: {IsActive}", 
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
            _logger.LogWarning("About to generate token with - User ID: {UserId}, Tenant ID: {TenantId}, Tenant Name: {TenantName}", 
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

            _logger.LogWarning("Response DTOs created - UserDto.TenantId: {UserTenantId}, TenantDto.Id: {TenantDtoId}, TenantDto.Name: {TenantDtoName}", 
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

    public async Task<ApiResponseDto<TokenResponseDto>> SelectTenantAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("User {UserId} selecting tenant {TenantId}", userId, tenantId);

            // Load user and verify tenant access
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.TenantUsers!.Where(tu => tu.IsActive))
                .Include(u => u.UserRoles!.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found during tenant selection", userId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("User not found");
            }

            // Verify access to tenant
            var hasAccess = user.TenantUsers?.Any(tu => tu.TenantId == tenantId && tu.IsActive) ?? false;
            if (!hasAccess)
            {
                _logger.LogWarning("User {UserId} denied access to tenant {TenantId}", userId, tenantId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Access denied to tenant");
            }

            // Load tenant
            var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId, cancellationToken);
            if (tenant == null || !tenant.IsActive)
            {
                _logger.LogWarning("Tenant {TenantId} not found or inactive", tenantId);
                return ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant not found or inactive");
            }

            _logger.LogInformation("Generating full JWT with tenant context");
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user, tenant);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            refreshToken.TenantId = tenant.Id;
            refreshToken.CreatedByIp = GetClientIpAddress();

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Return full response with tenant
            var userDto = _mapper.Map<DTOs.User.UserDto>(user);
            userDto.TenantId = tenant.Id;
            var tenantDto = _mapper.Map<DTOs.Tenant.TenantDto>(tenant);

            var response = new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiryDate,
                TokenType = "Bearer",
                User = userDto,
                Tenant = tenantDto
            };

            _logger.LogInformation("User {UserId} successfully selected tenant {TenantId} ({TenantName})", 
                userId, tenant.Id, tenant.Name);

            return ApiResponseDto<TokenResponseDto>.SuccessResult(response, $"Welcome to {tenant.Name}!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting tenant {TenantId} for user {UserId}", tenantId, userId);
            return ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant selection failed");
        }
    }

    // Helper methods
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
                Settings = "{}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(newTenant);
            return newTenant;
        }

        var allTenants = await _tenantRepository.GetAllTenantsAsync(cancellationToken);
        var firstTenant = allTenants.FirstOrDefault();

        if (firstTenant != null)
        {
            return firstTenant;
        }

        var defaultTenant = new Tenant
        {
            Name = "Default",
            SubscriptionPlan = "Basic",
            IsActive = true,
            Settings = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(defaultTenant);
        return defaultTenant;
    }

    // 🔧 COMPREHENSIVE METHOD: Creates Tenant Admin role with ALL 14 permissions
    private async Task CreateTenantAdminRoleAndAssignToUserAsync(int tenantId, int userId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating comprehensive Tenant Admin role for tenant {TenantId}", tenantId);

            // Check if Tenant Admin role already exists for this tenant
            var existingRole = await _context.Roles
                .Where(r => r.TenantId == tenantId && r.Name == "Tenant Admin")
                .FirstOrDefaultAsync(cancellationToken);

            if (existingRole != null)
            {
                _logger.LogInformation("Tenant Admin role already exists with ID: {RoleId}", existingRole.Id);
                
                // Just assign the existing role to the user
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = existingRole.Id,
                    TenantId = tenantId,
                    IsActive = true,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.UserRoles.Add(userRole);
                return;
            }

            // Create the Tenant Admin role
            var tenantAdminRole = new Role
            {
                Name = "Tenant Admin",
                Description = "Full administrative access to the tenant",
                TenantId = tenantId,
                IsSystemRole = false,
                IsDefault = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(tenantAdminRole);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tenant Admin role created with ID: {RoleId}", tenantAdminRole.Id);

            // 🔧 COMPREHENSIVE: All admin permissions including COMPLETE role management (MATCHING TEST SEEDER)
            var adminPermissions = new[]
            {
                // Users permissions - COMPLETE SET (matching test seeder)
                "users.view", "users.edit", "users.create", "users.delete", 
                "users.view_all",           // ← 🆕 CRITICAL: Missing from our list!
                "users.manage_roles",       // ← Already have this

                // Roles permissions - COMPLETE SET (matching test seeder)
                "roles.view", "roles.edit", "roles.create", "roles.delete",
                "roles.assign_users",       // ← Already have this
                "roles.manage_permissions", // ← Already have this

                // Tenants permissions - COMPLETE SET (matching test seeder)  
                "tenants.view", "tenants.edit",
                "tenants.create",           // ← 🆕 ADD: Missing from our list
                "tenants.delete",           // ← 🆕 ADD: Missing from our list
                "tenants.initialize",       // ← 🆕 ADD: Missing from our list
                "tenants.view_all",         // ← 🆕 ADD: Missing from our list
                "tenants.manage_settings",  // ← 🆕 ADD: Missing from our list

                // Reports permissions - COMPLETE SET (matching test seeder)
                "reports.view", "reports.export",
                "reports.create",           // ← 🆕 ADD: Missing from our list
                "reports.schedule",         // ← 🆕 ADD: Missing from our list

                // Permissions management - COMPLETE SET (matching test seeder)
                "permissions.view", "permissions.assign",
                "permissions.create",       // ← 🆕 ADD: Missing from our list  
                "permissions.edit",         // ← 🆕 ADD: Missing from our list
                "permissions.delete",       // ← 🆕 ADD: Missing from our list
                "permissions.manage",       // ← 🆕 ADD: Missing from our list

                // Settings permissions
                "settings.view", "settings.edit",
                "audit.view"
            };

            // Use the comprehensive permission creation method
            await EnsurePermissionsExistAndAssignToRoleAsync(tenantAdminRole.Id, adminPermissions, cancellationToken);

            // Assign the role to the user
            var userRoleAssignment = new UserRole
            {
                UserId = userId,
                RoleId = tenantAdminRole.Id,
                TenantId = tenantId,
                IsActive = true,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserRoles.Add(userRoleAssignment);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tenant Admin role created and assigned to user {UserId} with {PermissionCount} permissions", 
                userId, adminPermissions.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comprehensive Tenant Admin role for tenant {TenantId} and user {UserId}", tenantId, userId);
            throw;
        }
    }

    private async Task EnsurePermissionsExistAndAssignToRoleAsync(int roleId, string[] permissionNames, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 PERMISSION DEBUG: Starting permission creation for role {RoleId} with {RequestedCount} permissions", roleId, permissionNames.Length);
        
        // Get existing permissions
        var existingPermissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name))
            .ToListAsync(cancellationToken);

        var existingPermissionNames = existingPermissions.Select(p => p.Name).ToHashSet();
        
        _logger.LogInformation("🔍 PERMISSION DEBUG: Found {ExistingCount} existing permissions: {ExistingPermissions}", 
            existingPermissions.Count, string.Join(", ", existingPermissionNames));

        // Create missing permissions
        var missingPermissions = permissionNames
            .Where(name => !existingPermissionNames.Contains(name))
            .ToList();

        _logger.LogInformation("🔍 PERMISSION DEBUG: Need to create {MissingCount} missing permissions: {MissingPermissions}", 
            missingPermissions.Count, string.Join(", ", missingPermissions));

        foreach (var permissionName in missingPermissions)
        {
            var permission = new Permission
            {
                Name = permissionName,
                Description = GetPermissionDescription(permissionName),
                Category = GetPermissionCategory(permissionName),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _context.Permissions.Add(permission);
            existingPermissions.Add(permission);
            _logger.LogInformation("🔍 PERMISSION DEBUG: Added new permission: {PermissionName}", permissionName);
        }

        if (missingPermissions.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("🔍 PERMISSION DEBUG: Saved {CreatedCount} new permissions to database", missingPermissions.Count);
        }

        _logger.LogInformation("🔍 PERMISSION DEBUG: Total permissions to assign: {TotalCount}", existingPermissions.Count);

        // Assign all permissions to the role
        foreach (var permission in existingPermissions)
        {
            // Check if this role-permission combination already exists
            var existingRolePermission = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingRolePermission == null)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.RolePermissions.Add(rolePermission);
                _logger.LogInformation("🔍 PERMISSION DEBUG: Added role permission: Role {RoleId} -> Permission {PermissionName}", roleId, permission.Name);
            }
            else
            {
                _logger.LogInformation("🔍 PERMISSION DEBUG: Role permission already exists: Role {RoleId} -> Permission {PermissionName}", roleId, permission.Name);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("🔍 PERMISSION DEBUG: Successfully assigned {PermissionCount} permissions to role {RoleId}", existingPermissions.Count, roleId);
    }

    private static string GetPermissionDescription(string permissionName)
    {
        return permissionName switch
        {
            "users.view" => "View users",
            "users.edit" => "Edit users", 
            "users.create" => "Create users",
            "users.delete" => "Delete users",
            "users.view_all" => "View all users across tenants",      // ← 🆕 ADD
            "users.manage_roles" => "Manage user roles",              // ← Already have this
            
            "roles.view" => "View roles",
            "roles.edit" => "Edit roles",
            "roles.create" => "Create roles", 
            "roles.delete" => "Delete roles",
            "roles.assign_users" => "Assign roles to users",
            "roles.manage_permissions" => "Manage role permissions",
            
            "tenants.view" => "View tenant information",
            "tenants.edit" => "Edit tenant settings",
            "tenants.create" => "Create new tenants",                 // ← 🆕 ADD
            "tenants.delete" => "Delete tenants",                     // ← 🆕 ADD
            "tenants.initialize" => "Initialize tenant settings",     // ← 🆕 ADD
            "tenants.view_all" => "View all tenants",                 // ← 🆕 ADD
            "tenants.manage_settings" => "Manage tenant settings",    // ← 🆕 ADD
            
            "reports.view" => "View reports",
            "reports.create" => "Create reports",                     // ← 🆕 ADD
            "reports.export" => "Export reports",
            "reports.schedule" => "Schedule reports",                 // ← 🆕 ADD
            
            "permissions.view" => "View permissions",
            "permissions.assign" => "Assign permissions",
            "permissions.create" => "Create permissions",             // ← 🆕 ADD
            "permissions.edit" => "Edit permissions",                 // ← 🆕 ADD
            "permissions.delete" => "Delete permissions",             // ← 🆕 ADD
            "permissions.manage" => "Manage all permissions",         // ← 🆕 ADD
            
            "settings.view" => "View settings",
            "settings.edit" => "Edit settings",
            "audit.view" => "View audit logs",
            "profile.view" => "View own profile",
            "profile.edit" => "Edit own profile",
            "dashboard.view" => "View dashboard",
            _ => $"Permission: {permissionName}"
        };
    }

    private static string GetPermissionCategory(string permissionName)
    {
        var parts = permissionName.Split('.');
        return parts.Length > 0 ? char.ToUpper(parts[0][0]) + parts[0][1..] : "General";
    }

    private string GetClientIpAddress()
    {
        return "127.0.0.1"; // TODO: Implement proper IP resolution with IHttpContextAccessor
    }
}
