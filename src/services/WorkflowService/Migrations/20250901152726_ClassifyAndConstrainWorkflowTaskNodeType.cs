using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    public partial class ClassifyAndConstrainWorkflowTaskNodeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop prior NodeType constraint if it exists (idempotent)
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkflowTasks"" 
                DROP CONSTRAINT IF EXISTS ""CK_WorkflowTask_NodeType"";
            ");

            // 2. Classify timer tasks first (prefix heuristic)
            migrationBuilder.Sql(@"
                UPDATE ""WorkflowTasks""
                SET ""NodeType"" = 'timer'
                WHERE lower(""NodeId"") LIKE 'timer-%'
                  AND (""NodeType"" IS NULL OR trim(""NodeType"") = '' OR ""NodeType"" <> 'timer');
            ");

            // 3. Set everything else that is null/empty/invalid to 'human'
            migrationBuilder.Sql(@"
                UPDATE ""WorkflowTasks""
                SET ""NodeType"" = 'human'
                WHERE ""NodeType"" IS NULL
                   OR trim(""NodeType"") = ''
                   OR ""NodeType"" NOT IN ('human','timer');
            ");

            // 4. Create (or replace) helper classification function (idempotent)
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION classify_workflowtask_nodetype()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    IF NEW.""NodeType"" IS NULL OR trim(NEW.""NodeType"") = '' THEN
                        IF lower(NEW.""NodeId"") LIKE 'timer-%' THEN
                            NEW.""NodeType"" := 'timer';
                        ELSE
                            NEW.""NodeType"" := 'human';
                        END IF;
                    END IF;
                    RETURN NEW;
                END;
                $$;
            ");

            // 5. Drop + recreate trigger to auto-classify on insert/update
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trg_classify_nodetype_workflowtasks ON ""WorkflowTasks"";
                CREATE TRIGGER trg_classify_nodetype_workflowtasks
                BEFORE INSERT OR UPDATE ON ""WorkflowTasks""
                FOR EACH ROW
                EXECUTE FUNCTION classify_workflowtask_nodetype();
            ");

            // 6. Add strict check constraint (after data is clean)
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkflowTasks""
                ADD CONSTRAINT ""CK_WorkflowTask_NodeType""
                CHECK (""NodeType"" IN ('human','timer'));
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove constraint
            migrationBuilder.Sql(@"
                ALTER TABLE ""WorkflowTasks""
                DROP CONSTRAINT IF EXISTS ""CK_WorkflowTask_NodeType"";
            ");

            // Remove trigger (leave function in case other tables might reuse it; drop if you prefer)
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS trg_classify_nodetype_workflowtasks ON ""WorkflowTasks"";
            ");

            // (Optional) Drop function if you want a fully clean rollback:
            // migrationBuilder.Sql(@"
            //     DROP FUNCTION IF EXISTS classify_workflowtask_nodetype();
            // ");
        }
    }
}
