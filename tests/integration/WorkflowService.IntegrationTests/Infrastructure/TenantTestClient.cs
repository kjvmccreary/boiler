using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net;
using Xunit;

namespace WorkflowService.IntegrationTests.Infrastructure;

public class TenantTestClient
{
    private readonly HttpClient _client;
    private readonly Func<string,string,int,Task<string>> _tokenFactory;

    public TenantTestClient(HttpClient client, Func<string,string,int,Task<string>> tokenFactory)
    {
        _client = client;
        _tokenFactory = tokenFactory;
    }

    public async Task AuthorizeAsync(string email, string password, int tenantId)
    {
        var token = await _tokenFactory(email, password, tenantId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (_client.DefaultRequestHeaders.Contains("X-Tenant-ID"))
            _client.DefaultRequestHeaders.Remove("X-Tenant-ID");
        _client.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
    }

    public Task<HttpResponseMessage> GetAsync(string url) => _client.GetAsync(url);
    public Task<HttpResponseMessage> PostJsonAsync(string url, object body)
        => _client.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

    public static async Task<T> ReadAsAsync<T>(HttpResponseMessage resp)
    {
        var json = await resp.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}
