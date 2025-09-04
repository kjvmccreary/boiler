using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WorkflowService.Engine.AutomaticActions.Executors;

/// <summary>
/// Minimal webhook executor. HTTPS only. Non-success => failure unless allowNonSuccess=true.
/// Config schema (node.action.config):
/// {
///   "url": "https://... (required)",
///   "method": "POST",
///   "headers": { "X-Source": "workflow" },
///   "body": { ... } OR "bodyRaw": "string",
///   "timeoutSeconds": 5,
///   "allowNonSuccess": false,
///   "truncateResponseBytes": 1024
/// }
/// </summary>
public class WebhookAutomaticActionExecutor : IAutomaticActionExecutor
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<WebhookAutomaticActionExecutor> _logger;

    public string Kind => "webhook";

    public WebhookAutomaticActionExecutor(
        IHttpClientFactory httpFactory,
        ILogger<WebhookAutomaticActionExecutor> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    public async Task<IAutomaticActionResult> ExecuteAsync(IAutomaticActionContext ctx)
    {
        try
        {
            if (ctx.ActionConfig == null)
                return AutomaticActionResult.Fail("Missing webhook configuration");

            var root = ctx.ActionConfig.RootElement;

            string? url = root.TryGetProperty("url", out var urlEl) && urlEl.ValueKind == JsonValueKind.String
                ? urlEl.GetString()
                : null;

            if (string.IsNullOrWhiteSpace(url))
                return AutomaticActionResult.Fail("Webhook url required");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                return AutomaticActionResult.Fail("Webhook must use absolute HTTPS URL");

            string method = root.TryGetProperty("method", out var methodEl) && methodEl.ValueKind == JsonValueKind.String
                ? methodEl.GetString() ?? "POST"
                : "POST";

            bool allowNonSuccess = root.TryGetProperty("allowNonSuccess", out var allowEl) &&
                                   allowEl.ValueKind == JsonValueKind.True;

            int timeoutSeconds = 5;
            if (root.TryGetProperty("timeoutSeconds", out var toEl) && toEl.ValueKind == JsonValueKind.Number)
                timeoutSeconds = Math.Clamp(toEl.GetInt32(), 1, 30);

            int truncate = 1024;
            if (root.TryGetProperty("truncateResponseBytes", out var trEl) && trEl.ValueKind == JsonValueKind.Number)
                truncate = Math.Clamp(trEl.GetInt32(), 128, 8192);

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ctx.CancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            var client = _httpFactory.CreateClient("workflow-webhook");
            using var req = new HttpRequestMessage(new HttpMethod(method.ToUpperInvariant()), uri);

            // Headers
            if (root.TryGetProperty("headers", out var headersEl) && headersEl.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in headersEl.EnumerateObject())
                {
                    if (!string.IsNullOrWhiteSpace(prop.Name) && prop.Value.ValueKind == JsonValueKind.String)
                    {
                        req.Headers.TryAddWithoutValidation(prop.Name, prop.Value.GetString());
                    }
                }
            }

            // Body
            if (root.TryGetProperty("bodyRaw", out var bodyRawEl) && bodyRawEl.ValueKind == JsonValueKind.String)
            {
                req.Content = new StringContent(bodyRawEl.GetString() ?? string.Empty, Encoding.UTF8, "application/json");
            }
            else if (root.TryGetProperty("body", out var bodyEl))
            {
                var json = bodyEl.GetRawText();
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            else
            {
                // default empty JSON body for POST/PUT/PATCH
                if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                    method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
                    method.Equals("PATCH", StringComparison.OrdinalIgnoreCase))
                {
                    req.Content = new StringContent("{}", Encoding.UTF8, "application/json");
                }
            }

            _logger.LogInformation("WF_AUTO_WEBHOOK Request Start Url={Url} Method={Method} Instance={InstanceId} Node={NodeId}",
                url, method, ctx.Instance.Id, ctx.NodeId);

            var start = DateTime.UtcNow;
            using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            var elapsedMs = (int)(DateTime.UtcNow - start).TotalMilliseconds;

            string? bodySnippet = null;
            try
            {
                if (resp.Content != null)
                {
                    var bytes = await resp.Content.ReadAsByteArrayAsync(cts.Token);
                    if (bytes.Length > 0)
                    {
                        if (bytes.Length > truncate)
                            bodySnippet = Encoding.UTF8.GetString(bytes.AsSpan(0, truncate)) + "...(truncated)";
                        else
                            bodySnippet = Encoding.UTF8.GetString(bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "WF_AUTO_WEBHOOK Response body read failed");
            }

            var success = resp.IsSuccessStatusCode || allowNonSuccess;
            if (!success)
            {
                return AutomaticActionResult.Fail($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}");
            }

            var output = new
            {
                status = (int)resp.StatusCode,
                reason = resp.ReasonPhrase,
                durationMs = elapsedMs,
                body = bodySnippet
            };

            return AutomaticActionResult.Ok(output);
        }
        catch (OperationCanceledException)
        {
            return AutomaticActionResult.Fail("Webhook timed out");
        }
        catch (Exception ex)
        {
            return AutomaticActionResult.Fail($"Webhook error: {ex.Message}");
        }
    }
}
