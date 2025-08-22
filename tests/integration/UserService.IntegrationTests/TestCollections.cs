using Xunit;
using UserService.IntegrationTests.Fixtures; // ✅ FIX: Add missing using directive

namespace UserService.IntegrationTests;

// ✅ FIXED: Create separate test collections with proper isolation
[CollectionDefinition("WebApplicationTests", DisableParallelization = false)]
public class WebApplicationTestsCollection : ICollectionFixture<WebApplicationTestFixture>
{
}

// ✅ INDIVIDUAL COLLECTIONS: Separate collections for better isolation
[CollectionDefinition("UsersController", DisableParallelization = false)]
public class UsersControllerCollection : ICollectionFixture<WebApplicationTestFixture>
{
}

[CollectionDefinition("RolesController", DisableParallelization = false)]
public class RolesControllerCollection : ICollectionFixture<WebApplicationTestFixture>
{
}

[CollectionDefinition("CachePerformance", DisableParallelization = false)]
public class CachePerformanceCollection : ICollectionFixture<WebApplicationTestFixture>
{
}
