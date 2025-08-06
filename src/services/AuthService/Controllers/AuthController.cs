// FILE: src/services/AuthService/Controllers/AuthController.cs
using System.Security.Claims;
using Contracts.Auth;
using DTOs.Auth;
using DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
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
    public ActionResult<ApiResponseDto<bool>> ValidateToken() // FIXED: Removed async since no await
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
    public ActionResult<ApiResponseDto<object>> GetCurrentUser() // FIXED: Removed async since no await
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
}
