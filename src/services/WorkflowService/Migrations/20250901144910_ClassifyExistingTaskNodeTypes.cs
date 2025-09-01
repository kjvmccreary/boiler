using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace WorkflowService.Migrations
{
    public partial class ClassifyExistingTaskNodeTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Any task whose NodeId starts with 'timer-' is a timer
            migrationBuilder.Sql(@"
                UPDATE ""WorkflowTasks""
                SET ""NodeType""='timer'
                WHERE ""NodeType"" <> 'timer' AND LOWER(""NodeId"") LIKE 'timer-%';
            ");

            // All remaining rows that are not classified set to human (if empty or something else)
            migrationBuilder.Sql(@"
                UPDATE ""WorkflowTasks""
                SET ""NodeType""='human'
                WHERE (""NodeType"" IS NULL OR ""NodeType"" = '' OR ""NodeType"" NOT IN ('human','timer'));
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op rollback (leave classifications)
        }
    }
}
