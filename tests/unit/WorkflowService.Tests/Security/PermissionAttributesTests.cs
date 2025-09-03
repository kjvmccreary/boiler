using System.Reflection;
using Common.Constants;
using Common.Authorization;
using FluentAssertions;
using Xunit;

namespace WorkflowService.Tests.Security;

public class PermissionAttributesTests
{
    private static readonly Assembly WorkflowAssembly = typeof(Program).Assembly;

    [Fact]
    public void All_RequiresPermission_Attributes_Should_Map_To_Defined_Permissions()
    {
        // Gather all const permissions
        var defined = Permissions.GetAllPermissions().ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Discover controller types in workflow service assembly
        var controllers = WorkflowAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller", StringComparison.Ordinal))
            .ToList();

        var missing = new List<string>();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var controller in controllers)
        {
            // Class-level
            var classPerms = controller.GetCustomAttributes<RequiresPermissionAttribute>(inherit: true)
                .Select(a => a.Policy)
                .Where(p => !string.IsNullOrWhiteSpace(p));

            foreach (var p in classPerms)
            {
                used.Add(p!);
                if (!defined.Contains(p!))
                    missing.Add($"{controller.Name} (class): {p}");
            }

            // Method-level
            var methods = controller
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (var m in methods)
            {
                var attrs = m.GetCustomAttributes<RequiresPermissionAttribute>(inherit: true)
                    .Select(a => a.Policy)
                    .Where(p => !string.IsNullOrWhiteSpace(p));

                foreach (var p in attrs)
                {
                    used.Add(p!);
                    if (!defined.Contains(p!))
                        missing.Add($"{controller.Name}.{m.Name}: {p}");
                }
            }
        }

        // Assert nothing missing
        missing.Should().BeEmpty("all permissions used by controllers must be defined in Permissions constants");

        // MVP baseline permissions should appear at least once
        var requiredMvp = new[]
        {
            Permissions.Workflow.ViewTasks,
            Permissions.Workflow.ClaimTasks,
            Permissions.Workflow.CompleteTasks,
            Permissions.Workflow.Admin,
            Permissions.Workflow.ViewInstances,
            Permissions.Workflow.StartInstances
        };

        requiredMvp.All(r => used.Contains(r))
            .Should().BeTrue("MVP required permission set must be referenced by at least one endpoint");
    }

    [Fact]
    public void No_Legacy_RequirePermissionAttribute_Is_Used()
    {
        // The legacy attribute (RequirePermissionAttribute) lives in the shared Common assembly, not necessarily the workflow service assembly.
        // This test should PASS if:
        //  (a) The legacy attribute type is not present in the loaded AppDomain at all (already removed) OR
        //  (b) It exists but is not applied to any workflow controllers / actions.

        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a =>
            {
                try { return !a.IsDynamic; } catch { return false; }
            })
            .ToList();

        var legacyAttributeType = allAssemblies
            .SelectMany(a =>
            {
                try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
            })
            .FirstOrDefault(t => t.Name == "RequirePermissionAttribute");

        // If it's gone completely, this is fine.
        if (legacyAttributeType == null)
        {
            return;
        }

        // If present, ensure it's not applied to any controllers or their public methods in the workflow assembly.
        var offendingUsages = new List<string>();

        var controllers = WorkflowAssembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller", StringComparison.Ordinal))
            .ToList();

        foreach (var controller in controllers)
        {
            // Class-level usage
            if (controller.GetCustomAttributes(inherit: true).Any(a => a.GetType() == legacyAttributeType))
            {
                offendingUsages.Add($"{controller.Name} (class)");
            }

            // Method-level usage
            var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var m in methods)
            {
                if (m.GetCustomAttributes(inherit: true).Any(a => a.GetType() == legacyAttributeType))
                {
                    offendingUsages.Add($"{controller.Name}.{m.Name}");
                }
            }
        }

        offendingUsages.Should().BeEmpty("legacy RequirePermissionAttribute should not be used; replace with RequiresPermissionAttribute. Found: " +
            string.Join(", ", offendingUsages));
    }
}
