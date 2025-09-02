using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    public partial class AddOutboxIdempotencyKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // (Existing index/foreign key drops & alters you already had – KEEP THEM AS IS ABOVE THIS POINT)

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowInstances");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowTasks_AssignedToRole_Status",
                table: "WorkflowTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowTasks_TenantId_AssignedToUserId",
                table: "WorkflowTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowTasks_WorkflowInstanceId_Status",
                table: "WorkflowTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowInstances_TenantId_StartedAt",
                table: "WorkflowInstances");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowEvents_TenantId_UserId",
                table: "WorkflowEvents");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowEvents_WorkflowInstanceId_OccurredAt",
                table: "WorkflowEvents");

            migrationBuilder.AlterColumn<string>(
                name: "CompletionData",
                table: "WorkflowTasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VersionNotes",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublishNotes",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            // === FIX: Add column WITHOUT default & nullable so we can backfill unique values ===
            migrationBuilder.AddColumn<Guid>(
                name: "IdempotencyKey",
                table: "OutboxMessages",
                type: "uuid",
                nullable: true);

            // Ensure pgcrypto so gen_random_uuid() works (idempotent)
            migrationBuilder.Sql(@"CREATE EXTENSION IF NOT EXISTS ""pgcrypto"";");

            // Backfill any existing rows
            migrationBuilder.Sql(@"
UPDATE ""OutboxMessages""
SET ""IdempotencyKey"" = gen_random_uuid()
WHERE ""IdempotencyKey"" IS NULL;");

            // Enforce NOT NULL after backfill
            migrationBuilder.Sql(@"ALTER TABLE ""OutboxMessages"" ALTER COLUMN ""IdempotencyKey"" SET NOT NULL;");

            // (Optional) DB-side default for future safety; harmless if code also sets it
            migrationBuilder.Sql(@"ALTER TABLE ""OutboxMessages"" ALTER COLUMN ""IdempotencyKey"" SET DEFAULT gen_random_uuid();");

            // New supporting indexes (simplified originals you kept)
            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_WorkflowInstanceId",
                table: "WorkflowTasks",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEvents_WorkflowInstanceId",
                table: "WorkflowEvents",
                column: "WorkflowInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_TenantId_IdempotencyKey",
                table: "OutboxMessages",
                columns: new[] { "TenantId", "IdempotencyKey" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowInstances",
                column: "WorkflowDefinitionId",
                principalTable: "WorkflowDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowInstances");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowTasks_WorkflowInstanceId",
                table: "WorkflowTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowEvents_WorkflowInstanceId",
                table: "WorkflowEvents");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_TenantId_IdempotencyKey",
                table: "OutboxMessages");

            // Remove DB default before dropping
            migrationBuilder.Sql(@"ALTER TABLE ""OutboxMessages"" ALTER COLUMN ""IdempotencyKey"" DROP DEFAULT;");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "OutboxMessages");

            migrationBuilder.AlterColumn<string>(
                name: "CompletionData",
                table: "WorkflowTasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VersionNotes",
                table: "WorkflowDefinitions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "WorkflowDefinitions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublishNotes",
                table: "WorkflowDefinitions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Recreate the dropped indexes from original state if needed (omitted here for brevity)
            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                table: "WorkflowInstances",
                column: "WorkflowDefinitionId",
                principalTable: "WorkflowDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
