using Microsoft.AspNetCore.Mvc;
using ApiGateway.Services;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceDiscoveryController : ControllerBase
{
    private readonly IServiceDiscovery _serviceDiscovery;
    private readonly ILogger<ServiceDiscoveryController> _logger;

    public ServiceDiscoveryController(
        IServiceDiscovery serviceDiscovery,
        ILogger<ServiceDiscoveryController> logger)
    {
        _serviceDiscovery = serviceDiscovery;
        _logger = logger;
    }

    [HttpGet("services")]
    public async Task<ActionResult<Dictionary<string, ServiceHealthStatus>>> GetServicesAsync()
    {
        var services = await _serviceDiscovery.GetServiceHealthStatusAsync();
        return Ok(services);
    }

    [HttpGet("services/{serviceName}")]
    public async Task<ActionResult<IEnumerable<ServiceEndpoint>>> GetServiceEndpointsAsync(string serviceName)
    {
        var endpoints = await _serviceDiscovery.GetHealthyServicesAsync(serviceName);
        if (!endpoints.Any())
        {
            return NotFound($"No healthy instances found for service: {serviceName}");
        }
        
        return Ok(endpoints);
    }

    [HttpPost("services")]
    public async Task<ActionResult> RegisterServiceAsync([FromBody] ServiceEndpoint endpoint)
    {
        if (string.IsNullOrEmpty(endpoint.ServiceName) || string.IsNullOrEmpty(endpoint.Host))
        {
            return BadRequest("ServiceName and Host are required");
        }

        endpoint.InstanceId = $"{endpoint.ServiceName}-{endpoint.Host}-{endpoint.Port}";
        await _serviceDiscovery.RegisterServiceAsync(endpoint);
        
        _logger.LogInformation("Service registered via API: {ServiceName}", endpoint.ServiceName);
        return Ok(new { message = "Service registered successfully", instanceId = endpoint.InstanceId });
    }

    [HttpDelete("services/{serviceName}/{instanceId}")]
    public async Task<ActionResult> UnregisterServiceAsync(string serviceName, string instanceId)
    {
        await _serviceDiscovery.UnregisterServiceAsync(serviceName, instanceId);
        _logger.LogInformation("Service unregistered via API: {ServiceName}/{InstanceId}", serviceName, instanceId);
        return Ok(new { message = "Service unregistered successfully" });
    }
}
