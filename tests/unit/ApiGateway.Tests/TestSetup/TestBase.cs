using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiGateway.Tests.TestSetup;

public abstract class TestBase
{
    protected Mock<ILogger<T>> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    protected HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        context.Request.Headers["Content-Type"] = "application/json";
        context.Response.Body = new MemoryStream();
        return context;
    }

    protected IConfiguration CreateConfiguration(Dictionary<string, string> configValues)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();
    }

    protected ServiceCollection CreateServiceCollection()
    {
        return new ServiceCollection();
    }
}
