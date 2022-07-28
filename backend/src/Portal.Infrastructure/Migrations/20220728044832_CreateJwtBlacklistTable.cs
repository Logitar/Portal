using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Portal.Infrastructure.Migrations
{
    public partial class CreateJwtBlacklistTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JwtBlacklist",
                columns: table => new
                {
                    Sid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JwtBlacklist", x => x.Sid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JwtBlacklist_ExpiresAt",
                table: "JwtBlacklist",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_JwtBlacklist_Id",
                table: "JwtBlacklist",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JwtBlacklist");
        }
    }
}
