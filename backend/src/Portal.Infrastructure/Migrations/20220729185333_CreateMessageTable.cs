using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Portal.Infrastructure.Migrations
{
    public partial class CreateMessageTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Recipients = table.Column<string>(type: "jsonb", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    SenderProvider = table.Column<int>(type: "integer", nullable: false),
                    SenderAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SenderDisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RealmId = table.Column<Guid>(type: "uuid", nullable: true),
                    RealmAlias = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RealmName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TemplateSubject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TemplateContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TemplateDisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Errors = table.Column<string>(type: "jsonb", nullable: true),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false),
                    Result = table.Column<string>(type: "jsonb", nullable: true),
                    Succeeded = table.Column<bool>(type: "boolean", nullable: false),
                    Variables = table.Column<string>(type: "jsonb", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Sid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_HasErrors",
                table: "Messages",
                column: "HasErrors");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Id",
                table: "Messages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmAlias",
                table: "Messages",
                column: "RealmAlias");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmId",
                table: "Messages",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmName",
                table: "Messages",
                column: "RealmName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderAddress",
                table: "Messages",
                column: "SenderAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderDisplayName",
                table: "Messages",
                column: "SenderDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderProvider",
                table: "Messages",
                column: "SenderProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Succeeded",
                table: "Messages",
                column: "Succeeded");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateContentType",
                table: "Messages",
                column: "TemplateContentType");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateDisplayName",
                table: "Messages",
                column: "TemplateDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateId",
                table: "Messages",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateKey",
                table: "Messages",
                column: "TemplateKey");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateSubject",
                table: "Messages",
                column: "TemplateSubject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
