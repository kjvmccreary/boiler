using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Constants;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.IntegrationTests.Fixtures;
using Xunit;

namespace UserService.IntegrationTests.Infrastructure;

public class TestSetupVerificationTests : TestBase
{
    public TestSetupVerificationTests(WebApplicationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task TestSetup_ShouldCreateAllRequiredTestData()
    {
        var permissions = await _dbContext.Permissions.ToListAsync();
        var users = await _dbContext.Users.ToListAsync();
        var roles = await _dbContext.Roles.ToListAsync();
        var tenants = await _dbContext.Tenants.ToListAsync();

        tenants.Should().HaveCount(2, "Should have 2 test tenants");
        users.Should().HaveCount(7, "Should have 7 test users");
        roles.Should().HaveCount(8, "Should have 8 roles");

        // Legacy static count (deprecated) – left commented if you want quick fallback:
        // permissions.Should().HaveCount(65, "Original baseline permissions count");

        // Dynamic permission validation
        var constantPermissions = Permissions.GetAllPermissions();
        var dbPermissionNames = permissions.Select(p => p.Name).ToList();

        var missingInDb = constantPermissions
            .Except(dbPermissionNames, StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p)
            .ToList();

        var extraInDb = dbPermissionNames
            .Except(constantPermissions, StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p)
            .ToList();

        // Ensure we did not accidentally shrink the permission surface
        const int MinimumBaseline = 65;
        constantPermissions.Count.Should()
            .BeGreaterOrEqualTo(MinimumBaseline, $"permission constant count should not drop below baseline {MinimumBaseline}");

        // Fail if any new constants were not seeded
        missingInDb.Should().BeEmpty($"""
            All permission constants must be seeded.
            Missing (not in DB): {string.Join(", ", missingInDb)}
            Total constants: {constantPermissions.Count}
            Seeded: {dbPermissionNames.Count}
            """);

        // Optionally enforce no orphan (DB-only) permissions after transition period:
        // extraInDb.Should().BeEmpty($"DB has permissions not defined in constants: {string.Join(", ", extraInDb)}");

        _logger.LogInformation("✅ Permissions seeded: {Seeded}/{Defined} (Missing={Missing}, Extra={Extra})",
            dbPermissionNames.Count, constantPermissions.Count,
            missingInDb.Count, extraInDb.Count);
    }
}
