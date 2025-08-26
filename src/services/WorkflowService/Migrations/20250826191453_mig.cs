using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    /// <inheritdoc />
    public partial class mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentDefinitionId",
                table: "WorkflowDefinitions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublishNotes",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VersionNotes",
                table: "WorkflowDefinitions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventData",
                table: "OutboxMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "OutboxMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "OutboxMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "OutboxMessages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "OutboxMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentDefinitionId",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "PublishNotes",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "VersionNotes",
                table: "WorkflowDefinitions");

            migrationBuilder.DropColumn(
                name: "EventData",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "OutboxMessages");
        }
    }
}
