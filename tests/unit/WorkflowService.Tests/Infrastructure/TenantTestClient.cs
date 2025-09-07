using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WorkflowService.Tests.Infrastructure;

public class TenantTestClient
{
    private readonly WorkflowServiceTestFixture? _fx;
    private readonly HttpClient _http;

    // Preferred constructor (provides fixture context)
    public TenantTestClient(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _http = fx.CreateClient();
    }

    // Legacy overloads accepting fixture + ignored second parameter
    public TenantTestClient(WorkflowServiceTestFixture fx, object? _ignored) : this(fx) { }
    public TenantTestClient(WorkflowServiceTestFixture fx, bool _stage2) : this(fx) { }
    public TenantTestClient(WorkflowServiceTestFixture fx, int _tenantId) : this(fx) { }

    // Direct HttpClient constructor (no fixture context)
    public TenantTestClient(HttpClient httpClient)
    {
        _fx = null;
        _http = httpClient;
    }

    // NEW bridge overloads for tests calling (HttpClient, X)
    public TenantTestClient(HttpClient httpClient, object? _ignored) : this(httpClient) { }
    public TenantTestClient(HttpClient httpClient, bool _ignored) : this(httpClient) { }
    public TenantTestClient(HttpClient httpClient, int _ignored) : this(httpClient) { }

    public HttpClient Raw => _http;

    public async Task AuthorizeAsync(string email, string password, int tenantId)
    {
        // Backwards compatibility: if fixture exists call its token helper; else fabricate token
        string token = _fx != null
            ? await _fx.GetTenantTokenAsync(email, password, tenantId)
            : $"dummy-{tenantId}";

        _http.DefaultRequestHeaders.Remove("Authorization");
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        _http.DefaultRequestHeaders.Remove("X-Tenant-ID");
        _http.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());

        _http.DefaultRequestHeaders.Remove("X-Test-Auth-Stage"); // ensure stage2 effective
    }

    public void SetStage1()
    {
        _http.DefaultRequestHeaders.Remove("Authorization");
        _http.DefaultRequestHeaders.Remove("X-Tenant-ID");
        _http.DefaultRequestHeaders.Remove("X-Test-Auth-Stage");
        _http.DefaultRequestHeaders.Add("Authorization", "Bearer stage1-token");
        _http.DefaultRequestHeaders.Add("X-Test-Auth-Stage", "1");
    }

    public void SetStage2(int tenantId)
    {
        _http.DefaultRequestHeaders.Remove("Authorization");
        _http.DefaultRequestHeaders.Remove("X-Tenant-ID");
        _http.DefaultRequestHeaders.Remove("X-Test-Auth-Stage");
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer stage2-{tenantId}");
        _http.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
    }

    public Task<HttpResponseMessage> GetAsync(string url) => _http.GetAsync(url);
    public Task<HttpResponseMessage> PostJsonAsync<T>(string url, T payload) =>
        _http.PostAsJsonAsync(url, payload);
}
