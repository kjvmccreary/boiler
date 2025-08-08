using System.Collections.Concurrent;
using System.Text.Json;

namespace ApiGateway.Services;

public class ServiceDiscovery : IServiceDiscovery
{
    private readonly ILogger<ServiceDiscovery> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly Timer _healthCheckTimer;
    
    // In-memory service registry (for production, use Consul, etcd, or Redis)
    private readonly ConcurrentDictionary<string, List<ServiceEndpoint>> _serviceRegistry = new();
    private readonly ServiceDiscoveryOptions _options;

    public ServiceDiscovery(
        ILogger<ServiceDiscovery> logger, 
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
        _options = configuration.GetSection("ServiceDiscovery").Get<ServiceDiscoveryOptions>() ?? new ServiceDiscoveryOptions();
        
        // Initialize with known services from configuration
        InitializeKnownServices();
        
        // Start health check timer
        _healthCheckTimer = new Timer(PerformHealthChecks, null, TimeSpan.Zero, TimeSpan.FromSeconds(_options.HealthCheckIntervalSeconds));
    }

    // FIXED: Return Task.FromResult since this is a synchronous operation
    public Task<IEnumerable<ServiceEndpoint>> GetHealthyServicesAsync(string serviceName)
    {
        if (!_serviceRegistry.TryGetValue(serviceName, out var endpoints))
        {
            return Task.FromResult(Enumerable.Empty<ServiceEndpoint>());
        }

        var healthyEndpoints = endpoints.Where(e => e.IsHealthy).ToList();
        return Task.FromResult<IEnumerable<ServiceEndpoint>>(healthyEndpoints);
    }

    public async Task<ServiceEndpoint?> GetServiceEndpointAsync(string serviceName)
    {
        var healthyServices = await GetHealthyServicesAsync(serviceName);
        
        if (!healthyServices.Any())
        {
            _logger.LogWarning("No healthy instances found for service: {ServiceName}", serviceName);
            return null;
        }

        // Simple round-robin load balancing
        var serviceList = healthyServices.ToList();
        var selectedService = serviceList[Random.Shared.Next(serviceList.Count)];
        
        _logger.LogDebug("Selected service instance: {ServiceName} -> {Host}:{Port}", 
            serviceName, selectedService.Host, selectedService.Port);
        
        return selectedService;
    }

    // FIXED: Remove async since this is a synchronous operation
    public Task RegisterServiceAsync(ServiceEndpoint endpoint)
    {
        _serviceRegistry.AddOrUpdate(
            endpoint.ServiceName,
            new List<ServiceEndpoint> { endpoint },
            (key, existing) =>
            {
                var existingInstance = existing.FirstOrDefault(e => e.InstanceId == endpoint.InstanceId);
                if (existingInstance != null)
                {
                    existing.Remove(existingInstance);
                }
                existing.Add(endpoint);
                return existing;
            });

        _logger.LogInformation("Registered service: {ServiceName} instance {InstanceId} at {Host}:{Port}",
            endpoint.ServiceName, endpoint.InstanceId, endpoint.Host, endpoint.Port);
        
        return Task.CompletedTask;
    }

    // FIXED: Remove async since this is a synchronous operation
    public Task UnregisterServiceAsync(string serviceName, string instanceId)
    {
        if (_serviceRegistry.TryGetValue(serviceName, out var endpoints))
        {
            var toRemove = endpoints.FirstOrDefault(e => e.InstanceId == instanceId);
            if (toRemove != null)
            {
                endpoints.Remove(toRemove);
                _logger.LogInformation("Unregistered service: {ServiceName} instance {InstanceId}",
                    serviceName, instanceId);
            }
        }
        
        return Task.CompletedTask;
    }

    // FIXED: Return Task.FromResult since this is a synchronous operation
    public Task<Dictionary<string, ServiceHealthStatus>> GetServiceHealthStatusAsync()
    {
        var result = new Dictionary<string, ServiceHealthStatus>();
        
        foreach (var kvp in _serviceRegistry)
        {
            var serviceName = kvp.Key;
            var instances = kvp.Value;
            
            result[serviceName] = new ServiceHealthStatus
            {
                ServiceName = serviceName,
                TotalInstances = instances.Count,
                HealthyInstances = instances.Count(i => i.IsHealthy),
                LastCheck = instances.Any() ? instances.Max(i => i.LastHealthCheck) : DateTime.UtcNow,
                Instances = instances.ToList()
            };
        }
        
        return Task.FromResult(result);
    }

    private void InitializeKnownServices()
    {
        var knownServices = _configuration.GetSection("ServiceDiscovery:KnownServices").Get<List<ServiceEndpoint>>() ?? new List<ServiceEndpoint>();
        
        foreach (var service in knownServices)
        {
            service.InstanceId = $"{service.ServiceName}-{service.Host}-{service.Port}";
            service.IsHealthy = true; // Will be verified by health check
            
            // FIXED: Don't block with .Wait(), use the synchronous method directly
            RegisterServiceAsync(service);
        }
        
        _logger.LogInformation("Initialized {Count} known services", knownServices.Count);
    }

    private async void PerformHealthChecks(object? state)
    {
        try
        {
            var tasks = new List<Task>();
            
            foreach (var serviceGroup in _serviceRegistry)
            {
                foreach (var endpoint in serviceGroup.Value)
                {
                    tasks.Add(CheckServiceHealthAsync(endpoint));
                }
            }
            
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during health check execution");
        }
    }

    private async Task CheckServiceHealthAsync(ServiceEndpoint endpoint)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.HealthCheckTimeoutSeconds));
            
            var response = await _httpClient.GetAsync(endpoint.HealthCheckUrl, cts.Token);
            var isHealthy = response.IsSuccessStatusCode;
            
            if (endpoint.IsHealthy != isHealthy)
            {
                _logger.LogInformation("Service {ServiceName} instance {InstanceId} health changed: {OldStatus} -> {NewStatus}",
                    endpoint.ServiceName, endpoint.InstanceId, endpoint.IsHealthy, isHealthy);
            }
            
            endpoint.IsHealthy = isHealthy;
            endpoint.LastHealthCheck = DateTime.UtcNow;
            
            if (!isHealthy)
            {
                _logger.LogWarning("Health check failed for {ServiceName} at {Url}: {StatusCode}",
                    endpoint.ServiceName, endpoint.HealthCheckUrl, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            endpoint.IsHealthy = false;
            endpoint.LastHealthCheck = DateTime.UtcNow;
            
            _logger.LogWarning("Health check failed for {ServiceName} at {Url}: {Error}",
                endpoint.ServiceName, endpoint.HealthCheckUrl, ex.Message);
        }
    }

    public void Dispose()
    {
        _healthCheckTimer?.Dispose();
        _httpClient?.Dispose();
    }
}

public class ServiceDiscoveryOptions
{
    public int HealthCheckIntervalSeconds { get; set; } = 30;
    public int HealthCheckTimeoutSeconds { get; set; } = 5;
    public List<ServiceEndpoint> KnownServices { get; set; } = new();
}
