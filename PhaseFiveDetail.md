# Phase 5: User Service - Complete Implementation Guide

## Executive Summary

**Phase**: 5 of 12  
**Duration**: 1-2 sessions  
**Complexity**: Medium  
**Focus**: User Management & Profile Operations  
**Prerequisites**: Phase 4 (Authentication Service) - ✅ **100% COMPLETE**

---

## 🎯 Objectives

### Primary Goals
- Implement comprehensive user management functionality
- Create user profile management endpoints
- Set up user-tenant relationship management
- Establish role-based authorization framework
- Ensure tenant-scoped data isolation

### Business Value
- **User Management**: Complete CRUD operations for user administration
- **Profile Management**: Self-service user profile updates
- **Authorization Framework**: Foundation for role-based permissions
- **Multi-Tenant Security**: Enforce tenant boundaries in user operations

---

## 📋 Deliverables Overview

### 1. User Controller (5 Required Endpoints)
- `GET /api/users/profile` - Get current user profile
- `PUT /api/users/profile` - Update user profile
- `GET /api/users` - List users (tenant-scoped, admin only)
- `GET /api/users/{id}` - Get specific user (with permissions)
- `DELETE /api/users/{id}` - Delete user (admin only)

### 2. User Services (Business Logic Layer)
- `IUserService` & `UserService` - Core user operations
- `IUserProfileService` & `UserProfileService` - Profile management

### 3. Authorization Setup (Security Framework)
- Role-based authorization policies
- Tenant-scoped access control
- Permission-based action filtering

### 4. Integration Tests (Quality Assurance)
- User CRUD operations testing
- Authorization scenario validation
- Tenant isolation verification

---

## 🏗️ Detailed Implementation Specifications

### 1. User Controller Implementation

#### File Location
`src/services/UserService/Controllers/UsersController.cs`

#### Complete Controller Template
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Contracts.Services;
using DTOs.Common;
using DTOs.Users;
using System.Security.Claims;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IUserProfileService userProfileService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _userProfileService = userProfileService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's profile information
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponseDto<UserProfileDto>>> GetCurrentUserProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var userProfile = await _userProfileService.GetUserProfileAsync(userId);
            
            if (userProfile == null)
            {
                return NotFound(ApiResponseDto<UserProfileDto>.Failure("User profile not found"));
            }

            return Ok(ApiResponseDto<UserProfileDto>.Success(userProfile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile for user {UserId}", GetCurrentUserId());
            return StatusCode(500, ApiResponseDto<UserProfileDto>.Failure("An error occurred while retrieving your profile"));
        }
    }

    /// <summary>
    /// Update current user's profile information
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponseDto<UserProfileDto>>> UpdateCurrentUserProfile([FromBody] UpdateUserProfileDto updateProfileDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var updatedProfile = await _userProfileService.UpdateUserProfileAsync(userId, updateProfileDto);
            
            if (updatedProfile == null)
            {
                return BadRequest(ApiResponseDto<UserProfileDto>.Failure("Failed to update user profile"));
            }

            return Ok(ApiResponseDto<UserProfileDto>.Success(updatedProfile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for user {UserId}", GetCurrentUserId());
            return StatusCode(500, ApiResponseDto<UserProfileDto>.Failure("An error occurred while updating your profile"));
        }
    }

    /// <summary>
    /// Get list of users (Admin only, tenant-scoped)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<UserSummaryDto>>>> GetUsers(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            var tenantId = GetCurrentTenantId();
            var users = await _userService.GetUsersAsync(tenantId, page, pageSize, searchTerm);
            
            return Ok(ApiResponseDto<PagedResultDto<UserSummaryDto>>.Success(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for tenant {TenantId}", GetCurrentTenantId());
            return StatusCode(500, ApiResponseDto<PagedResultDto<UserSummaryDto>>.Failure("An error occurred while retrieving users"));
        }
    }

    /// <summary>
    /// Get specific user by ID (with permission checks)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<UserDetailDto>>> GetUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var tenantId = GetCurrentTenantId();
            var isAdmin = User.IsInRole("Admin");

            // Users can view their own profile, admins can view any user in their tenant
            if (id != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var user = await _userService.GetUserAsync(id, tenantId);
            
            if (user == null)
            {
                return NotFound(ApiResponseDto<UserDetailDto>.Failure("User not found"));
            }

            return Ok(ApiResponseDto<UserDetailDto>.Success(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId} for tenant {TenantId}", id, GetCurrentTenantId());
            return StatusCode(500, ApiResponseDto<UserDetailDto>.Failure("An error occurred while retrieving the user"));
        }
    }

    /// <summary>
    /// Delete user (Admin only, cannot delete self)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var tenantId = GetCurrentTenantId();

            // Prevent admins from deleting themselves
            if (id == currentUserId)
            {
                return BadRequest(ApiResponseDto<bool>.Failure("You cannot delete your own account"));
            }

            var result = await _userService.DeleteUserAsync(id, tenantId);
            
            if (!result)
            {
                return NotFound(ApiResponseDto<bool>.Failure("User not found or could not be deleted"));
            }

            return Ok(ApiResponseDto<bool>.Success(true, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for tenant {TenantId}", id, GetCurrentTenantId());
            return StatusCode(500, ApiResponseDto<bool>.Failure("An error occurred while deleting the user"));
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

    private int GetCurrentTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (int.TryParse(tenantIdClaim, out var tenantId))
        {
            return tenantId;
        }
        throw new UnauthorizedAccessException("Tenant ID not found in token");
    }

    #endregion
}
```

### 2. User Services Implementation

#### IUserService Interface
**Location**: `src/shared/Contracts/Services/IUserService.cs`

```csharp
using DTOs.Common;
using DTOs.Users;

namespace Contracts.Services;

public interface IUserService
{
    Task<PagedResultDto<UserSummaryDto>> GetUsersAsync(int tenantId, int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<UserDetailDto?> GetUserAsync(int userId, int tenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(int userId, int tenantId, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(int userId, int tenantId, CancellationToken cancellationToken = default);
    Task<UserDetailDto?> UpdateUserRoleAsync(int userId, int tenantId, string role, CancellationToken cancellationToken = default);
}
```

#### UserService Implementation
**Location**: `src/services/UserService/Services/UserService.cs`

```csharp
using AutoMapper;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Common;
using DTOs.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResultDto<UserSummaryDto>> GetUsersAsync(
        int tenantId, 
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _userRepository.GetQuery()
                .Where(u => u.TenantId == tenantId)
                .Where(u => u.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<List<UserSummaryDto>>(users);

            return new PagedResultDto<UserSummaryDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<UserDetailDto?> GetUserAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetQuery()
                .Where(u => u.Id == userId && u.TenantId == tenantId && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            return user != null ? _mapper.Map<UserDetailDto>(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId} for tenant {TenantId}", userId, tenantId);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetQuery()
                .Where(u => u.Id == userId && u.TenantId == tenantId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return false;
            }

            // Soft delete - set IsActive to false
            user.IsActive = false;
            user.DeactivatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId} for tenant {TenantId}", userId, tenantId);
            throw;
        }
    }

    public async Task<bool> UserExistsAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _userRepository.GetQuery()
                .AnyAsync(u => u.Id == userId && u.TenantId == tenantId && u.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} exists for tenant {TenantId}", userId, tenantId);
            throw;
        }
    }

    public async Task<UserDetailDto?> UpdateUserRoleAsync(int userId, int tenantId, string role, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetQuery()
                .Where(u => u.Id == userId && u.TenantId == tenantId && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return null;
            }

            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserDetailDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role for user {UserId} in tenant {TenantId}", userId, tenantId);
            throw;
        }
    }
}
```

#### IUserProfileService Interface
**Location**: `src/shared/Contracts/Services/IUserProfileService.cs`

```csharp
using DTOs.Users;

namespace Contracts.Services;

public interface IUserProfileService
{
    Task<UserProfileDto?> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateProfileDto, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserPreferencesAsync(int userId, UserPreferencesDto preferences, CancellationToken cancellationToken = default);
}
```

#### UserProfileService Implementation
**Location**: `src/services/UserService/Services/UserProfileService.cs`

```csharp
using AutoMapper;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserService.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserProfileService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetQuery()
                .Where(u => u.Id == userId && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            return user != null ? _mapper.Map<UserProfileDto>(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile for user {UserId}", userId);
            throw;
        }
    }

    public async Task<UserProfileDto?> UpdateUserProfileAsync(
        int userId, 
        UpdateUserProfileDto updateProfileDto, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetQuery()
                .Where(u => u.Id == userId && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return null;
            }

            // Update profile fields
            user.FirstName = updateProfileDto.FirstName;
            user.LastName = updateProfileDto.LastName;
            user.PhoneNumber = updateProfileDto.PhoneNumber;
            user.TimeZone = updateProfileDto.TimeZone;
            user.Language = updateProfileDto.Language;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return _mapper.Map<UserProfileDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> UpdateUserPreferencesAsync(
        int userId, 
        UserPreferencesDto preferences, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetQuery()
                .Where(u => u.Id == userId && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return false;
            }

            // Update preferences (stored as JSON in database)
            user.Preferences = System.Text.Json.JsonSerializer.Serialize(preferences);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences for user {UserId}", userId);
            throw;
        }
    }
}
```

### 3. DTOs (Data Transfer Objects)

#### File Locations
`src/shared/DTOs/Users/`

#### UserProfileDto.cs
```csharp
namespace DTOs.Users;

public record UserProfileDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? TimeZone { get; init; }
    public string? Language { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public UserPreferencesDto? Preferences { get; init; }
}

public record UserPreferencesDto
{
    public bool EmailNotifications { get; init; }
    public bool SmsNotifications { get; init; }
    public string Theme { get; init; } = "light";
    public string DateFormat { get; init; } = "MM/dd/yyyy";
    public string TimeFormat { get; init; } = "12h";
}
```

#### UpdateUserProfileDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace DTOs.Users;

public record UpdateUserProfileDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; init; }

    [StringLength(100)]
    public string? TimeZone { get; init; }

    [StringLength(10)]
    public string? Language { get; init; }
}
```

#### UserSummaryDto.cs
```csharp
namespace DTOs.Users;

public record UserSummaryDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public bool IsActive { get; init; }
}
```

#### UserDetailDto.cs
```csharp
namespace DTOs.Users;

public record UserDetailDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? TimeZone { get; init; }
    public string? Language { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public bool IsActive { get; init; }
    public bool IsEmailConfirmed { get; init; }
    public int LoginAttempts { get; init; }
    public DateTime? LockedUntil { get; init; }
}
```

### 4. Authorization Setup

#### Authorization Policies
**Location**: `src/services/UserService/Extensions/AuthorizationExtensions.cs`

```csharp
using Microsoft.AspNetCore.Authorization;

namespace UserService.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddUserServiceAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Admin role policy
            options.AddPolicy("AdminOnly", policy => 
                policy.RequireRole("Admin"));

            // User can access own resources policy
            options.AddPolicy("OwnerOrAdmin", policy =>
                policy.RequireAssertion(context =>
                {
                    var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var requestedUserId = context.Resource?.ToString();
                    var isAdmin = context.User.IsInRole("Admin");

                    return isAdmin || (userId != null && userId == requestedUserId);
                }));

            // Tenant isolation policy
            options.AddPolicy("SameTenant", policy =>
                policy.RequireAssertion(context =>
                {
                    var userTenantId = context.User.FindFirst("tenant_id")?.Value;
                    var requestedTenantId = context.Resource?.ToString();

                    return userTenantId != null && userTenantId == requestedTenantId;
                }));
        });

        return services;
    }
}
```

### 5. AutoMapper Profiles

#### UserProfile.cs
**Location**: `src/services/UserService/Mapping/UserProfile.cs`

```csharp
using AutoMapper;
using DTOs.Entities;
using DTOs.Users;

namespace UserService.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserProfileDto>()
            .ForMember(dest => dest.Preferences, opt => 
                opt.MapFrom(src => string.IsNullOrEmpty(src.Preferences) 
                    ? null 
                    : System.Text.Json.JsonSerializer.Deserialize<UserPreferencesDto>(src.Preferences)));

        CreateMap<User, UserSummaryDto>();
        
        CreateMap<User, UserDetailDto>();
        
        CreateMap<UpdateUserProfileDto, User>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}
```

### 6. Project Structure

#### UserService Project
```
src/services/UserService/
├── Controllers/
│   └── UsersController.cs
├── Services/
│   ├── UserService.cs
│   └── UserProfileService.cs
├── Mapping/
│   └── UserProfile.cs
├── Extensions/
│   └── AuthorizationExtensions.cs
├── UserService.csproj
└── Program.cs
```

#### Program.cs
**Location**: `src/services/UserService/Program.cs`

```csharp
using Common.Data;
using Common.Extensions;
using Common.Middleware;
using UserService.Extensions;
using UserService.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddUserServiceAuthorization();

// Services
builder.Services.AddScoped<Contracts.Services.IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<Contracts.Services.IUserProfileService, Services.UserProfileService>();

// Repository pattern
builder.Services.AddRepositories();

// AutoMapper
builder.Services.AddAutoMapper(typeof(UserProfile));

// Logging
builder.Services.AddLogging();

// FluentValidation
builder.Services.AddFluentValidation();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
```

---

## ✅ Dependencies & Prerequisites

### Phase 4 Requirements (VERIFIED ✅)
- ✅ **User registration and login working** - Full implementation with enterprise security
- ✅ **JWT tokens generated and validated correctly** - Production-ready JWT with tenant claims
- ✅ **Tenant context preserved in authentication** - Tenant information in JWT claims
- ✅ **Complete input validation implemented** - Comprehensive FluentValidation for all DTOs

### Technical Dependencies
- ✅ **Database Foundation** - User entities and repositories from Phase 3
- ✅ **Common Libraries** - Shared DTOs, services, and middleware
- ✅ **Authentication Middleware** - JWT authentication from Phase 4

### Required NuGet Packages
```xml
<!-- UserService.csproj -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.7" />
<PackageReference Include="AutoMapper" Version="15.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="15.0.1" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
```

---

## 🎯 Acceptance Criteria

### Functional Requirements
- [ ] **GET /api/users/profile** returns current user's profile information
- [ ] **PUT /api/users/profile** updates user profile with validation
- [ ] **GET /api/users** returns paginated, searchable user list (admin only)
- [ ] **GET /api/users/{id}** returns user details with proper authorization
- [ ] **DELETE /api/users/{id}** soft-deletes users (admin only, not self)

### Security Requirements
- [ ] **Authentication Required** - All endpoints require valid JWT token
- [ ] **Role-Based Authorization** - Admin-only endpoints properly protected
- [ ] **Tenant Isolation** - Users can only access users in their tenant
- [ ] **Self-Service Restrictions** - Users can only modify their own profiles
- [ ] **Prevent Self-Deletion** - Admins cannot delete their own accounts

### Quality Requirements
- [ ] **Comprehensive Error Handling** - All endpoints handle exceptions gracefully
- [ ] **Input Validation** - All DTOs properly validated with FluentValidation
- [ ] **Logging** - All operations logged with appropriate levels
- [ ] **Performance** - Efficient database queries with proper indexing

### Integration Testing
- [ ] **User CRUD Operations** - All endpoints tested with valid/invalid scenarios
- [ ] **Authorization Scenarios** - Role-based access control verified
- [ ] **Tenant Isolation** - Cross-tenant access prevention verified
- [ ] **Error Handling** - Exception scenarios properly tested

---

## 🧪 Integration Test Implementation

### Test Project Structure
```
tests/integration/UserService.IntegrationTests/
├── UserService.IntegrationTests.csproj
├── Controllers/
│   └── UsersControllerTests.cs
├── TestBase.cs
└── TestUtilities/
    ├── TestUser.cs
    └── AuthenticationHelper.cs
```

### Sample Integration Test
**Location**: `tests/integration/UserService.IntegrationTests/Controllers/UsersControllerTests.cs`

```csharp
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using DTOs.Users;
using DTOs.Common;
using System.Net;

namespace UserService.IntegrationTests.Controllers;

public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUserProfile_WithValidToken_ReturnsUserProfile()
    {
        // Arrange
        var token = await AuthenticationHelper.GetValidUserToken(_client);
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserProfileDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUsers_WithoutAdminRole_ReturnsForbidden()
    {
        // Arrange
        var userToken = await AuthenticationHelper.GetValidUserToken(_client);
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserProfile_WithValidData_ReturnsUpdatedProfile()
    {
        // Arrange
        var token = await AuthenticationHelper.GetValidUserToken(_client);
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new UpdateUserProfileDto
        {
            FirstName = "Updated",
            LastName = "User",
            PhoneNumber = "+1-555-0123",
            TimeZone = "America/New_York",
            Language = "en"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/profile", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserProfileDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.FirstName.Should().Be("Updated");
        result.Data.LastName.Should().Be("User");
    }
}
```

---

## 🚀 Next Phase Prerequisites

Upon completion of Phase 5, the following should be verified:

### ✅ Success Criteria Checklist
- [ ] **User management endpoints functional** - All 5 endpoints working correctly
- [ ] **Proper authorization and tenant isolation** - Security policies enforced
- [ ] **Integration tests passing** - All test scenarios validated
- [ ] **No cross-tenant data leakage** - Tenant boundaries enforced
- [ ] **Role-based access control working** - Admin/User permissions enforced

### Phase 6 Preparation
Phase 5 completion enables **Phase 6: API Gateway** implementation with:
- Centralized routing for Auth and User services
- Rate limiting and middleware setup
- Service discovery configuration
- Unified API documentation

---

## 📝 Implementation Checklist

### Week 1: Core Implementation
- [ ] Create UserService project structure
- [ ] Implement UsersController with 5 endpoints
- [ ] Create UserService and UserProfileService classes
- [ ] Implement User DTOs and validation
- [ ] Set up AutoMapper profiles
- [ ] Configure authorization policies

### Week 2: Testing & Quality
- [ ] Create integration test project
- [ ] Implement comprehensive test scenarios
- [ ] Test authorization and tenant isolation
- [ ] Verify error handling and logging
- [ ] Performance testing and optimization
- [ ] Documentation and code review

---

**Document Version**: 1.0  
**Created**: Ready for Phase 5 Implementation  
**Dependencies**: Phase 4 (Authentication Service) - 100% Complete ✅  
**Next Phase**: Phase 6 (API Gateway)

---

## 🔄 Context for Future Sessions

**Use this document as complete context when asking for Phase 5 implementation assistance. It contains:**

✅ **Complete specifications** for all 5 required endpoints  
✅ **Full service layer implementation** with business logic  
✅ **Authorization framework** with role-based policies  
✅ **Integration testing strategy** with sample tests  
✅ **Project structure** and configuration details  
✅ **Acceptance criteria** and success metrics  
✅ **Dependencies verification** from Phase 4  
✅ **Next phase preparation** for Phase 6  

**Ready for implementation with enterprise-grade authentication foundation from Phase 4!**
