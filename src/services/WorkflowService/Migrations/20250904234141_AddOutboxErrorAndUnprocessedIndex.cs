using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxErrorAndUnprocessedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "LastError",
                table: "OutboxMessages",
                newName: "Error");

            migrationBuilder.CreateIndex(
                name: "IDX_Outbox_Unprocessed",
                table: "OutboxMessages",
                columns: new[] { "TenantId", "CreatedAt" },
                filter: "\"ProcessedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IDX_Outbox_Unprocessed",
                table: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "Error",
                table: "OutboxMessages",
                newName: "LastError");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "OutboxMessages",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
