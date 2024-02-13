using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.SqlServer.Migrations
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
                    DictionaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Locale = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    EntryCount = table.Column<int>(type: "int", nullable: false),
                    Entries = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_Dictionaries_EntryCount",
                table: "Dictionaries",
                column: "EntryCount");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_Locale",
                table: "Dictionaries",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_TenantId_Locale",
                table: "Dictionaries",
                columns: new[] { "TenantId", "Locale" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

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
