using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDemo = table.Column<bool>(type: "boolean", nullable: false),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Recipients = table.Column<string>(type: "jsonb", nullable: false),
                    RealmId = table.Column<Guid>(type: "uuid", nullable: false),
                    RealmUniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RealmDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    SenderProvider = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SenderEmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SenderDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateUniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TemplateDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TemplateContentType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IgnoreUserLocale = table.Column<bool>(type: "boolean", nullable: false),
                    Locale = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Variables = table.Column<string>(type: "jsonb", nullable: true),
                    Errors = table.Column<string>(type: "jsonb", nullable: true),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false),
                    Result = table.Column<string>(type: "jsonb", nullable: true),
                    Succeeded = table.Column<bool>(type: "boolean", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AggregateId",
                table: "Messages",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedById",
                table: "Messages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedOn",
                table: "Messages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_HasErrors",
                table: "Messages",
                column: "HasErrors");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsDemo",
                table: "Messages",
                column: "IsDemo");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmDisplayName",
                table: "Messages",
                column: "RealmDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmId",
                table: "Messages",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmUniqueName",
                table: "Messages",
                column: "RealmUniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderDisplayName",
                table: "Messages",
                column: "SenderDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderEmailAddress",
                table: "Messages",
                column: "SenderEmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Subject",
                table: "Messages",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Succeeded",
                table: "Messages",
                column: "Succeeded");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateDisplayName",
                table: "Messages",
                column: "TemplateDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateId",
                table: "Messages",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateUniqueName",
                table: "Messages",
                column: "TemplateUniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedById",
                table: "Messages",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedOn",
                table: "Messages",
                column: "UpdatedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
