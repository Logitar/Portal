using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Infrastructure.Migrations
{
    public partial class RenameMessageSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TemplateSubject",
                table: "Messages",
                newName: "Subject");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_TemplateSubject",
                table: "Messages",
                newName: "IX_Messages_Subject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Messages",
                newName: "TemplateSubject");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_Subject",
                table: "Messages",
                newName: "IX_Messages_TemplateSubject");
        }
    }
}
