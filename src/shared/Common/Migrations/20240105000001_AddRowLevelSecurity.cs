// FILE: src/shared/Common/Migrations/20240105000001_AddRowLevelSecurity.cs
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class AddRowLevelSecurity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enable Row Level Security on Users table
            migrationBuilder.Sql(@"
                ALTER TABLE ""Users"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_users ON ""Users""
                    USING (""TenantId""::text = current_setting('app.current_tenant', true));
            ");

            // Enable Row Level Security on RefreshTokens table
            migrationBuilder.Sql(@"
                ALTER TABLE ""RefreshTokens"" ENABLE ROW LEVEL SECURITY;
                
                CREATE POLICY tenant_isolation_policy_refresh_tokens ON ""RefreshTokens""
                    USING (""TenantId""::text = current_setting('app.current_tenant', true));
            ");

            // Create function to set tenant context
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION set_tenant_context(tenant_id TEXT)
                RETURNS VOID AS $$
                BEGIN
                    PERFORM set_config('app.current_tenant', tenant_id, false);
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Create function to get current tenant context
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_current_tenant()
                RETURNS TEXT AS $$
                BEGIN
                    RETURN current_setting('app.current_tenant', true);
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Create function to clear tenant context
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION clear_tenant_context()
                RETURNS VOID AS $$
                BEGIN
                    PERFORM set_config('app.current_tenant', '', false);
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Add audit functions for tracking changes
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_updated_at_column()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.""UpdatedAt"" = CURRENT_TIMESTAMP;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Create triggers for updating UpdatedAt timestamp
            migrationBuilder.Sql(@"
                CREATE TRIGGER update_tenants_updated_at BEFORE UPDATE ON ""Tenants""
                    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
                    
                CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON ""Users""
                    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
                    
                CREATE TRIGGER update_tenant_users_updated_at BEFORE UPDATE ON ""TenantUsers""
                    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
                    
                CREATE TRIGGER update_refresh_tokens_updated_at BEFORE UPDATE ON ""RefreshTokens""
                    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
            ");

            // Create indexes for performance
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_users_tenant_active ON ""Users"" (""TenantId"", ""IsActive"");
                CREATE INDEX IF NOT EXISTS idx_users_email_confirmed ON ""Users"" (""EmailConfirmed"");
                CREATE INDEX IF NOT EXISTS idx_users_last_login ON ""Users"" (""LastLoginAt"");
                CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expiry ON ""RefreshTokens"" (""ExpiryDate"");
                CREATE INDEX IF NOT EXISTS idx_refresh_tokens_active ON ""RefreshTokens"" (""IsRevoked"", ""ExpiryDate"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop triggers
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS update_tenants_updated_at ON ""Tenants"";
                DROP TRIGGER IF EXISTS update_users_updated_at ON ""Users"";
                DROP TRIGGER IF EXISTS update_tenant_users_updated_at ON ""TenantUsers"";
                DROP TRIGGER IF EXISTS update_refresh_tokens_updated_at ON ""RefreshTokens"";
            ");

            // Drop functions
            migrationBuilder.Sql(@"
                DROP FUNCTION IF EXISTS update_updated_at_column();
                DROP FUNCTION IF EXISTS set_tenant_context(TEXT);
                DROP FUNCTION IF EXISTS get_current_tenant();
                DROP FUNCTION IF EXISTS clear_tenant_context();
            ");

            // Drop policies and disable RLS
            migrationBuilder.Sql(@"
                DROP POLICY IF EXISTS tenant_isolation_policy_users ON ""Users"";
                DROP POLICY IF EXISTS tenant_isolation_policy_refresh_tokens ON ""RefreshTokens"";
                
                ALTER TABLE ""Users"" DISABLE ROW LEVEL SECURITY;
                ALTER TABLE ""RefreshTokens"" DISABLE ROW LEVEL SECURITY;
            ");

            // Drop indexes
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS idx_users_tenant_active;
                DROP INDEX IF EXISTS idx_users_email_confirmed;
                DROP INDEX IF EXISTS idx_users_last_login;
                DROP INDEX IF EXISTS idx_refresh_tokens_expiry;
                DROP INDEX IF EXISTS idx_refresh_tokens_active;
            ");
        }
    }
}
