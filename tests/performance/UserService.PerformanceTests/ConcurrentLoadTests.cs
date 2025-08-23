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
public class ConcurrentLoadTests : IClassFixture<PerformanceTestFixture>
{
    private readonly PerformanceTestFixture _fixture;
    private readonly ITestOutputHelper _output;

    public ConcurrentLoadTests(PerformanceTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task ConcurrentUserRequests_ShouldMaintainPerformance()
    {
        // Arrange
        const int concurrentUsers = 10;
        const int requestsPerUser = 5;
        var allResults = new List<(long ElapsedMs, bool Success)>();

        // Act - Simulate concurrent users (all using the same valid user ID and tenant)
        var tasks = new List<Task<List<(long ElapsedMs, bool Success)>>>();
        
        for (int i = 0; i < concurrentUsers; i++)
        {
            var userTask = SimulateUserLoad(1, requestsPerUser); // 🔧 FIX: Use valid user ID 1
            tasks.Add(userTask);
        }

        var userResults = await Task.WhenAll(tasks);
        
        // Flatten results
        foreach (var results in userResults)
        {
            allResults.AddRange(results);
        }

        // Assert
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

            averageTime.Should().BeLessThan(200, "Average response time should be reasonable");
            successRate.Should().BeGreaterThan(0.8, "Success rate should be high");
        }
        else
        {
            throw new InvalidOperationException("No successful requests out of 50 attempts. Check JWT configuration and API availability.");
        }
    }

    [Fact]
    public async Task DatabaseConnectionPool_ShouldHandleConcurrency()
    {
        // Arrange
        const int concurrentConnections = 20;
        var tasks = new List<Task<bool>>();

        // Act - Multiple concurrent database operations
        for (int i = 0; i < concurrentConnections; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                var httpClient = _fixture.CreateClient();
                var token = JwtTokenGenerator.GenerateUserToken(1, 1); // 🔧 FIX: Use valid user ID 1, tenant ID 1
                httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
                httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", "1"); // 🔧 FIX: Use valid tenant ID 1

                try
                {
                    var response = await httpClient.GetAsync("/api/roles");
                    var success = response.IsSuccessStatusCode;
                    httpClient.Dispose();
                    return success;
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
        var successfulConnections = results.Count(r => r);
        var successRate = (double)successfulConnections / concurrentConnections;

        _output.WriteLine($"📊 Database Connection Pool Test:");
        _output.WriteLine($"   Concurrent Connections: {concurrentConnections}");
        _output.WriteLine($"   Successful: {successfulConnections}");
        _output.WriteLine($"   Success Rate: {successRate:P2}");

        if (successfulConnections == 0)
        {
            throw new InvalidOperationException("All database connection attempts failed - likely JWT configuration issue");
        }

        successRate.Should().BeGreaterThan(0.9, "Most concurrent connections should succeed");
    }

    private async Task<List<(long ElapsedMs, bool Success)>> SimulateUserLoad(int userId, int requestsPerUser)
    {
        var results = new List<(long ElapsedMs, bool Success)>();
        
        for (int i = 0; i < requestsPerUser; i++)
        {
            var httpClient = _fixture.CreateClient();
            var token = JwtTokenGenerator.GenerateUserToken(userId, 1);
            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);
            httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", "1");

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await httpClient.GetAsync("/api/users/profile");
                stopwatch.Stop();

                results.Add((stopwatch.ElapsedMilliseconds, response.IsSuccessStatusCode));
            }
            catch
            {
                results.Add((0, false));
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        return results;
    }
}
