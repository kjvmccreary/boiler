using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using WorkflowService.Domain.Models;

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
///
/// Token Expansion (Phase 2):
///   Supported in headers, bodyRaw, and string values within body object:
///     {{instance.id}}
///     {{context.someField}}
///     {{context.order.total}}
///   Unresolved tokens are left as-is.
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

            // Parse workflow context (best-effort)
            JsonNode? contextNode = null;
            if (!string.IsNullOrWhiteSpace(ctx.CurrentContextJson))
            {
                try { contextNode = JsonNode.Parse(ctx.CurrentContextJson); } catch { /* ignore */ }
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ctx.CancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            var client = _httpFactory.CreateClient("workflow-webhook");
            var req = new HttpRequestMessage(new HttpMethod(method.ToUpperInvariant()), uri);

            // Headers (with token expansion)
            if (root.TryGetProperty("headers", out var headersEl) && headersEl.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in headersEl.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.String)
                    {
                        var rawVal = prop.Value.GetString() ?? string.Empty;
                        var expanded = ExpandTokens(rawVal, ctx.Instance, contextNode);
                        if (!string.IsNullOrWhiteSpace(prop.Name))
                        {
                            req.Headers.TryAddWithoutValidation(prop.Name, expanded);
                        }
                    }
                }
            }

            // Body (bodyRaw OR body)
            if (root.TryGetProperty("bodyRaw", out var bodyRawEl) && bodyRawEl.ValueKind == JsonValueKind.String)
            {
                var expanded = ExpandTokens(bodyRawEl.GetString() ?? string.Empty, ctx.Instance, contextNode);
                req.Content = new StringContent(expanded, Encoding.UTF8, "application/json");
            }
            else if (root.TryGetProperty("body", out var bodyEl))
            {
                // Expand only string leaf nodes
                JsonNode? bodyNode = null;
                try
                {
                    bodyNode = JsonNode.Parse(bodyEl.GetRawText());
                }
                catch
                {
                    return AutomaticActionResult.Fail("Invalid body JSON");
                }

                if (bodyNode != null)
                {
                    ExpandJsonStringValues(bodyNode, ctx.Instance, contextNode);
                    var json = bodyNode.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
                    req.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }
            }
            else
            {
                // default empty JSON body for state-changing verbs
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

    #region Token Expansion

    private static readonly Regex TokenRegex = new(@"\{\{([^\}]+)\}\}", RegexOptions.Compiled);

    private static string ExpandTokens(string input, WorkflowInstance instance, JsonNode? contextNode)
    {
        if (string.IsNullOrEmpty(input)) return input;

        return TokenRegex.Replace(input, m =>
        {
            var raw = m.Groups[1].Value.Trim();
            if (string.IsNullOrEmpty(raw)) return m.Value;

            if (raw.StartsWith("instance.", StringComparison.OrdinalIgnoreCase))
            {
                var key = raw.Substring("instance.".Length).ToLowerInvariant();
                return key switch
                {
                    "id" => instance.Id.ToString(),
                    "definitionid" => instance.WorkflowDefinitionId.ToString(),
                    "definitionversion" => instance.DefinitionVersion.ToString(),
                    "status" => instance.Status.ToString(),
                    _ => m.Value
                };
            }

            if (raw.StartsWith("context.", StringComparison.OrdinalIgnoreCase))
            {
                var path = raw.Substring("context.".Length);
                var val = ResolveContextPath(contextNode, path);
                if (val == null) return m.Value;
                return val;
            }

            return m.Value;
        });
    }

    private static string? ResolveContextPath(JsonNode? root, string path)
    {
        if (root == null || string.IsNullOrWhiteSpace(path)) return null;

        var segments = path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        JsonNode? current = root;

        foreach (var seg in segments)
        {
            if (current == null) return null;

            if (current is JsonObject obj)
            {
                if (!obj.TryGetPropertyValue(seg, out current))
                    return null;
            }
            else if (current is JsonArray arr)
            {
                if (int.TryParse(seg, out var idx))
                {
                    current = idx >= 0 && idx < arr.Count ? arr[idx] : null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        if (current is null) return null;

        if (current is JsonValue jv)
        {
            if (jv.TryGetValue<bool>(out var b)) return b ? "true" : "false";
            if (jv.TryGetValue<long>(out var l)) return l.ToString();
            if (jv.TryGetValue<double>(out var d)) return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (jv.TryGetValue<string>(out var s)) return s;
            return jv.ToJsonString();
        }

        return current.ToJsonString();
    }

    private static void ExpandJsonStringValues(JsonNode node, WorkflowInstance instance, JsonNode? contextNode)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var prop in obj.ToList())
                {
                    if (prop.Value is JsonValue v && v.TryGetValue<string>(out var s))
                    {
                        var expanded = ExpandTokens(s, instance, contextNode);
                        if (!ReferenceEquals(expanded, s))
                            obj[prop.Key] = expanded;
                    }
                    else if (prop.Value != null)
                    {
                        ExpandJsonStringValues(prop.Value, instance, contextNode);
                    }
                }
                break;

            case JsonArray arr:
                for (int i = 0; i < arr.Count; i++)
                {
                    var item = arr[i];
                    if (item is JsonValue v && v.TryGetValue<string>(out var s))
                    {
                        var expanded = ExpandTokens(s, instance, contextNode);
                        if (!ReferenceEquals(expanded, s))
                            arr[i] = expanded;
                    }
                    else if (item != null)
                    {
                        ExpandJsonStringValues(item, instance, contextNode);
                    }
                }
                break;
        }
    }

    #endregion
}
