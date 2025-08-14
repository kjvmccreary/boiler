using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using DTOs.Common;
using DTOs.Auth;
using DTOs.User;
using Contracts.Services;
using UserService.IntegrationTests.Fixtures;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Collections.Concurrent;

namespace UserService.IntegrationTests.LoadTesting;

[Collection("RoleAssignmentLoad")]
public class RoleAssignmentLoadTests : TestBase
{
    private readonly ConcurrentDictionary<string, TimeSpan> _performanceMetrics = new();

    public RoleAssignmentLoadTests(WebApplicationTestFixture fixture) : base(fixture) { }

    #region Bulk Role Assignment Tests

    [Fact]
    public async Task AssignRoles_BulkAssignments_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create test roles for bulk assignment
        var testRoles = await CreateTestRolesForLoadTesting();
        var testUser = await CreateTestUserForLoadTesting();

        const int numberOfAssignments = 20; // Reasonable number for integration tests
        var assignmentTasks = new List<Task<HttpResponseMessage>>();

        // Act - Perform bulk role assignments
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Math.Min(numberOfAssignments, testRoles.Count); i++)
        {
            var assignRequest = new AssignUserRoleDto
            {
                RoleId = testRoles[i].Id
            };

            var task = _client.PostAsJsonAsync($"/api/users/{testUser.Id}/roles", assignRequest);
            assignmentTasks.Add(task);
        }

        var responses = await Task.WhenAll(assignmentTasks);
        stopwatch.Stop();

        // Assert
        var successfulAssignments = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
        var totalTime = stopwatch.ElapsedMilliseconds;

        successfulAssignments.Should().BeGreaterThan(0, "At least some role assignments should succeed");
        totalTime.Should().BeLessThan(10000, "Bulk role assignments should complete within 10 seconds");

        // ✅ FIX: Convert double to int explicitly
        var averageTimePerAssignment = (int)(totalTime / (double)numberOfAssignments);
        averageTimePerAssignment.Should().BeLessThan(500, "Average time per assignment should be under 500ms");

        _logger.LogInformation("Bulk assignment performance: {SuccessCount}/{TotalCount} successful in {TotalMs}ms (avg: {AvgMs}ms per assignment)", 
            successfulAssignments, numberOfAssignments, totalTime, averageTimePerAssignment);

        // Cleanup
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }

    [Fact]
    public async Task AssignRoles_ConcurrentAssignments_ShouldHandleRaceConditions()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var testRoles = await CreateTestRolesForLoadTesting();
        var testUser = await CreateTestUserForLoadTesting();

        const int concurrentAssignments = 5;
        var tasks = new List<Task<(HttpStatusCode Status, string RoleName)>>();

        // Act - Attempt concurrent assignments of different roles to same user
        for (int i = 0; i < Math.Min(concurrentAssignments, testRoles.Count); i++)
        {
            var roleId = testRoles[i].Id;
            var roleName = testRoles[i].Name;
            
            var task = AssignRoleAsync(testUser.Id, roleId, roleName);
            tasks.Add(task);
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        var successfulAssignments = results.Count(r => r.Status == HttpStatusCode.OK);
        var conflictedAssignments = results.Count(r => r.Status == HttpStatusCode.Conflict || r.Status == HttpStatusCode.BadRequest);

        successfulAssignments.Should().BeGreaterThan(0, "At least some concurrent assignments should succeed");
        (successfulAssignments + conflictedAssignments).Should().Be(results.Length, 
            "All assignments should either succeed or fail with appropriate status codes");

        _logger.LogInformation("Concurrent assignment results: {SuccessCount} successful, {ConflictCount} conflicts", 
            successfulAssignments, conflictedAssignments);
    }

    [Fact]
    public async Task RemoveRoles_BulkRemoval_ShouldCompleteEfficiently()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var testRoles = await CreateTestRolesForLoadTesting();
        var testUser = await CreateTestUserForLoadTesting();

        // First assign roles to the user
        await AssignMultipleRolesToUser(testUser.Id, testRoles.Take(10).ToList());

        // Act - Bulk removal of roles
        var removalTasks = new List<Task<HttpResponseMessage>>();
        var stopwatch = Stopwatch.StartNew();

        foreach (var role in testRoles.Take(10))
        {
            var task = _client.DeleteAsync($"/api/roles/{role.Id}/users/{testUser.Id}");
            removalTasks.Add(task);
        }

        var responses = await Task.WhenAll(removalTasks);
        stopwatch.Stop();

        // Assert
        var successfulRemovals = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
        var totalTime = stopwatch.ElapsedMilliseconds;

        successfulRemovals.Should().BeGreaterThan(0, "At least some role removals should succeed");
        totalTime.Should().BeLessThan(5000, "Bulk role removal should complete within 5 seconds");

        _logger.LogInformation("Bulk removal performance: {SuccessCount} removals in {TotalMs}ms", 
            successfulRemovals, totalTime);

        // Cleanup
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }

    #endregion

    #region User Role Query Load Tests

    [Fact]
    public async Task GetUserRoles_WithManyRoles_ShouldMaintainPerformance()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var testRoles = await CreateTestRolesForLoadTesting();
        var testUser = await CreateTestUserForLoadTesting();

        // Assign multiple roles to test performance with larger datasets
        await AssignMultipleRolesToUser(testUser.Id, testRoles.Take(15).ToList());

        // Act - Query user roles multiple times
        var queryTasks = new List<Task<(HttpStatusCode Status, int RoleCount, long ElapsedMs)>>();

        for (int i = 0; i < 20; i++)
        {
            var task = QueryUserRolesAsync(testUser.Id);
            queryTasks.Add(task);
        }

        var results = await Task.WhenAll(queryTasks);

        // Assert
        results.Should().AllSatisfy(result =>
        {
            result.Status.Should().Be(HttpStatusCode.OK);
            result.RoleCount.Should().BeGreaterThan(0);
            result.ElapsedMs.Should().BeLessThan(1000, "User role queries should complete within 1 second");
        });

        var averageTime = results.Average(r => r.ElapsedMs);
        var maxRoles = results.Max(r => r.RoleCount);

        averageTime.Should().BeLessThan(300, "Average query time should be under 300ms");

        _logger.LogInformation("User role query performance: {MaxRoles} roles, average {AvgMs}ms", 
            maxRoles, averageTime);
    }

    [Fact]
    public async Task GetRoleUsers_WithManyUsersPerRole_ShouldScaleWell()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create multiple test users and assign them to the same role
        var testUsers = await CreateMultipleTestUsers(10);
        var testRole = (await CreateTestRolesForLoadTesting()).First();

        // Assign all users to the same role
        foreach (var user in testUsers)
        {
            await AssignRoleAsync(user.Id, testRole.Id, testRole.Name);
        }

        // Act - Query role users
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync($"/api/roles/{testRole.Id}/users");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1500, 
            "Role user queries should complete within 1.5 seconds even with many users");

        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeTrue();
        result.Data!.Count.Should().BeGreaterThan(5, "Should return multiple users for the role");

        _logger.LogInformation("Role user query returned {UserCount} users in {ElapsedMs}ms", 
            result.Data.Count, stopwatch.ElapsedMilliseconds);
    }

    #endregion

    #region Stress Testing

    [Fact]
    public async Task RoleAssignmentSystem_UnderStress_ShouldRemainStable()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        const int stressOperations = 50;
        var operationTasks = new List<Task<string>>();

        // Act - Perform mixed operations under stress
        for (int i = 0; i < stressOperations; i++)
        {
            var operationType = i % 4;
            Task<string> task;

            switch (operationType)
            {
                case 0:
                    task = PerformRoleCreation(i);
                    break;
                case 1:
                    task = PerformRoleAssignment(i);
                    break;
                case 2:
                    task = PerformRoleQuery(i);
                    break;
                default:
                    task = PerformPermissionUpdate(i);
                    break;
            }

            operationTasks.Add(task);
        }

        var results = await Task.WhenAll(operationTasks);

        // Assert
        var successfulOperations = results.Count(r => r.StartsWith("SUCCESS"));
        var errorOperations = results.Count(r => r.StartsWith("ERROR"));

        successfulOperations.Should().BeGreaterThan((int)(stressOperations * 0.8), 
            "At least 80% of stress operations should succeed");

        _logger.LogInformation("Stress test results: {SuccessCount}/{TotalCount} successful operations", 
            successfulOperations, stressOperations);

        // Log any errors for debugging
        var errors = results.Where(r => r.StartsWith("ERROR")).ToList();
        foreach (var error in errors.Take(5)) // Log first 5 errors
        {
            _logger.LogWarning("Stress test error: {Error}", error);
        }
    }

    #endregion

    #region Helper Methods

    private async Task<List<RoleDto>> CreateTestRolesForLoadTesting()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var roles = new List<RoleDto>();
        
        for (int i = 1; i <= 25; i++)
        {
            try
            {
                var createRequest = new CreateRoleDto
                {
                    Name = $"LoadTestRole{i}",
                    Description = $"Load testing role {i}",
                    Permissions = new List<string> { "users.view" }
                };

                var response = await _client.PostAsJsonAsync("/api/roles", createRequest);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
                    if (result?.Data != null)
                    {
                        roles.Add(result.Data);
                    }
                }
                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to create test role {RoleNumber}: {Error}", i, ex.Message);
            }
        }

        return roles;
    }

    private async Task<UserDto> CreateTestUserForLoadTesting()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateUserDto
        {
            Email = $"loadtest.user.{Guid.NewGuid():N}@tenant1.com",
            FirstName = "Load",
            LastName = "Test",
            // 🔧 FIX: Add required password fields
            Password = "TestPassword123!",
            ConfirmPassword = "TestPassword123!"
        };

        var response = await _client.PostAsJsonAsync("/api/users", createRequest);
        
        // 🔧 FIX: Better error handling
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to create test user. Status: {response.StatusCode}, Content: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();
        if (result?.Success != true || result.Data == null)
        {
            throw new InvalidOperationException($"User creation returned unsuccessful result: {result?.Message}");
        }

        return result.Data;
    }

    private async Task<List<UserDto>> CreateMultipleTestUsers(int count)
    {
        var users = new List<UserDto>();
        
        for (int i = 1; i <= count; i++)
        {
            try
            {
                var user = await CreateTestUserForLoadTesting();
                users.Add(user);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to create test user {UserNumber}: {Error}", i, ex.Message);
            }
        }

        return users;
    }

    private async Task AssignMultipleRolesToUser(int userId, List<RoleDto> roles)
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        foreach (var role in roles)
        {
            try
            {
                var assignRequest = new AssignUserRoleDto { RoleId = role.Id };
                var response = await _client.PostAsJsonAsync($"/api/users/{userId}/roles", assignRequest);
                response.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to assign role {RoleId} to user {UserId}: {Error}", 
                    role.Id, userId, ex.Message);
            }
        }
    }

    private async Task<(HttpStatusCode Status, string RoleName)> AssignRoleAsync(int userId, int roleId, string roleName)
    {
        try
        {
            var token = await GetAuthTokenAsync();
            var client = _fixture.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var assignRequest = new AssignUserRoleDto { RoleId = roleId };
            var response = await client.PostAsJsonAsync($"/api/users/{userId}/roles", assignRequest);
            
            var status = response.StatusCode;
            response.Dispose();
            client.Dispose();

            return (status, roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            return (HttpStatusCode.InternalServerError, roleName);
        }
    }

    private async Task<(HttpStatusCode Status, int RoleCount, long ElapsedMs)> QueryUserRolesAsync(int userId)
    {
        try
        {
            var token = await GetAuthTokenAsync();
            var client = _fixture.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var stopwatch = Stopwatch.StartNew();
            var response = await client.GetAsync($"/api/roles/users/{userId}");
            stopwatch.Stop();

            var roleCount = 0;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
                roleCount = result?.Data?.Count ?? 0;
            }

            var status = response.StatusCode;
            var elapsed = stopwatch.ElapsedMilliseconds;

            response.Dispose();
            client.Dispose();

            return (status, roleCount, elapsed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying roles for user {UserId}", userId);
            return (HttpStatusCode.InternalServerError, 0, 0);
        }
    }

    private async Task<string> PerformRoleCreation(int iteration)
    {
        try
        {
            var token = await GetAuthTokenAsync();
            var client = _fixture.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var createRequest = new CreateRoleDto
            {
                Name = $"StressTestRole{iteration}_{Guid.NewGuid():N}",
                Description = $"Stress test role {iteration}",
                Permissions = new List<string> { "users.view" }
            };

            var response = await client.PostAsJsonAsync("/api/roles", createRequest);
            var success = response.StatusCode == HttpStatusCode.Created;

            response.Dispose();
            client.Dispose();

            return success ? $"SUCCESS: Role creation {iteration}" : $"ERROR: Role creation {iteration} failed with {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"ERROR: Role creation {iteration} exception: {ex.Message}";
        }
    }

    private async Task<string> PerformRoleAssignment(int iteration)
    {
        try
        {
            // Use existing user and role for assignment
            var assignRequest = new AssignUserRoleDto { RoleId = 2 }; // Admin role
            
            var token = await GetAuthTokenAsync();
            var client = _fixture.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var response = await client.PostAsJsonAsync("/api/users/1/roles", assignRequest);
            var success = response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Conflict;

            response.Dispose();
            client.Dispose();

            return success ? $"SUCCESS: Role assignment {iteration}" : $"ERROR: Role assignment {iteration} failed";
        }
        catch (Exception ex)
        {
            return $"ERROR: Role assignment {iteration} exception: {ex.Message}";
        }
    }

    private async Task<string> PerformRoleQuery(int iteration)
    {
        try
        {
            var token = await GetAuthTokenAsync();
            var client = _fixture.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var response = await client.GetAsync("/api/roles");
            var success = response.StatusCode == HttpStatusCode.OK;

            response.Dispose();
            client.Dispose();

            return success ? $"SUCCESS: Role query {iteration}" : $"ERROR: Role query {iteration} failed";
        }
        catch (Exception ex)
        {
            return $"ERROR: Role query {iteration} exception: {ex.Message}";
        }
    }

    private async Task<string> PerformPermissionUpdate(int iteration)
    {
        try
        {
            var permissions = new List<string> { "users.view", "roles.view" };
            
            var token = await GetAuthTokenAsync();
            var client = _fixture.CreateClient();
            client.DefaultRequestHeaders.Authorization = new("Bearer", token);

            var response = await client.PutAsJsonAsync("/api/roles/2/permissions", permissions);
            var success = response.StatusCode == HttpStatusCode.OK;

            response.Dispose();
            client.Dispose();

            return success ? $"SUCCESS: Permission update {iteration}" : $"ERROR: Permission update {iteration} failed";
        }
        catch (Exception ex)
        {
            return $"ERROR: Permission update {iteration} exception: {ex.Message}";
        }
    }

    #endregion
}
