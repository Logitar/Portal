using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class TemplateContentsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "Templates",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Contents",
                table: "Templates",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
