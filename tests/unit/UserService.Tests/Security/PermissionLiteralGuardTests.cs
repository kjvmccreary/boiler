using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

public class PermissionLiteralGuardTests
{
    [Fact]
    public void No_Raw_Permission_Literals_In_UserService_Controllers()
    {
        var root = GetServiceRoot("UserService");
        var offenders = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(f => f.Contains("Controllers"))
            .Select(f => new { f, text = File.ReadAllText(f) })
            .Where(x =>
                x.text.Contains("RequiresPermission(\"users.", System.StringComparison.OrdinalIgnoreCase) ||
                x.text.Contains("RequiresPermission(\"compliance.", System.StringComparison.OrdinalIgnoreCase))
            .Select(x => x.f)
            .ToList();

        offenders.Should().BeEmpty("controllers must use Permissions.* constants instead of raw literals");
    }

    private static string GetServiceRoot(string serviceName)
    {
        var dir = Directory.GetCurrentDirectory();
        while (!Directory.GetFiles(dir, "boiler.sln").Any())
            dir = Directory.GetParent(dir)!.FullName;
        var path = Path.Combine(dir, "src", "services", serviceName);
        Directory.Exists(path).Should().BeTrue();
        return path;
    }
}
