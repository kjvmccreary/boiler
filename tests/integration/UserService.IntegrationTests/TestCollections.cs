using Xunit;
using UserService.IntegrationTests.Fixtures; // ✅ FIX: Add missing using directive

namespace UserService.IntegrationTests;

// ✅ FIX: Create separate test collections to prevent database sharing
[CollectionDefinition("RolesController")]
public class RolesControllerCollection : ICollectionFixture<WebApplicationTestFixture>
{
}

[CollectionDefinition("UsersController")] 
public class UsersControllerCollection : ICollectionFixture<WebApplicationTestFixture>
{
}

[CollectionDefinition("TestSetupVerification")]
public class TestSetupVerificationCollection : ICollectionFixture<WebApplicationTestFixture>
{
}
