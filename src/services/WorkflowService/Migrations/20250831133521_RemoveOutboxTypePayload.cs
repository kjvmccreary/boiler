using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    public partial class RemoveOutboxTypePayload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent block: only execute merge + drops if legacy columns still exist
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name = 'OutboxMessages' AND column_name = 'payload'
    ) THEN
        -- Merge legacy Payload into EventData when EventData is empty / default
        EXECUTE '
            UPDATE ""OutboxMessages""
            SET ""EventData"" = CASE
                WHEN ((""EventData""::text IS NULL OR ""EventData""::text = '''' OR ""EventData"" = ''{}''::jsonb)
                      AND Payload IS NOT NULL AND btrim(Payload) <> '''')
                THEN
                    CASE
                        WHEN Payload ~ ''^\s*[{[]'' THEN Payload::jsonb
                        ELSE jsonb_build_object(''legacyPayload'', Payload)
                    END
                ELSE ""EventData""
            END
        ';

        -- Drop legacy columns (guard each in case only one exists)
        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_name = 'OutboxMessages' AND column_name = 'type'
        ) THEN
            EXECUTE 'ALTER TABLE ""OutboxMessages"" DROP COLUMN ""Type""';
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_name = 'OutboxMessages' AND column_name = 'payload'
        ) THEN
            EXECUTE 'ALTER TABLE ""OutboxMessages"" DROP COLUMN ""Payload""';
        END IF;
    END IF;
END
$$;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate columns only if they don't already exist
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name='OutboxMessages' AND column_name='Type'
    ) THEN
        EXECUTE 'ALTER TABLE ""OutboxMessages"" ADD COLUMN ""Type"" varchar(255) NOT NULL DEFAULT ''''';
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_name='OutboxMessages' AND column_name='Payload'
    ) THEN
        EXECUTE 'ALTER TABLE ""OutboxMessages"" ADD COLUMN ""Payload"" text NOT NULL DEFAULT ''''';
    END IF;

    -- Repopulate from canonical fields (safe if columns just added)
    EXECUTE '
        UPDATE ""OutboxMessages""
        SET ""Type"" = COALESCE(""EventType"", ''''),
            ""Payload"" = COALESCE(CASE
                WHEN jsonb_typeof(""EventData"") IS NULL THEN ''''
                ELSE ""EventData""::text
            END, '''')
    ';
END
$$;
");
        }
    }
}
