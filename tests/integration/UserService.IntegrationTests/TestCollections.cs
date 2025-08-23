using Xunit;
using UserService.IntegrationTests.Fixtures;

namespace UserService.IntegrationTests;

/// <summary>
/// 🔧 CRITICAL FIX: Single shared test collection to prevent multiple application instances
/// All integration tests should use this collection to share the same WebApplicationTestFixture
/// 
/// This solves the "11 application instances" problem by ensuring all tests use the same fixture
/// </summary>
[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<WebApplicationTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

// 🚨 REMOVE: Delete all the individual collections that were causing multiple instances
// ❌ Remove WebApplicationTestsCollection
// ❌ Remove UsersControllerCollection  
// ❌ Remove RolesControllerCollection
// ❌ Remove CachePerformanceCollection

// All tests should now use [Collection("Integration Tests")] instead
