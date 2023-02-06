using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LogActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestType",
                table: "Logs",
                newName: "ActivityType");

            migrationBuilder.RenameColumn(
                name: "RequestData",
                table: "Logs",
                newName: "ActivityData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActivityType",
                table: "Logs",
                newName: "RequestType");

            migrationBuilder.RenameColumn(
                name: "ActivityData",
                table: "Logs",
                newName: "RequestData");
        }
    }
}
