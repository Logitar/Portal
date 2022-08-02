using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class AddTemplateContentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Templates",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "text/plain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Templates");
        }
    }
}
