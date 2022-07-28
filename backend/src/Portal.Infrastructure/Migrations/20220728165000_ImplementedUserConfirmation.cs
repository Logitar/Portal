using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Infrastructure.Migrations
{
    public partial class ImplementedUserConfirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmed",
                table: "Users",
                newName: "PhoneNumberConfirmedAt");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                table: "Users",
                newName: "EmailConfirmedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailConfirmedById",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhoneNumberConfirmedById",
                table: "Users",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmedById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmedById",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmedAt",
                table: "Users",
                newName: "PhoneNumberConfirmed");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmedAt",
                table: "Users",
                newName: "EmailConfirmed");
        }
    }
}
