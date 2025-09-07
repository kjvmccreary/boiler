using Common.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkflowService.Background;
using WorkflowService.Engine.Timeouts;
using WorkflowService.Outbox;
using WorkflowService.Persistence;

namespace WorkflowService.Tests.Infrastructure;

public class TestWebApplicationFactory : WebApplicationFactory<WorkflowService.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Swap DbContexts (Workflow + shared ApplicationDbContext) to InMemory
            SwapDb<WorkflowDbContext>(services);
            SwapDb<ApplicationDbContext>(services);

            void SwapDb<TCtx>(IServiceCollection svc) where TCtx : DbContext
            {
                var descriptors = svc.Where(d => d.ServiceType == typeof(DbContextOptions<TCtx>)).ToList();
                foreach (var d in descriptors) svc.Remove(d);

                svc.AddDbContext<TCtx>(o =>
                    o.UseInMemoryDatabase($"{typeof(TCtx).Name}-wf-tests-{Guid.NewGuid():N}")
                     .EnableSensitiveDataLogging());
            }

            // Remove hosted services (background workers) for deterministic tests
            RemoveHosted<OutboxBackgroundWorker>(services);
            RemoveHosted<OutboxIdempotencyBackfillWorker>(services);
            RemoveHosted<TimerWorker>(services);
            RemoveHosted<JoinTimeoutWorker>(services);

            void RemoveHosted<THosted>(IServiceCollection svc)
            {
                var hosted = svc.FirstOrDefault(d =>
                    d.ServiceType == typeof(IHostedService) &&
                    d.ImplementationType == typeof(THosted));
                if (hosted != null) svc.Remove(hosted);
            }

            // Inject test authentication
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                    options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                    options.DefaultScheme = TestAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
        });
    }
}
