using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class AddRLSPoliciesTakeTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create tenant context functions first
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION set_tenant_context(tenant_id TEXT)
                RETURNS VOID AS $$
                BEGIN
                    PERFORM set_config('app.tenant_id', tenant_id, false);
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_current_tenant()
                RETURNS TEXT AS $$
                BEGIN
                    RETURN current_setting('app.tenant_id', true);
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Enable RLS on Users table
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_users ON ""Users""
                    USING (""TenantId""::text = current_setting('app.tenant_id', true));
            ");

            // Enable RLS on Roles table (allow system roles)
            migrationBuilder.Sql(@"
                ALTER TABLE ""Roles"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_roles ON ""Roles""
                    USING (
                        ""TenantId""::text = current_setting('app.tenant_id', true) 
                        OR ""IsSystemRole"" = true
                    );
            ");

            // Enable RLS on UserRoles table
            migrationBuilder.Sql(@"
                ALTER TABLE ""UserRoles"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_user_roles ON ""UserRoles""
                    USING (""TenantId""::text = current_setting('app.tenant_id', true));
            ");

            // Enable RLS on RefreshTokens table
            migrationBuilder.Sql(@"
                ALTER TABLE ""RefreshTokens"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_refresh_tokens ON ""RefreshTokens""
                    USING (""TenantId""::text = current_setting('app.tenant_id', true));
            ");

            // Enable RLS on AuditEntries table
            migrationBuilder.Sql(@"
                ALTER TABLE ""AuditEntries"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_audit_entries ON ""AuditEntries""
                    USING (""TenantId""::text = current_setting('app.tenant_id', true));
            ");

            // Enable RLS on RolePermissions table
            migrationBuilder.Sql(@"
                ALTER TABLE ""RolePermissions"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_role_permissions ON ""RolePermissions""
                    USING (EXISTS (
                        SELECT 1 FROM ""Roles"" r 
                        WHERE r.""Id"" = ""RolePermissions"".""RoleId"" 
                        AND (r.""TenantId""::text = current_setting('app.tenant_id', true) OR r.""IsSystemRole"" = true)
                    ));
            ");

            // Create performance indexes
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_users_tenant_id ON ""Users"" (""TenantId"");
                CREATE INDEX IF NOT EXISTS idx_roles_tenant_id ON ""Roles"" (""TenantId"");
                CREATE INDEX IF NOT EXISTS idx_user_roles_tenant_id ON ""UserRoles"" (""TenantId"");
                CREATE INDEX IF NOT EXISTS idx_refresh_tokens_tenant_id ON ""RefreshTokens"" (""TenantId"");
                CREATE INDEX IF NOT EXISTS idx_audit_entries_tenant_id ON ""AuditEntries"" (""TenantId"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop policies and disable RLS
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS tenant_isolation_policy_users ON ""Users"";
                DROP POLICY IF EXISTS tenant_isolation_policy_roles ON ""Roles"";
                DROP POLICY IF EXISTS tenant_isolation_policy_user_roles ON ""UserRoles"";
                DROP POLICY IF EXISTS tenant_isolation_policy_refresh_tokens ON ""RefreshTokens"";
                DROP POLICY IF EXISTS tenant_isolation_policy_audit_entries ON ""AuditEntries"";
                DROP POLICY IF EXISTS tenant_isolation_policy_role_permissions ON ""RolePermissions"";
                
                ALTER TABLE ""Users"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""Roles"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""UserRoles"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""RefreshTokens"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""AuditEntries"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""RolePermissions"" DISABLE ROW LEVEL SECURITY;
            ");

            // Drop functions
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS set_tenant_context(TEXT);
                DROP FUNCTION IF EXISTS get_current_tenant();
            ");

            // Drop indexes
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS idx_users_tenant_id;
                DROP INDEX IF EXISTS idx_roles_tenant_id;
                DROP INDEX IF EXISTS idx_user_roles_tenant_id;
                DROP INDEX IF EXISTS idx_refresh_tokens_tenant_id;
                DROP INDEX IF EXISTS idx_audit_entries_tenant_id;
            ");
        }
    }
}
