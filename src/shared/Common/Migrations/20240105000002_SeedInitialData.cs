// FILE: src/shared/Common/Migrations/20240105000002_SeedInitialData.cs
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert default tenant
            migrationBuilder.Sql(@"
                INSERT INTO ""Tenants"" (""Name"", ""Domain"", ""SubscriptionPlan"", ""IsActive"", ""Settings"", ""CreatedAt"", ""UpdatedAt"")
                VALUES (
                    'Default Tenant',
                    'localhost',
                    'Basic',
                    true,
                    '{""theme"":""default"",""features"":[""users"",""auth""]}',
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP
                )
                ON CONFLICT (""Domain"") DO NOTHING;
            ");

            // Create an admin user for the default tenant
            // Note: In a real application, you would generate a proper password hash
            // This is just for development purposes
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    default_tenant_id INTEGER;
                    admin_user_id INTEGER;
                BEGIN
                    -- Get the default tenant ID
                    SELECT ""Id"" INTO default_tenant_id FROM ""Tenants"" WHERE ""Domain"" = 'localhost';
                    
                    -- Set tenant context for RLS
                    PERFORM set_config('app.current_tenant', default_tenant_id::text, false);
                    
                    -- Insert admin user if not exists
                    INSERT INTO ""Users"" (
                        ""TenantId"", 
                        ""Email"", 
                        ""FirstName"", 
                        ""LastName"", 
                        ""PasswordHash"", 
                        ""IsActive"", 
                        ""EmailConfirmed"", 
                        ""CreatedAt"", 
                        ""UpdatedAt""
                    )
                    VALUES (
                        default_tenant_id::uuid,
                        'admin@localhost',
                        'System',
                        'Administrator',
                        '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeKMVTcOoqc8WITq2', -- 'password' hashed with BCrypt
                        true,
                        true,
                        CURRENT_TIMESTAMP,
                        CURRENT_TIMESTAMP
                    )
                    ON CONFLICT (""TenantId"", ""Email"") DO NOTHING
                    RETURNING ""Id"" INTO admin_user_id;
                    
                    -- If user was just created, get the ID
                    IF admin_user_id IS NULL THEN
                        SELECT ""Id"" INTO admin_user_id 
                        FROM ""Users"" 
                        WHERE ""TenantId"" = default_tenant_id::uuid AND ""Email"" = 'admin@localhost';
                    END IF;
                    
                    -- Create tenant-user relationship with SuperAdmin role
                    INSERT INTO ""TenantUsers"" (
                        ""TenantId"", 
                        ""UserId"", 
                        ""Role"", 
                        ""IsActive"", 
                        ""JoinedAt"", 
                        ""CreatedAt"", 
                        ""UpdatedAt""
                    )
                    VALUES (
                        default_tenant_id::uuid,
                        admin_user_id,
                        'SuperAdmin',
                        true,
                        CURRENT_TIMESTAMP,
                        CURRENT_TIMESTAMP,
                        CURRENT_TIMESTAMP
                    )
                    ON CONFLICT (""TenantId"", ""UserId"") DO NOTHING;
                    
                    -- Clear tenant context
                    PERFORM set_config('app.current_tenant', '', false);
                END $$;
            ");

            // Create development tenant if in development environment
            migrationBuilder.Sql(@"
                INSERT INTO ""Tenants"" (""Name"", ""Domain"", ""SubscriptionPlan"", ""IsActive"", ""Settings"", ""CreatedAt"", ""UpdatedAt"")
                VALUES (
                    'Development Tenant',
                    'dev.localhost',
                    'Premium',
                    true,
                    '{""theme"":""dark"",""features"":[""users"",""auth"",""analytics""]}',
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP
                )
                ON CONFLICT (""Domain"") DO NOTHING;
            ");

            // Create demo tenant
            migrationBuilder.Sql(@"
                INSERT INTO ""Tenants"" (""Name"", ""Domain"", ""SubscriptionPlan"", ""IsActive"", ""Settings"", ""CreatedAt"", ""UpdatedAt"")
                VALUES (
                    'Demo Tenant',
                    'demo.localhost',
                    'Basic',
                    true,
                    '{""theme"":""light"",""features"":[""users"",""auth""],""demo"":true}',
                    CURRENT_TIMESTAMP,
                    CURRENT_TIMESTAMP
                )
                ON CONFLICT (""Domain"") DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Clean up seed data
            migrationBuilder.Sql(@"
                DELETE FROM ""TenantUsers"" WHERE ""TenantId"" IN (
                    SELECT ""Id""::uuid FROM ""Tenants"" 
                    WHERE ""Domain"" IN ('localhost', 'dev.localhost', 'demo.localhost')
                );
            ");

            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    tenant_ids INTEGER[];
                BEGIN
                    -- Get tenant IDs to clean up
                    SELECT ARRAY(SELECT ""Id"" FROM ""Tenants"" WHERE ""Domain"" IN ('localhost', 'dev.localhost', 'demo.localhost'))
                    INTO tenant_ids;
                    
                    -- Clear tenant context
                    PERFORM set_config('app.current_tenant', '', false);
                    
                    -- Delete users from these tenants
                    DELETE FROM ""Users"" WHERE ""TenantId""::integer = ANY(tenant_ids);
                    
                    -- Delete the tenants
                    DELETE FROM ""Tenants"" WHERE ""Id"" = ANY(tenant_ids);
                END $$;
            ");
        }
    }
}
