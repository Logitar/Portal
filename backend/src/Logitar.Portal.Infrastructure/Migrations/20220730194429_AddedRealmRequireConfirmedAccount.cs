using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class AddedRealmRequireConfirmedAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequireConfirmedAccount",
                table: "Realms",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequireConfirmedAccount",
                table: "Realms");
        }
    }
}
