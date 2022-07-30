using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Infrastructure.Migrations
{
    public partial class AddedRealmAudience : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Realms",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Realms");
        }
    }
}
