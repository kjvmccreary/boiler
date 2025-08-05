// FILE: src/shared/Common/Migrations/20250805203738_FixRLSIndexes.cs
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class FixRLSIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create performance indexes with correct column names
            migrationBuilder.Sql(@"
                -- Performance indexes for tenant isolation
                CREATE INDEX IF NOT EXISTS idx_users_tenant_active ON ""Users"" (""TenantId"", ""IsActive"");
                CREATE INDEX IF NOT EXISTS idx_users_email_domain ON ""Users"" (""Email"");
                CREATE INDEX IF NOT EXISTS idx_users_created_at ON ""Users"" (""CreatedAt"");
                
                -- Refresh token performance indexes (CORRECTED: ExpiryDate not ExpiresAt)
                CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expiry ON ""RefreshTokens"" (""ExpiryDate"");
                CREATE INDEX IF NOT EXISTS idx_refresh_tokens_active ON ""RefreshTokens"" (""IsRevoked"", ""ExpiryDate"");
                CREATE INDEX IF NOT EXISTS idx_refresh_tokens_tenant_user ON ""RefreshTokens"" (""TenantId"", ""UserId"");
                
                -- Tenant user relationship indexes
                CREATE INDEX IF NOT EXISTS idx_tenant_users_active ON ""TenantUsers"" (""TenantId"", ""IsActive"");
                CREATE INDEX IF NOT EXISTS idx_tenant_users_role ON ""TenantUsers"" (""Role"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the indexes
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS idx_users_tenant_active;
                DROP INDEX IF EXISTS idx_users_email_domain;
                DROP INDEX IF EXISTS idx_users_created_at;
                DROP INDEX IF EXISTS idx_refresh_tokens_expiry;
                DROP INDEX IF EXISTS idx_refresh_tokens_active;
                DROP INDEX IF EXISTS idx_refresh_tokens_tenant_user;
                DROP INDEX IF EXISTS idx_tenant_users_active;
                DROP INDEX IF EXISTS idx_tenant_users_role;
            ");
        }
    }
}
