using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordRecoveryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasswordRecoverySenderId",
                table: "Realms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PasswordRecoveryTemplateId",
                table: "Realms",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_PasswordRecoverySenderId",
                table: "Realms",
                column: "PasswordRecoverySenderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_PasswordRecoveryTemplateId",
                table: "Realms",
                column: "PasswordRecoveryTemplateId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Senders_PasswordRecoverySenderId",
                table: "Realms",
                column: "PasswordRecoverySenderId",
                principalTable: "Senders",
                principalColumn: "SenderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Templates_PasswordRecoveryTemplateId",
                table: "Realms",
                column: "PasswordRecoveryTemplateId",
                principalTable: "Templates",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Senders_PasswordRecoverySenderId",
                table: "Realms");

            migrationBuilder.DropForeignKey(
                name: "FK_Realms_Templates_PasswordRecoveryTemplateId",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_Realms_PasswordRecoverySenderId",
                table: "Realms");

            migrationBuilder.DropIndex(
                name: "IX_Realms_PasswordRecoveryTemplateId",
                table: "Realms");

            migrationBuilder.DropColumn(
                name: "PasswordRecoverySenderId",
                table: "Realms");

            migrationBuilder.DropColumn(
                name: "PasswordRecoveryTemplateId",
                table: "Realms");
        }
    }
}
