using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class AddedMessageIsDemo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDemo",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsDemo",
                table: "Messages",
                column: "IsDemo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_IsDemo",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsDemo",
                table: "Messages");
        }
    }
}
