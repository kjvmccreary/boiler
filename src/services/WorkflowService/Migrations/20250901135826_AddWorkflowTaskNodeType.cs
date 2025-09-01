using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    public partial class AddWorkflowTaskNodeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NodeType",
                table: "WorkflowTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "human");

            // Drop any existing tenant constraint (old strict or failed attempt)
            migrationBuilder.Sql(@"ALTER TABLE ""WorkflowTasks"" DROP CONSTRAINT IF EXISTS ""CK_WorkflowTask_TenantId"";");

            // Backfill (still optional)
            migrationBuilder.Sql(@"
                UPDATE ""WorkflowTasks""
                SET ""NodeType""='timer'
                WHERE LOWER(""TaskName"") LIKE '%timer%' AND ""NodeType""='human';
            ");

            // Add permissive constraint: if GUC not set -> pass; if set -> must match
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkflowTasks""
                ADD CONSTRAINT ""CK_WorkflowTask_TenantId""
                CHECK (
                    current_setting('app.tenant_id', true) IS NULL
                    OR ""TenantId"" = current_setting('app.tenant_id', true)::int
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE ""WorkflowTasks"" DROP CONSTRAINT IF EXISTS ""CK_WorkflowTask_TenantId"";");

            migrationBuilder.DropColumn(
                name: "NodeType",
                table: "WorkflowTasks");

            // (Optional) restore original strict constraint if you really want it back:
            // migrationBuilder.Sql(@"
            //     ALTER TABLE ""WorkflowTasks""
            //     ADD CONSTRAINT ""CK_WorkflowTask_TenantId""
            //     CHECK (""TenantId"" = current_setting('app.tenant_id')::int);
            // ");
        }
    }
}
