using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class AddedRealmUserOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "RequireConfirmedAccount",
                table: "Realms",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "AllowedUsernameCharacters",
                table: "Realms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequireUniqueEmail",
                table: "Realms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedUsernameCharacters",
                table: "Realms");

            migrationBuilder.DropColumn(
                name: "RequireUniqueEmail",
                table: "Realms");

            migrationBuilder.AlterColumn<bool>(
                name: "RequireConfirmedAccount",
                table: "Realms",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);
        }
    }
}
