using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class FixRLSPolicieForCrossTenantUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing restrictive policies
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS tenant_isolation_policy_users ON ""Users"";
                DROP POLICY IF EXISTS tenant_isolation_policy_user_roles ON ""UserRoles"";
            ");

            // Create new cross-tenant friendly policies

            // Users policy: Users can be accessed if they have access to the current tenant
            migrationBuilder.Sql(@"
                CREATE POLICY cross_tenant_users_policy ON ""Users""
                    USING (
                        -- Allow access to users who have roles in the current tenant
                        EXISTS (
                            SELECT 1 FROM ""UserRoles"" ur 
                            WHERE ur.""UserId"" = ""Users"".""Id"" 
                            AND ur.""TenantId""::text = current_setting('app.tenant_id', true)
                        )
                        OR
                        -- Allow access to users who are members of the current tenant (legacy)
                        EXISTS (
                            SELECT 1 FROM ""TenantUsers"" tu
                            WHERE tu.""UserId"" = ""Users"".""Id""
                            AND tu.""TenantId""::text = current_setting('app.tenant_id', true)
                        )
                        OR
                        -- Allow access if no tenant context is set (system operations)
                        current_setting('app.tenant_id', true) = ''
                    );
            ");

            // UserRoles policy: Allow access to roles for the current tenant only
            migrationBuilder.Sql(@"
                CREATE POLICY tenant_scoped_user_roles_policy ON ""UserRoles""
                    USING (
                        ""TenantId""::text = current_setting('app.tenant_id', true)
                        OR 
                        current_setting('app.tenant_id', true) = ''
                    );
            ");

            // Add a function to check if a user has access to a tenant
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION user_has_tenant_access(user_id INTEGER, tenant_id INTEGER)
                RETURNS BOOLEAN AS $$
                BEGIN
                    -- Check if user has any roles in the tenant
                    RETURN EXISTS (
                        SELECT 1 FROM ""UserRoles"" ur 
                        WHERE ur.""UserId"" = user_id 
                        AND ur.""TenantId"" = tenant_id
                        AND ur.""IsActive"" = true
                    ) OR EXISTS (
                        -- Check legacy TenantUsers table
                        SELECT 1 FROM ""TenantUsers"" tu
                        WHERE tu.""UserId"" = user_id
                        AND tu.""TenantId"" = tenant_id
                        AND tu.""IsActive"" = true
                    );
                END;
                $$ LANGUAGE plpgsql;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the cross-tenant policies
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS cross_tenant_users_policy ON ""Users"";
                DROP POLICY IF EXISTS tenant_scoped_user_roles_policy ON ""UserRoles"";
                DROP FUNCTION IF EXISTS user_has_tenant_access(INTEGER, INTEGER);
            ");

            // Restore original restrictive policies
            migrationBuilder.Sql(@"
                CREATE POLICY tenant_isolation_policy_users ON ""Users""
                    USING (""TenantId""::text = current_setting('app.tenant_id', true));
                    
                CREATE POLICY tenant_isolation_policy_user_roles ON ""UserRoles""
                    USING (""TenantId""::text = current_setting('app.tenant_id', true));
            ");
        }
    }
}
