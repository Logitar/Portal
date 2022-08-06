using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class RefactoredExternalProviders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExternalProviders_Key_Value",
                table: "ExternalProviders");

            migrationBuilder.AlterColumn<int>(
                name: "UserSid",
                table: "ExternalProviders",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RealmSid",
                table: "ExternalProviders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_RealmSid_Key_Value",
                table: "ExternalProviders",
                columns: new[] { "RealmSid", "Key", "Value" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalProviders_Realms_RealmSid",
                table: "ExternalProviders",
                column: "RealmSid",
                principalTable: "Realms",
                principalColumn: "Sid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalProviders_Realms_RealmSid",
                table: "ExternalProviders");

            migrationBuilder.DropIndex(
                name: "IX_ExternalProviders_RealmSid_Key_Value",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "RealmSid",
                table: "ExternalProviders");

            migrationBuilder.AlterColumn<int>(
                name: "UserSid",
                table: "ExternalProviders",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_Key_Value",
                table: "ExternalProviders",
                columns: new[] { "Key", "Value" });
        }
    }
}
