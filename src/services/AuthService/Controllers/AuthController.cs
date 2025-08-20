// FILE: src/services/AuthService/Controllers/AuthController.cs
using System.Security.Claims;
using Contracts.Auth;
using Contracts.Services;
using DTOs.Auth;
using DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Data;
using Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IPasswordService _passwordService;
    private readonly IServiceProvider _serviceProvider;

    public AuthController(
        IAuthService authService, 
        ILogger<AuthController> logger,
        IPasswordService passwordService,
        IServiceProvider serviceProvider)
    {
        _authService = authService;
        _logger = logger;
        _passwordService = passwordService;
        _serviceProvider = serviceProvider;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Register endpoint");
            return StatusCode(500, ApiResponseDto<TokenResponseDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Login endpoint");
            return StatusCode(500, ApiResponseDto<TokenResponseDto>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Switch to a different tenant and issue new JWT tokens
    /// </summary>
    [HttpPost("switch-tenant")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> SwitchTenant(
        [FromBody] SwitchTenantDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.SwitchTenantAsync(userId, request.TenantId, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error switching tenant for user {UserId} to tenant {TenantId}", 
                GetCurrentUserId(), request.TenantId);
            return StatusCode(500, ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant switch failed"));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> RefreshToken(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RefreshToken endpoint");
            return StatusCode(500, ApiResponseDto<TokenResponseDto>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<bool>>> Logout(
        [FromBody] LogoutRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                _logger.LogWarning("Logout attempt with missing refresh token");
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Refresh token is required"));
            }

            var result = await _authService.LogoutAsync(request.RefreshToken, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Logout endpoint");
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword(
        [FromBody] ChangePasswordDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ChangePassword endpoint");
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ResetPassword(
        [FromBody] ResetPasswordRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(request.Email, cancellationToken);
            return Ok(result); // Always return success for security
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ResetPassword endpoint");
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Internal server error"));
        }
    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ConfirmEmail(
        [FromBody] ConfirmEmailRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _authService.ConfirmEmailAsync(request.Email, request.Token, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ConfirmEmail endpoint");
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("validate-token")]
    [Authorize]
    public ActionResult<ApiResponseDto<bool>> ValidateToken()
    {
        try
        {
            // If we reach here, the token is valid (passed [Authorize])
            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Token is valid"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ValidateToken endpoint");
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponseDto<object>> GetCurrentUser()
    {
        try
        {
            var userInfo = new
            {
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                FirstName = User.FindFirst(ClaimTypes.GivenName)?.Value,
                LastName = User.FindFirst(ClaimTypes.Surname)?.Value,
                TenantId = User.FindFirst("tenant_id")?.Value,
                TenantName = User.FindFirst("tenant_name")?.Value,
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
                Permissions = User.FindAll("permission").Select(c => c.Value).ToList()
            };

            return Ok(ApiResponseDto<object>.SuccessResult(userInfo, "User info retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCurrentUser endpoint");
            return StatusCode(500, ApiResponseDto<object>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("permissions")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetMyPermissions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var permissions = await _authService.GetUserPermissionsAsync(userId);
            return Ok(ApiResponseDto<List<string>>.SuccessResult(permissions, "Permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions");
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("Internal server error"));
        }
    }

    [HttpGet("roles")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetMyRoles()
    {
        try
        {
            var userId = GetCurrentUserId();
            var roles = await _authService.GetUserRolesAsync(userId);
            return Ok(ApiResponseDto<List<string>>.SuccessResult(roles, "Roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles");
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("Internal server error"));
        }
    }

    /// <summary>
    /// Complete login by selecting tenant and issuing full JWT
    /// </summary>
    [HttpPost("select-tenant")]
    [Authorize] // User must have basic JWT token
    public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> SelectTenant(
        [FromBody] SelectTenantDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.SelectTenantAsync(userId, request.TenantId, cancellationToken);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting tenant for user {UserId}", GetCurrentUserId());
            return StatusCode(500, ApiResponseDto<TokenResponseDto>.ErrorResult("Tenant selection failed"));
        }
    }

    #region Helper Methods

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("User ID not found in token");
    }

    #endregion

    #region Debug Endpoints (Remove in Production)

    [HttpGet("debug/generate-hash/{password}")]
    public ActionResult<string> GenerateHash(string password)
    {
        try
        {
            var hash = _passwordService.HashPassword(password);
            return Ok(new { Password = password, Hash = hash, Message = "Hash generated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("debug/logout-format")]
    public ActionResult<object> GetLogoutFormat()
    {
        return Ok(new
        {
            Message = "Logout endpoint expects POST with JSON body",
            ExpectedContentType = "application/json",
            ExpectedBody = new
            {
                refreshToken = "your-refresh-token-here"
            },
            ExampleCurl = "curl -X POST http://localhost:7000/api/auth/logout -H \"Content-Type: application/json\" -H \"Authorization: Bearer YOUR_JWT_TOKEN\" -d '{\"refreshToken\":\"YOUR_REFRESH_TOKEN\"}'"
        });
    }

    [HttpPost("debug/logout-test")]
    public ActionResult<object> LogoutTest([FromBody] LogoutRequestDto? request)
    {
        try
        {
            _logger.LogWarning("🔍 LOGOUT TEST: Content-Type: {ContentType}", Request.ContentType);
            _logger.LogWarning("🔍 LOGOUT TEST: Raw body exists: {HasBody}", Request.Body != null);
            _logger.LogWarning("🔍 LOGOUT TEST: Request object: {Request}", request);
            _logger.LogWarning("🔍 LOGOUT TEST: RefreshToken: {RefreshToken}", request?.RefreshToken);

            return Ok(new
            {
                Success = true,
                ReceivedContentType = Request.ContentType,
                ReceivedRefreshToken = request?.RefreshToken,
                Message = request?.RefreshToken != null ? "Request format is correct!" : "Missing refresh token in request body"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in logout test endpoint");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("debug/tenant-info/{tenantId}")]
    [Authorize]
    public async Task<ActionResult> GetTenantDebugInfo(int tenantId)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            // 🔧 FIX: Use service provider to get dependencies
            using var scope = _serviceProvider.CreateScope();
            var tenantRepository = scope.ServiceProvider.GetRequiredService<ITenantManagementRepository>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Direct repository access to see what's being returned
            var tenant = await tenantRepository.GetTenantByIdAsync(tenantId);
            
            // Check user access
            var userAccess = await context.TenantUsers
                .Where(tu => tu.UserId == userId && tu.TenantId == tenantId && tu.IsActive)
                .Select(tu => new { tu.TenantId, tu.Role, tu.IsActive })
                .FirstOrDefaultAsync();

            // Get all tenants to compare
            var allTenants = await context.Tenants
                .Select(t => new { t.Id, t.Name, t.IsActive })
                .ToListAsync();
                
            // Get all user's tenant access
            var allUserAccess = await context.TenantUsers
                .Where(tu => tu.UserId == userId && tu.IsActive)
                .Include(tu => tu.Tenant)
                .Select(tu => new { tu.TenantId, tu.Role, TenantName = tu.Tenant.Name })
                .ToListAsync();

            return Ok(new
            {
                RequestedTenantId = tenantId,
                FoundTenant = tenant != null ? new { tenant.Id, tenant.Name, tenant.IsActive } : null,
                UserAccessToRequestedTenant = userAccess,
                AllTenants = allTenants,
                AllUserTenantAccess = allUserAccess,
                CurrentUserId = userId,
                DatabaseLooksCorrect = tenant?.Id == tenantId && tenant?.Name != null,
                ExpectedForTenant6 = "Should return 'My Number Two' if database is correct"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                Error = ex.Message, 
                Stack = ex.StackTrace,
                Type = ex.GetType().Name 
            });
        }
    }

    #endregion
}
