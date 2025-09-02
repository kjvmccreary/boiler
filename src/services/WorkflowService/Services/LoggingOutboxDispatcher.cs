using WorkflowService.Domain.Models;
using WorkflowService.Services.Interfaces;
using System.Text.Json;

namespace WorkflowService.Services;

public class LoggingOutboxDispatcher : IOutboxDispatcher
{
    private readonly ILogger<LoggingOutboxDispatcher> _logger;
    private readonly IHttpClientFactory? _httpClientFactory;
    private readonly string? _webhookUrl;
    private readonly bool _enableWebhook;

    public LoggingOutboxDispatcher(
        ILogger<LoggingOutboxDispatcher> logger,
        IHttpClientFactory? httpClientFactory = null,
        IConfiguration? configuration = null)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        
        if (configuration != null)
        {
            var outboxConfig = configuration.GetSection("WorkflowSettings:Outbox");
            _webhookUrl = outboxConfig.GetValue<string>("WebhookUrl");
            _enableWebhook = !string.IsNullOrEmpty(_webhookUrl) && 
                            outboxConfig.GetValue<bool>("EnableWebhook", false);
        }
    }

    public async Task<bool> DispatchAsync(OutboxMessage message, CancellationToken ct)
    {
        // Always log (core MVP behavior)
        var size = message.EventData?.Length ?? 0;

        _logger.LogInformation(
            "OUTBOX_DISPATCH EventType={EventType} Id={Id} Tenant={TenantId} Retry={Retry} PayloadSize={Size} IdempotencyKey={IdempotencyKey}",
            message.EventType,
            message.Id,
            message.TenantId,
            message.RetryCount,
            size,
            message.IdempotencyKey);

        // Optionally POST to webhook (if configured)
        if (_enableWebhook && !string.IsNullOrEmpty(_webhookUrl) && _httpClientFactory != null)
        {
            try
            {
                return await PostToWebhookAsync(message, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OUTBOX_WEBHOOK_FAILED Id={Id} - falling back to log-only", message.Id);
                return true; // Still consider it "dispatched" to logs
            }
        }

        return true; // Successfully "dispatched" to logs
    }

    private async Task<bool> PostToWebhookAsync(OutboxMessage message, CancellationToken ct)
    {
        using var httpClient = _httpClientFactory!.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        var payload = new
        {
            Id = message.Id,
            EventType = message.EventType,
            TenantId = message.TenantId,
            IdempotencyKey = message.IdempotencyKey,
            CreatedAt = message.CreatedAt,
            Data = JsonDocument.Parse(message.EventData ?? "{}")
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync(_webhookUrl, content, ct);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("OUTBOX_WEBHOOK_SUCCESS Id={Id} Status={StatusCode}", 
                message.Id, response.StatusCode);
            return true;
        }
        else
        {
            _logger.LogWarning("OUTBOX_WEBHOOK_FAILED Id={Id} Status={StatusCode}", 
                message.Id, response.StatusCode);
            return false; // Will trigger retry
        }
    }
}
