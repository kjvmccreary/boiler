using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    public partial class DropOutboxTypePayloadFinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='OutboxMessages' AND column_name='Type') THEN
        -- Backfill EventType from legacy Type if EventType blank/null
        EXECUTE '
            UPDATE ""OutboxMessages""
            SET ""EventType"" = LOWER(""Type"")
            WHERE (""EventType"" IS NULL OR ""EventType"" = '''')
              AND ""Type"" IS NOT NULL AND ""Type"" <> ''''
        ';
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='OutboxMessages' AND column_name='Payload') THEN
        -- Merge Payload into EventData when EventData empty / default
        -- Cast Payload to text in all checks to be agnostic of jsonb vs text
        EXECUTE '
            UPDATE ""OutboxMessages""
            SET ""EventData"" = CASE
                WHEN (
                        (""EventData""::text IS NULL OR ""EventData""::text = '''' OR ""EventData"" = ''{}''::jsonb)
                        AND ""Payload"" IS NOT NULL
                        AND COALESCE(""Payload""::text, '''') <> ''''
                     )
                THEN
                    CASE
                        WHEN ""Payload""::text ~ ''^\s*[{[]'' THEN (""Payload""::text)::jsonb
                        ELSE jsonb_build_object(''legacyPayload'', ""Payload""::text)
                    END
                ELSE ""EventData""
            END
        ';
    END IF;

    -- Drop legacy columns if they are still present
    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='OutboxMessages' AND column_name='Type') THEN
        EXECUTE 'ALTER TABLE ""OutboxMessages"" DROP COLUMN ""Type""';
    END IF;

    IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='OutboxMessages' AND column_name='Payload') THEN
        EXECUTE 'ALTER TABLE ""OutboxMessages"" DROP COLUMN ""Payload""';
    END IF;
END
$$;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='OutboxMessages' AND column_name='Type') THEN
        EXECUTE 'ALTER TABLE ""OutboxMessages"" ADD COLUMN ""Type"" varchar(255) NOT NULL DEFAULT ''''';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name='OutboxMessages' AND column_name='Payload') THEN
        EXECUTE 'ALTER TABLE ""OutboxMessages"" ADD COLUMN ""Payload"" text NOT NULL DEFAULT ''''';
    END IF;

    -- Repopulate from canonical fields (best-effort)
    EXECUTE '
        UPDATE ""OutboxMessages""
        SET ""Type"" = COALESCE(NULLIF(""Type"", ''''), COALESCE(""EventType"", '''')),
            ""Payload"" = COALESCE(NULLIF(""Payload"", ''''), ""EventData""::text)
    ';
END
$$;
");
        }
    }
}
