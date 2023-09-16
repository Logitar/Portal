using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CompleteMessageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Realms_RealmId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "RealmSummary",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsDemo",
                table: "Messages",
                column: "IsDemo");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientCount",
                table: "Messages",
                column: "RecipientCount");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Status",
                table: "Messages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Subject",
                table: "Messages",
                column: "Subject");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Realms_RealmId",
                table: "Messages",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "RealmId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Realms_RealmId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_IsDemo",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_RecipientCount",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Status",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Subject",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "RealmSummary",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Realms_RealmId",
                table: "Messages",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "RealmId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
