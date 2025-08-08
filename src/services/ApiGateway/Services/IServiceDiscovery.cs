namespace ApiGateway.Services;

public interface IServiceDiscovery
{
    Task<IEnumerable<ServiceEndpoint>> GetHealthyServicesAsync(string serviceName);
    Task<ServiceEndpoint?> GetServiceEndpointAsync(string serviceName);
    Task RegisterServiceAsync(ServiceEndpoint endpoint);
    Task UnregisterServiceAsync(string serviceName, string instanceId);
    Task<Dictionary<string, ServiceHealthStatus>> GetServiceHealthStatusAsync();
}

public class ServiceEndpoint
{
    public string ServiceName { get; set; } = string.Empty;
    public string InstanceId { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Scheme { get; set; } = "http";
    public string HealthCheckPath { get; set; } = "/health";
    public DateTime LastHealthCheck { get; set; }
    public bool IsHealthy { get; set; }
    public string Version { get; set; } = "1.0.0";
    public Dictionary<string, string> Tags { get; set; } = new();
    
    public string BaseUrl => $"{Scheme}://{Host}:{Port}";
    public string HealthCheckUrl => $"{BaseUrl}{HealthCheckPath}";
}

public class ServiceHealthStatus
{
    public string ServiceName { get; set; } = string.Empty;
    public int HealthyInstances { get; set; }
    public int TotalInstances { get; set; }
    public DateTime LastCheck { get; set; }
    public List<ServiceEndpoint> Instances { get; set; } = new();
}
