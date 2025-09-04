using Common.Hashing;
using Contracts.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Hashing;

public static class DeterministicHashingServiceCollectionExtensions
{
    /// Registers deterministic hashing (singleton) for use across services.
    public static IServiceCollection AddDeterministicHashing(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DeterministicHashOptions>(configuration.GetSection("DeterministicHashing"));
        services.AddSingleton<IDeterministicHasher, DeterministicHasher>();
        return services;
    }
}
