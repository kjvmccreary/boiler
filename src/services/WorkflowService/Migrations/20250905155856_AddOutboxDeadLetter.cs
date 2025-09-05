using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkflowService.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxDeadLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeadLetter",
                table: "OutboxMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_DeadLetter_CreatedAt",
                table: "OutboxMessages",
                columns: new[] { "DeadLetter", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_DeadLetter_CreatedAt",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "DeadLetter",
                table: "OutboxMessages");
        }
    }
}
