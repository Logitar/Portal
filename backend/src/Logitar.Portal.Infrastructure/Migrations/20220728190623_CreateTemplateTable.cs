using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class CreateTemplateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_EmailNormalized",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmSid = table.Column<int>(type: "integer", nullable: true),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    KeyNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Contents = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Sid);
                    table.ForeignKey(
                        name: "FK_Templates_Realms_RealmSid",
                        column: x => x.RealmSid,
                        principalTable: "Realms",
                        principalColumn: "Sid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Templates_DisplayName",
                table: "Templates",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Id",
                table: "Templates",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Key",
                table: "Templates",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmSid_KeyNormalized",
                table: "Templates",
                columns: new[] { "RealmSid", "KeyNormalized" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailNormalized",
                table: "Users",
                column: "EmailNormalized");
        }
    }
}
