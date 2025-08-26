using Microsoft.AspNetCore.Authorization;
using Common.Constants; // 🔧 FIX: Use existing permissions

namespace WorkflowService.Security;

/// <summary>
/// Workflow-specific authorization policies
/// </summary>
public static class WorkflowPolicies
{
    /// <summary>
    /// Policy names for authorization requirements
    /// </summary>
    public static class Policies
    {
        public const string CanViewDefinitions = "CanViewDefinitions";
        public const string CanManageDefinitions = "CanManageDefinitions";
        public const string CanPublishDefinitions = "CanPublishDefinitions";
        
        public const string CanViewInstances = "CanViewInstances";
        public const string CanManageInstances = "CanManageInstances";
        
        public const string CanViewTasks = "CanViewTasks";
        public const string CanViewAllTasks = "CanViewAllTasks";
        public const string CanManageTasks = "CanManageTasks";
        
        public const string IsWorkflowAdmin = "IsWorkflowAdmin";
        public const string CanPerformBulkOperations = "CanPerformBulkOperations";
        public const string CanViewAnalytics = "CanViewAnalytics";
    }

    /// <summary>
    /// Configure workflow authorization policies using existing permissions
    /// </summary>
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        // Definition Policies
        options.AddPolicy(Policies.CanViewDefinitions, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.ViewDefinitions) ||
                context.User.HasClaim("permission", Permissions.Workflow.Read) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        options.AddPolicy(Policies.CanManageDefinitions, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.CreateDefinitions) ||
                context.User.HasClaim("permission", Permissions.Workflow.EditDefinitions) ||
                context.User.HasClaim("permission", Permissions.Workflow.Write) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        options.AddPolicy(Policies.CanPublishDefinitions, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.PublishDefinitions) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        // Instance Policies
        options.AddPolicy(Policies.CanViewInstances, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.ViewInstances) ||
                context.User.HasClaim("permission", Permissions.Workflow.Read) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        options.AddPolicy(Policies.CanManageInstances, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.StartInstances) ||
                context.User.HasClaim("permission", Permissions.Workflow.ManageInstances) ||
                context.User.HasClaim("permission", Permissions.Workflow.SignalInstances) ||
                context.User.HasClaim("permission", Permissions.Workflow.TerminateInstances) ||
                context.User.HasClaim("permission", Permissions.Workflow.Write) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        // Task Policies
        options.AddPolicy(Policies.CanViewTasks, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.ViewTasks) ||
                context.User.HasClaim("permission", Permissions.Workflow.Read) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        options.AddPolicy(Policies.CanViewAllTasks, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.ViewAllTasks) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        options.AddPolicy(Policies.CanManageTasks, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.ClaimTasks) ||
                context.User.HasClaim("permission", Permissions.Workflow.CompleteTasks) ||
                context.User.HasClaim("permission", Permissions.Workflow.ReassignTasks) ||
                context.User.HasClaim("permission", Permissions.Workflow.Write) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        // Admin Policies
        options.AddPolicy(Policies.IsWorkflowAdmin, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.Admin) ||
                context.User.HasClaim("permission", Permissions.Workflow.AdminOperations)));

        options.AddPolicy(Policies.CanPerformBulkOperations, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.BulkOperations) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));

        options.AddPolicy(Policies.CanViewAnalytics, policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim("permission", Permissions.Workflow.ViewAnalytics) ||
                context.User.HasClaim("permission", Permissions.Workflow.Admin)));
    }
}
