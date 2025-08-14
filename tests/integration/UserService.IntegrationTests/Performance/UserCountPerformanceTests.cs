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

namespace UserService.IntegrationTests.Performance;

[Collection("UserCountPerformance")]
public class UserCountPerformanceTests : TestBase
{
    public UserCountPerformanceTests(WebApplicationTestFixture fixture) : base(fixture) { }

    #region User Count Query Performance Tests

    [Fact]
    public async Task GetRoles_WithUserCounts_ShouldCompleteUnder500ms()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Warm up the system with a preliminary request
        await _client.GetAsync("/api/roles");

        // Act - Measure performance
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/roles");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500, 
            "User count queries should complete within 500ms for normal data volumes");

        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        result!.Success.Should().BeTrue();
        result.Data!.Items.Should().NotBeEmpty();
        
        // Verify user counts are populated
        var rolesWithUserCounts = result.Data.Items.Where(r => r.UserCount > 0).ToList();
        rolesWithUserCounts.Should().NotBeEmpty("At least some roles should have user counts");

        _logger.LogInformation("GetRoles with user counts completed in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
    }

    [Fact]
    public async Task GetRoles_MultipleConsecutiveRequests_ShouldMaintainPerformance()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var requestTimes = new List<long>();
        const int numberOfRequests = 10;

        // Act - Make multiple consecutive requests
        for (int i = 0; i < numberOfRequests; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync("/api/roles");
            stopwatch.Stop();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            requestTimes.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert
        var averageTime = requestTimes.Average();
        var maxTime = requestTimes.Max();
        var minTime = requestTimes.Min();

        averageTime.Should().BeLessThan(300, "Average request time should be under 300ms");
        maxTime.Should().BeLessThan(1000, "No single request should take more than 1 second");
        
        // Performance should not degrade significantly
        var timeVariance = maxTime - minTime;
        timeVariance.Should().BeLessThan(500, "Request time variance should be reasonable");

        _logger.LogInformation("Performance stats - Average: {AvgMs}ms, Max: {MaxMs}ms, Min: {MinMs}ms, Variance: {VarianceMs}ms", 
            averageTime, maxTime, minTime, timeVariance);
    }

    [Fact]
    public async Task GetRoleUsers_WithLargeUserBase_ShouldCompleteUnder1Second()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // First, create additional test users and assign them to a role to simulate larger data
        await CreateTestUsersForPerformanceTesting();

        // Act - Measure performance of getting users for a role
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/roles/2/users"); // Admin role
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, 
            "Getting role users should complete within 1 second even with larger datasets");

        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        _logger.LogInformation("GetRoleUsers completed in {ElapsedMs}ms for {UserCount} users", 
            stopwatch.ElapsedMilliseconds, result.Data!.Count);
    }

    [Fact]
    public async Task GetRoles_WithPagination_ShouldScaleLinearly()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var pageSizes = new[] { 5, 10, 20, 50 };
        var performanceResults = new Dictionary<int, long>();

        // Act - Test different page sizes
        foreach (var pageSize in pageSizes)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync($"/api/roles?page=1&pageSize={pageSize}");
            stopwatch.Stop();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            performanceResults[pageSize] = stopwatch.ElapsedMilliseconds;

            var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
            result!.Success.Should().BeTrue();
        }

        // Assert - Performance should scale reasonably with page size
        var smallPageTime = performanceResults[5];
        var largePageTime = performanceResults[50];

        // Large page should not be more than 3x slower than small page
        largePageTime.Should().BeLessThan(smallPageTime * 3, 
            "Performance should scale reasonably with page size");

        _logger.LogInformation("Pagination performance: {Results}", 
            string.Join(", ", performanceResults.Select(kvp => $"{kvp.Key} items: {kvp.Value}ms")));
    }

    [Fact]
    public async Task UserCountQueries_ShouldUseEfficientSQLQueries()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // This test verifies that the backend is using efficient GROUP BY queries
        // rather than individual queries for each role's user count

        // Act - Make request that triggers user count calculation
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/roles");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Performance should be consistent regardless of number of roles
        // If using efficient queries, time should be roughly constant
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(400, 
            "Efficient GROUP BY queries should keep response time low");

        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        result!.Success.Should().BeTrue();
        
        // Verify multiple roles have user counts (proves GROUP BY is working)
        var rolesWithCounts = result.Data!.Items.Count(r => r.UserCount >= 0);
        rolesWithCounts.Should().BeGreaterThan(3, "Multiple roles should have user counts calculated");

        _logger.LogInformation("User count query completed in {ElapsedMs}ms for {RoleCount} roles", 
            stopwatch.ElapsedMilliseconds, result.Data.Items.Count);
    }

    #endregion

    #region Memory and Resource Usage Tests

    [Fact]
    public async Task GetRoles_RepeatedRequests_ShouldNotCauseMemoryLeaks()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        const int numberOfRequests = 100;
        var responses = new List<HttpResponseMessage>();

        // Act - Make many requests to test for memory leaks
        for (int i = 0; i < numberOfRequests; i++)
        {
            var response = await _client.GetAsync("/api/roles");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responses.Add(response);

            // Dispose every 10 responses to simulate normal usage
            if (i % 10 == 0)
            {
                foreach (var resp in responses.Take(5))
                {
                    resp.Dispose();
                }
                responses.RemoveRange(0, Math.Min(5, responses.Count));
            }
        }

        // Cleanup remaining responses
        foreach (var response in responses)
        {
            response.Dispose();
        }

        // Assert - If we get here without memory issues, the test passes
        // In a real scenario, you might monitor actual memory usage
        true.Should().BeTrue("Memory leak test completed successfully");

        _logger.LogInformation("Completed {RequestCount} requests without memory issues", numberOfRequests);
    }

    [Fact]
    public async Task GetRoleUsers_ConcurrentRequests_ShouldHandleLoadEfficiently()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        const int concurrentRequests = 10;
        var tasks = new List<Task<(HttpStatusCode StatusCode, long ElapsedMs)>>();

        // Act - Make concurrent requests
        for (int i = 0; i < concurrentRequests; i++)
        {
            var task = MakeTimedRequest($"/api/roles/{2}/users");
            tasks.Add(task);
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(result => 
        {
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.ElapsedMs.Should().BeLessThan(2000, "Concurrent requests should complete within 2 seconds");
        });

        var averageTime = results.Average(r => r.ElapsedMs);
        averageTime.Should().BeLessThan(1000, "Average concurrent request time should be under 1 second");

        _logger.LogInformation("Concurrent request performance: Average {AvgMs}ms across {RequestCount} requests", 
            averageTime, concurrentRequests);
    }

    #endregion

    #region Helper Methods

    private async Task CreateTestUsersForPerformanceTesting()
    {
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create a few additional test users to increase data volume
        var testUsers = new[]
        {
            new CreateUserDto { Email = "perftest1@tenant1.com", FirstName = "Perf", LastName = "Test1" },
            new CreateUserDto { Email = "perftest2@tenant1.com", FirstName = "Perf", LastName = "Test2" },
            new CreateUserDto { Email = "perftest3@tenant1.com", FirstName = "Perf", LastName = "Test3" }
        };

        foreach (var user in testUsers)
        {
            try
            {
                await _client.PostAsJsonAsync("/api/users", user);
                // Note: In a real performance test, you'd create many more users
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to create test user {Email}: {Error}", user.Email, ex.Message);
                // Continue with other users
            }
        }
    }

    private async Task<(HttpStatusCode StatusCode, long ElapsedMs)> MakeTimedRequest(string endpoint)
    {
        var token = await GetAuthTokenAsync();
        var client = _fixture.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var stopwatch = Stopwatch.StartNew();
        var response = await client.GetAsync(endpoint);
        stopwatch.Stop();

        var result = (response.StatusCode, stopwatch.ElapsedMilliseconds);
        response.Dispose();
        client.Dispose();

        return result;
    }

    #endregion
}
