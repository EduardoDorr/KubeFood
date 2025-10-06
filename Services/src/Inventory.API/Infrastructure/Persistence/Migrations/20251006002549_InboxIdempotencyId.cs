using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KubeFood.Inventory.API.Migrations
{
    /// <inheritdoc />
    public partial class InboxIdempotencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdempotencyId",
                table: "OutboxMessages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyId",
                table: "InboxMessages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_IdempotencyId",
                table: "OutboxMessages",
                column: "IdempotencyId",
                unique: true,
                filter: "[IdempotencyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_IdempotencyId",
                table: "InboxMessages",
                column: "IdempotencyId",
                unique: true,
                filter: "[IdempotencyId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_IdempotencyId",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_InboxMessages_IdempotencyId",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "IdempotencyId",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "IdempotencyId",
                table: "InboxMessages");
        }
    }
}
