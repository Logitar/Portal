using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class LogitarPortal_4_1_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "Senders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Senders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Recipients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Recipients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderAddress",
                table: "Messages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "SenderPhoneNumber",
                table: "Messages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_PhoneNumber",
                table: "Senders",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_PhoneNumber",
                table: "Recipients",
                column: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Senders_PhoneNumber",
                table: "Senders");

            migrationBuilder.DropIndex(
                name: "IX_Recipients_PhoneNumber",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Senders");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Recipients");

            migrationBuilder.DropColumn(
                name: "SenderPhoneNumber",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "Senders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Recipients",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SenderAddress",
                table: "Messages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
