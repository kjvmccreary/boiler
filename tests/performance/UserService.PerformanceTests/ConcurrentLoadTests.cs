using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Common.Data;
using DTOs.Common;
using DTOs.User;
using UserService.PerformanceTests.Fixtures;
using UserService.PerformanceTests.TestUtilities;
using UserService.PerformanceTests.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace UserService.PerformanceTests;

[Collection("Performance")]
public class ConcurrentLoadTests : IClassFixture<PerformanceTestFixture>, IAsyncLifetime
{
    private readonly PerformanceTestFixture _fixture;
    private readonly ITestOutputHelper _output;

    public ConcurrentLoadTests(PerformanceTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    public async Task InitializeAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await PerformanceDataSeeder.SeedPerformanceDataAsync(context);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ConcurrentUserRequests_ShouldMaintainPerformance()
    {
        // Arrange
        const int concurrentUsers = 10;
        const int requestsPerUser = 5;
        var allResults = new List<(long ElapsedMs, bool Success)>();

        // Act - Simulate concurrent users
        var tasks = new List<Task<List<(long ElapsedMs, bool Success)>>>();
        
        for (int userId = 1; userId <= concurrentUsers; userId++)
        {
            var userTask = SimulateUserLoad(userId, requestsPerUser);
            tasks.Add(userTask);
        }

        var userResults = await Task.WhenAll(tasks);
        
        // Flatten results
        foreach (var results in userResults)
        {
            allResults.AddRange(results);
        }

        // ✅ FIX: Handle empty results gracefully
        var successfulRequests = allResults.Where(r => r.Success).ToList();
        var successRate = allResults.Count > 0 ? (double)successfulRequests.Count / allResults.Count : 0.0;

        if (successfulRequests.Count > 0)
        {
            var averageTime = successfulRequests.Average(r => r.ElapsedMs);
            var maxTime = successfulRequests.Max(r => r.ElapsedMs);

            _output.WriteLine($"🚀 Concurrent Load Test Results:");
            _output.WriteLine($"   Total Requests: {allResults.Count}");
            _output.WriteLine($"   Successful: {successfulRequests.Count}");
            _output.WriteLine($"   Success Rate: {successRate:P2}");
            _output.WriteLine($"   Average Time: {averageTime:F2}ms");
            _output.WriteLine($"   Max Time: {maxTime}ms");

            // Performance assertions only if we have successful requests
            successRate.Should().BeGreaterThan(0.95, "At least 95% of requests should succeed");
            averageTime.Should().BeLessThan(200, "Average response time should be reasonable under load");
        }
        else
        {
            // If no successful requests, we need to fail the test but provide useful info
            _output.WriteLine($"❌ No successful requests out of {allResults.Count} total requests");
            _output.WriteLine($"   This indicates a configuration or authentication issue");
            
            // Check a few failed responses for debugging
            if (allResults.Count > 0)
            {
                _output.WriteLine($"   All {allResults.Count} requests failed - likely JWT authentication issue");
            }

            Assert.True(false, $"No successful requests out of {allResults.Count} attempts. Check JWT configuration and API availability.");
        }
    }

    private async Task<List<(long ElapsedMs, bool Success)>> SimulateUserLoad(int tenantId, int requestCount)
    {
        var results = new List<(long ElapsedMs, bool Success)>();
        var httpClient = _fixture.CreateClient();
        
        // ✅ FIX: Use corrected JWT token generator
        var token = JwtTokenGenerator.GenerateAdminToken(tenantId % 5 + 1); // Cycle through tenants 1-5
        
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
        httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", (tenantId % 5 + 1).ToString());

        for (int i = 0; i < requestCount; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await httpClient.GetAsync("/api/users?pageSize=5");
                stopwatch.Stop();
                
                results.Add((stopwatch.ElapsedMilliseconds, response.IsSuccessStatusCode));
            }
            catch
            {
                stopwatch.Stop();
                results.Add((stopwatch.ElapsedMilliseconds, false));
            }

            // Small delay to simulate real user behavior
            await Task.Delay(10);
        }

        httpClient.Dispose();
        return results;
    }

    [Fact]
    public async Task DatabaseConnectionPool_ShouldHandleConcurrency()
    {
        // Arrange
        const int concurrentConnections = 20;
        var tasks = new List<Task<bool>>();

        // Act - Test database connection pooling under load
        for (int i = 0; i < concurrentConnections; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var httpClient = _fixture.CreateClient();
                // ✅ FIX: Use corrected JWT token generator
                var token = JwtTokenGenerator.GenerateAdminToken(1);
                httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
                httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", "1");

                try
                {
                    var response = await httpClient.GetAsync("/api/roles?pageSize=5");
                    httpClient.Dispose();
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    httpClient.Dispose();
                    return false;
                }
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        var successCount = results.Count(r => r);
        var successRate = (double)successCount / concurrentConnections;

        _output.WriteLine($"📊 Database Connection Pool Test:");
        _output.WriteLine($"   Concurrent Connections: {concurrentConnections}");
        _output.WriteLine($"   Successful: {successCount}");
        _output.WriteLine($"   Success Rate: {successRate:P2}");

        if (successCount == 0)
        {
            _output.WriteLine($"❌ All database connection attempts failed - check JWT authentication");
            Assert.True(false, "All database connection attempts failed - likely JWT configuration issue");
        }

        successRate.Should().BeGreaterThan(0.9, "Database should handle concurrent connections efficiently");
    }
}
