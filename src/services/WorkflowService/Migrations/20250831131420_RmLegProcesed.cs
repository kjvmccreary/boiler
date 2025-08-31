using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    public partial class RmLegProcesed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop legacy indexes
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Processed_CreatedAt",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Type_Processed",
                table: "OutboxMessages");

            // 2. Drop legacy 'Processed' column
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "OutboxMessages");

            // 3. Ensure EventType length constraint
            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "OutboxMessages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            // 4. Drop any existing DEFAULT on EventData (unknown name → ALTER COLUMN ... DROP DEFAULT)
            migrationBuilder.Sql(@"
ALTER TABLE ""OutboxMessages""
ALTER COLUMN ""EventData"" DROP DEFAULT;
");

            // 5. Convert EventData text → jsonb with USING and safe coercion
            migrationBuilder.Sql(@"
ALTER TABLE ""OutboxMessages""
ALTER COLUMN ""EventData"" TYPE jsonb
USING
    CASE
        WHEN ""EventData"" IS NULL OR btrim(""EventData"") = '' THEN '{}'::jsonb
        ELSE ""EventData""::jsonb
    END;
");

            // 6. (Optional) Set a jsonb default to keep non-null invariant if desired
            // Comment out if you prefer no default.
            migrationBuilder.Sql(@"
ALTER TABLE ""OutboxMessages""
ALTER COLUMN ""EventData"" SET DEFAULT '{}'::jsonb;
");

            // 7. New indexes on IsProcessed
            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_EventType_IsProcessed",
                table: "OutboxMessages",
                columns: new[] { "EventType", "IsProcessed" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_IsProcessed_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "IsProcessed", "CreatedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new indexes
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_EventType_IsProcessed",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_IsProcessed_CreatedAt",
                table: "OutboxMessages");

            // Revert EventType
            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "OutboxMessages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            // Remove jsonb default before reverting type
            migrationBuilder.Sql(@"
ALTER TABLE ""OutboxMessages""
ALTER COLUMN ""EventData"" DROP DEFAULT;
");

            // Revert EventData jsonb → text
            migrationBuilder.Sql(@"
ALTER TABLE ""OutboxMessages""
ALTER COLUMN ""EventData"" TYPE text
USING ""EventData""::text;
");

            // Re-add legacy 'Processed' column & indexes
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "OutboxMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Processed_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "Processed", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Type_Processed",
                table: "OutboxMessages",
                columns: new[] { "Type", "Processed" });
        }
    }
}
