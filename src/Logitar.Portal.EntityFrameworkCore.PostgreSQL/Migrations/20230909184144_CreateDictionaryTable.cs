using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateDictionaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    DictionaryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    EntriesSerialized = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.DictionaryId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_AggregateId",
                table: "Dictionaries",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedBy",
                table: "Dictionaries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedOn",
                table: "Dictionaries",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_TenantId",
                table: "Dictionaries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_TenantId_Locale",
                table: "Dictionaries",
                columns: new[] { "TenantId", "Locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedBy",
                table: "Dictionaries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedOn",
                table: "Dictionaries",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_Version",
                table: "Dictionaries",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dictionaries");
        }
    }
}
