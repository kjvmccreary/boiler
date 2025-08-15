// FILE: src/services/AuthService/Controllers/AuthController.cs
using System.Security.Claims;
using Contracts.Auth;
using Contracts.Services; // ✅ ADD: For IPasswordService from shared contracts
using DTOs.Auth;
using DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// using AuthService.Services; // ❌ REMOVE: Don't use local namespace for IPasswordService

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IPasswordService _passwordService; // ✅ This will now use Contracts.Services.IPasswordService

    public AuthController(
        IAuthService authService, 
        ILogger<AuthController> logger,
        IPasswordService passwordService) // ➕ ADD: Inject password service
    {
        _authService = authService;
        _logger = logger;
        _passwordService = passwordService; // ➕ ADD: Assign password service
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

    // Add these endpoints to the existing AuthController

    [HttpGet("permissions")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetMyPermissions()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(ApiResponseDto<List<string>>.ErrorResult("Invalid user"));
            }

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(ApiResponseDto<List<string>>.ErrorResult("Invalid user"));
            }

            var roles = await _authService.GetUserRolesAsync(userId);
            return Ok(ApiResponseDto<List<string>>.SuccessResult(roles, "Roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles");
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("Internal server error"));
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
            // 🔍 DEBUG: Log what we received
            _logger.LogWarning("🔍 LOGOUT DEBUG: Received request");
            _logger.LogWarning("🔍 LOGOUT DEBUG: Content-Type: {ContentType}", Request.ContentType);
            _logger.LogWarning("🔍 LOGOUT DEBUG: Request body (RefreshToken): {RefreshToken}", request?.RefreshToken);
            
            if (request == null || string.IsNullOrEmpty(request.RefreshToken))
            {
                _logger.LogWarning("🚨 LOGOUT ERROR: Invalid request - missing refresh token");
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Invalid user"));
            }

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
                TenantName = User.FindFirst("tenant_name")?.Value
            };

            return Ok(ApiResponseDto<object>.SuccessResult(userInfo, "User info retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCurrentUser endpoint");
            return StatusCode(500, ApiResponseDto<object>.ErrorResult("Internal server error"));
        }
    }

    // ➕ ADD: Temporary hash generation endpoint (REMOVE after testing)
    [HttpGet("debug/generate-hash/{password}")]
    public ActionResult<string> GenerateHash(string password)
    {
        try
        {
            var hash = _passwordService.HashPassword(password); // ✅ NOW WORKS!
            return Ok(new { Password = password, Hash = hash, Message = "Hash generated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // 🔍 ADD: Debug endpoint to understand logout request format
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

    // 🔍 ADD: Simple logout test endpoint that doesn't require auth for debugging
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
}
