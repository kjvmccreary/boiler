using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTenantIsolationFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AuditEntries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ResourceId",
                table: "AuditEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AuditEntries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_AuditEntries_Tenants_TenantId",
                table: "AuditEntries",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditEntries_Users_UserId",
                table: "AuditEntries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditEntries_Tenants_TenantId",
                table: "AuditEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditEntries_Users_UserId",
                table: "AuditEntries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AuditEntries");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "AuditEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AuditEntries");
        }
    }
}
