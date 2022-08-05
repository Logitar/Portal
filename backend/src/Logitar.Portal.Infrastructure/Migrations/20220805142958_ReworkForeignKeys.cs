using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class ReworkForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalProvider_Users_UserSid",
                table: "ExternalProvider");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_UserSid",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalProvider",
                table: "ExternalProvider");

            migrationBuilder.RenameTable(
                name: "ExternalProvider",
                newName: "ExternalProviders");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalProvider_UserSid",
                table: "ExternalProviders",
                newName: "IX_ExternalProviders_UserSid");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalProvider_Key_Value",
                table: "ExternalProviders",
                newName: "IX_ExternalProviders_Key_Value");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalProvider_Id",
                table: "ExternalProviders",
                newName: "IX_ExternalProviders_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalProviders",
                table: "ExternalProviders",
                column: "Sid");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalProviders_Users_UserSid",
                table: "ExternalProviders",
                column: "UserSid",
                principalTable: "Users",
                principalColumn: "Sid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_UserSid",
                table: "Sessions",
                column: "UserSid",
                principalTable: "Users",
                principalColumn: "Sid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalProviders_Users_UserSid",
                table: "ExternalProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_UserSid",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalProviders",
                table: "ExternalProviders");

            migrationBuilder.RenameTable(
                name: "ExternalProviders",
                newName: "ExternalProvider");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalProviders_UserSid",
                table: "ExternalProvider",
                newName: "IX_ExternalProvider_UserSid");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalProviders_Key_Value",
                table: "ExternalProvider",
                newName: "IX_ExternalProvider_Key_Value");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalProviders_Id",
                table: "ExternalProvider",
                newName: "IX_ExternalProvider_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalProvider",
                table: "ExternalProvider",
                column: "Sid");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalProvider_Users_UserSid",
                table: "ExternalProvider",
                column: "UserSid",
                principalTable: "Users",
                principalColumn: "Sid");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_UserSid",
                table: "Sessions",
                column: "UserSid",
                principalTable: "Users",
                principalColumn: "Sid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
