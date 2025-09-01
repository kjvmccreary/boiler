using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    public partial class AddWorkflowTaskTenantTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop any existing tenant CHECK constraint (permissive or strict)
            migrationBuilder.Sql(@"ALTER TABLE ""WorkflowTasks"" DROP CONSTRAINT IF EXISTS ""CK_WorkflowTask_TenantId"";");

            // 2. (Optional) Drop old trigger if a previous attempt existed
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_enforce_tenant_workflowtasks ON ""WorkflowTasks"";");

            // 3. Create / replace tenant enforcement function (idempotent)
            migrationBuilder.Sql(@"
CREATE OR REPLACE FUNCTION enforce_tenant()
RETURNS trigger
LANGUAGE plpgsql
AS $$
DECLARE
    g_tenant TEXT;
BEGIN
    g_tenant := current_setting('app.tenant_id', true);
    IF g_tenant IS NULL OR trim(g_tenant) = '' THEN
        RAISE EXCEPTION 'Tenant GUC app.tenant_id not set for table %', TG_TABLE_NAME
            USING ERRCODE = '23514';
    END IF;

    IF NEW.""TenantId"" <> g_tenant::int THEN
        RAISE EXCEPTION 'Tenant mismatch (row=% application=%) on table %',
            NEW.""TenantId"", g_tenant, TG_TABLE_NAME
            USING ERRCODE = '23514';
    END IF;

    RETURN NEW;
END;
$$;
");

            // 4. Create the trigger to enforce tenant integrity on INSERT/UPDATE
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_enforce_tenant_workflowtasks
                BEFORE INSERT OR UPDATE ON ""WorkflowTasks""
                FOR EACH ROW
                EXECUTE FUNCTION enforce_tenant();
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the trigger
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS trg_enforce_tenant_workflowtasks ON ""WorkflowTasks"";");

            // Optionally (re)introduce a CHECK constraint (choose one style).
            // Here we restore the strict form to keep isolation if rolled back.
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkflowTasks""
                ADD CONSTRAINT ""CK_WorkflowTask_TenantId""
                CHECK (""TenantId"" = current_setting('app.tenant_id')::int);
            ");

            // NOTE: We intentionally keep the enforce_tenant() function in place because
            // other tables may adopt it. If you want to remove it ONLY when unused:
            // migrationBuilder.Sql(""DROP FUNCTION IF EXISTS enforce_tenant();"");
        }
    }
}
