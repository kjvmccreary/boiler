using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WorkflowService.Migrations
{
    /// <inheritdoc />
    public partial class InitialWorkflowMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    Processed = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    NextRetryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    JSONDefinition = table.Column<string>(type: "jsonb", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PublishedByUserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                    table.CheckConstraint("CK_WorkflowDefinition_TenantId", "\"TenantId\" = current_setting('app.tenant_id')::int");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    WorkflowDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    DefinitionVersion = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrentNodeIds = table.Column<string>(type: "jsonb", nullable: false),
                    Context = table.Column<string>(type: "jsonb", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedByUserId = table.Column<int>(type: "integer", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowInstances", x => x.Id);
                    table.CheckConstraint("CK_WorkflowInstance_TenantId", "\"TenantId\" = current_setting('app.tenant_id')::int");
                    table.ForeignKey(
                        name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowInstanceId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Data = table.Column<string>(type: "jsonb", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowEvents", x => x.Id);
                    table.CheckConstraint("CK_WorkflowEvent_TenantId", "\"TenantId\" = current_setting('app.tenant_id')::int");
                    table.ForeignKey(
                        name: "FK_WorkflowEvents_WorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowInstanceId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    NodeId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TaskName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AssignedToUserId = table.Column<int>(type: "integer", nullable: true),
                    AssignedToRole = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Data = table.Column<string>(type: "jsonb", nullable: false),
                    ClaimedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletionData = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTasks", x => x.Id);
                    table.CheckConstraint("CK_WorkflowTask_TenantId", "\"TenantId\" = current_setting('app.tenant_id')::int");
                    table.ForeignKey(
                        name: "FK_WorkflowTasks_WorkflowInstances_WorkflowInstanceId",
                        column: x => x.WorkflowInstanceId,
                        principalTable: "WorkflowInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_NextRetryAt",
                table: "OutboxMessages",
                column: "NextRetryAt");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Processed_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "Processed", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Type_Processed",
                table: "OutboxMessages",
                columns: new[] { "Type", "Processed" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_TenantId_IsPublished",
                table: "WorkflowDefinitions",
                columns: new[] { "TenantId", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_TenantId_Name_Version",
                table: "WorkflowDefinitions",
                columns: new[] { "TenantId", "Name", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEvents_TenantId_Type_OccurredAt",
                table: "WorkflowEvents",
                columns: new[] { "TenantId", "Type", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEvents_TenantId_UserId",
                table: "WorkflowEvents",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowEvents_WorkflowInstanceId_OccurredAt",
                table: "WorkflowEvents",
                columns: new[] { "WorkflowInstanceId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_TenantId_StartedAt",
                table: "WorkflowInstances",
                columns: new[] { "TenantId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_TenantId_Status",
                table: "WorkflowInstances",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_TenantId_WorkflowDefinitionId",
                table: "WorkflowInstances",
                columns: new[] { "TenantId", "WorkflowDefinitionId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowInstances_WorkflowDefinitionId",
                table: "WorkflowInstances",
                column: "WorkflowDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_AssignedToRole_Status",
                table: "WorkflowTasks",
                columns: new[] { "AssignedToRole", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_TenantId_AssignedToUserId",
                table: "WorkflowTasks",
                columns: new[] { "TenantId", "AssignedToUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_TenantId_DueDate",
                table: "WorkflowTasks",
                columns: new[] { "TenantId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_TenantId_Status",
                table: "WorkflowTasks",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTasks_WorkflowInstanceId_Status",
                table: "WorkflowTasks",
                columns: new[] { "WorkflowInstanceId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "WorkflowEvents");

            migrationBuilder.DropTable(
                name: "WorkflowTasks");

            migrationBuilder.DropTable(
                name: "WorkflowInstances");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");
        }
    }
}
