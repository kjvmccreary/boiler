using System;

namespace Common.Constants;

/// <summary>
/// Defines all available permissions in the system.
/// Permissions are compile-time constants that define what actions can be performed.
/// Roles are runtime entities that group these permissions together.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// User management permissions
    /// </summary>
    public static class Users
    {
        public const string View = "users.view";
        public const string Edit = "users.edit";
        public const string Delete = "users.delete";
        public const string Create = "users.create";
        public const string ViewAll = "users.view_all";
        public const string ManageRoles = "users.manage_roles";
    }

    /// <summary>
    /// Role and permission management
    /// </summary>
    public static class Roles
    {
        public const string View = "roles.view";
        public const string Create = "roles.create";
        public const string Edit = "roles.edit";
        public const string Delete = "roles.delete";
        public const string AssignUsers = "roles.assign_users";
        public const string ManagePermissions = "roles.manage_permissions";
    }

    /// <summary>
    /// Tenant management permissions (usually for system admins)
    /// </summary>
    public static class Tenants
    {
        public const string View = "tenants.view";
        public const string Create = "tenants.create";
        public const string Edit = "tenants.edit";
        public const string Delete = "tenants.delete";
        public const string Initialize = "tenants.initialize";
        public const string ViewAll = "tenants.view_all";
        public const string ManageSettings = "tenants.manage_settings";
    }

    /// <summary>
    /// System administration permissions
    /// </summary>
    public static class System
    {
        public const string ViewLogs = "system.view_logs";
        public const string ManageSettings = "system.manage_settings";
        public const string ViewMetrics = "system.view_metrics";
        public const string ManageBackups = "system.manage_backups";
        public const string Manage = "system.manage"; // 🔧 ADD: For MonitoringController
        public const string Monitor = "system.monitor"; // 🔧 ADD: For monitoring endpoints
    }

    /// <summary>
    /// Report and analytics permissions
    /// </summary>
    public static class Reports
    {
        public const string View = "reports.view";
        public const string Create = "reports.create";
        public const string Export = "reports.export";
        public const string Schedule = "reports.schedule";
    }

    /// <summary>
    /// Billing and subscription management
    /// </summary>
    public static class Billing
    {
        public const string View = "billing.view";
        public const string Manage = "billing.manage";
        public const string ViewInvoices = "billing.view_invoices";
        public const string ProcessPayments = "billing.process_payments";
    }

    /// <summary>
    /// Permission entity management permissions
    /// </summary>
    public static class PermissionManagement
    {
        public const string View = "permissions.view";
        public const string Create = "permissions.create";
        public const string Edit = "permissions.edit";
        public const string Delete = "permissions.delete";
        public const string Manage = "permissions.manage";
    }

    /// <summary>
    /// Workflow management permissions
    /// </summary>
    public static class Workflow
    {
        public const string Read = "workflow.read";
        public const string Write = "workflow.write";
        public const string Admin = "workflow.admin";
        public const string ViewDefinitions = "workflow.view_definitions";
        public const string CreateDefinitions = "workflow.create_definitions";
        public const string PublishDefinitions = "workflow.publish_definitions";
        public const string ViewInstances = "workflow.view_instances";
        public const string StartInstances = "workflow.start_instances";
        public const string ManageInstances = "workflow.manage_instances";
        public const string ViewTasks = "workflow.view_tasks";
        public const string ClaimTasks = "workflow.claim_tasks";
        public const string CompleteTasks = "workflow.complete_tasks";
        public const string ViewAllTasks = "workflow.view_all_tasks";
        public const string AdminOperations = "workflow.admin_operations";
    }

    /// <summary>
    /// Get all permission values as a list for seeding database
    /// </summary>
    public static List<string> GetAllPermissions()
    {
        var permissions = new List<string>();
        
        // Use reflection to get all const string fields from nested classes
        var permissionClasses = typeof(Permissions).GetNestedTypes();
        
        foreach (var permClass in permissionClasses)
        {
            var fields = permClass.GetFields()
                .Where(f => f.IsLiteral && f.FieldType == typeof(string));
            
            foreach (var field in fields)
            {
                var value = field.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    permissions.Add(value);
                }
            }
        }
        
        return permissions.OrderBy(p => p).ToList();
    }

    /// <summary>
    /// Get permissions grouped by category for UI display
    /// </summary>
    public static Dictionary<string, List<string>> GetPermissionsByCategory()
    {
        var result = new Dictionary<string, List<string>>();
        var permissionClasses = typeof(Permissions).GetNestedTypes();
        
        foreach (var permClass in permissionClasses)
        {
            var categoryName = permClass.Name;
            var categoryPermissions = new List<string>();
            
            var fields = permClass.GetFields()
                .Where(f => f.IsLiteral && f.FieldType == typeof(string));
            
            foreach (var field in fields)
            {
                var value = field.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    categoryPermissions.Add(value);
                }
            }
            
            if (categoryPermissions.Any())
            {
                result[categoryName] = categoryPermissions.OrderBy(p => p).ToList();
            }
        }
        
        return result;
    }

    /// <summary>
    /// Check if a permission string is valid
    /// </summary>
    public static bool IsValidPermission(string permission)
    {
        return GetAllPermissions().Contains(permission);
    }
}
