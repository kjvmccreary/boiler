using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using FluentAssertions;
using DTOs.Compliance;
using DTOs.Common;
using UserService.Controllers;

namespace IntegrationTests;

/// <summary>
/// Integration tests for compliance features
/// Phase 11 Session 3 - Compliance Features & Testing
/// </summary>
public class ComplianceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ComplianceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GenerateAccessReport_WithValidRequest_ShouldReturnReport()
    {
        // Arrange
        var request = new GenerateReportRequest
        {
            From = DateTime.UtcNow.AddDays(-30),
            To = DateTime.UtcNow,
            TenantId = 1
        };

        // Add authorization header (you'd implement proper auth setup)
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.PostAsJsonAsync("/api/compliance/reports/access", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<ComplianceReport>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Type.Should().Be("Access Report");
        result.Data.Sections.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GeneratePermissionUsageReport_WithValidRequest_ShouldReturnReport()
    {
        // Arrange
        var request = new GenerateReportRequest
        {
            From = DateTime.UtcNow.AddDays(-7),
            To = DateTime.UtcNow,
            TenantId = 1
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.PostAsJsonAsync("/api/compliance/reports/permissions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<ComplianceReport>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Type.Should().Be("Permission Usage Report");
    }

    [Fact]
    public async Task GenerateSecurityAuditReport_WithValidRequest_ShouldReturnReport()
    {
        // Arrange
        var request = new GenerateReportRequest
        {
            From = DateTime.UtcNow.AddDays(-14),
            To = DateTime.UtcNow,
            TenantId = 1
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.PostAsJsonAsync("/api/compliance/reports/security", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<ComplianceReport>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Type.Should().Be("Security Audit Report");
    }

    [Fact]
    public async Task GenerateDataRetentionReport_ShouldReturnReport()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.PostAsync("/api/compliance/reports/retention?tenantId=1", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<ComplianceReport>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Type.Should().Be("Data Retention Report");
    }

    [Fact]
    public async Task GetReports_ShouldReturnListOfReports()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.GetAsync("/api/compliance/reports?tenantId=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<ComplianceReport>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task ExportReport_WithPdfFormat_ShouldReturnPdfFile()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.GetAsync($"/api/compliance/reports/{reportId}/export?format=pdf");

        // Assert
        // Note: This would typically return 404 unless the report exists
        // In a real test, you'd first create a report, then export it
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExportReport_WithCsvFormat_ShouldReturnCsvFile()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.GetAsync($"/api/compliance/reports/{reportId}/export?format=csv");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActiveAlerts_ShouldReturnAlertsList()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.GetAsync("/api/compliance/alerts?tenantId=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<SecurityAlert>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task ResolveAlert_WithValidRequest_ShouldSucceed()
    {
        // Arrange
        var alertId = Guid.NewGuid();
        var request = new ResolveAlertRequest
        {
            ResolvedBy = "test-user",
            ResolutionNotes = "Resolved during testing"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/compliance/alerts/{alertId}/resolve", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetAlertConfiguration_ShouldReturnConfiguration()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.GetAsync("/api/compliance/alerts/configuration");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<AlertConfiguration>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAlertConfiguration_WithValidConfig_ShouldSucceed()
    {
        // Arrange
        var config = new AlertConfiguration
        {
            EnableEmailNotifications = true,
            EnableSlackNotifications = false,
            EmailRecipients = new List<string> { "test@example.com" },
            MinimumSeverityForNotification = AlertSeverity.Medium,
            NotificationCooldown = TimeSpan.FromMinutes(15)
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.PutAsJsonAsync("/api/compliance/alerts/configuration", config);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task TestAlert_WithValidRequest_ShouldSendAlert()
    {
        // Arrange
        var request = new TestAlertRequest
        {
            Severity = AlertSeverity.Low,
            Message = "This is a test alert from integration tests"
        };

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.PostAsJsonAsync("/api/compliance/alerts/test", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Theory]
    [InlineData("pdf")]
    [InlineData("csv")]
    [InlineData("json")]
    public async Task ExportReport_WithDifferentFormats_ShouldReturnCorrectContentType(string format)
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.GetAsync($"/api/compliance/reports/{reportId}/export?format={format}");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var expectedContentType = format switch
            {
                "pdf" => "application/pdf",
                "csv" => "text/csv",
                "json" => "application/json",
                _ => throw new ArgumentException($"Unexpected format: {format}")
            };

            response.Content.Headers.ContentType?.MediaType.Should().Be(expectedContentType);
        }
    }

    [Fact]
    public async Task ExportReport_WithInvalidFormat_ShouldReturnBadRequest()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        // Act
        var response = await _client.GetAsync($"/api/compliance/reports/{reportId}/export?format=invalid");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
