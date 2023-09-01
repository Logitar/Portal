using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserIdentifierTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserIdentifiers",
                columns: table => new
                {
                    UserIdentifierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ValueNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentifiers", x => x.UserIdentifierId);
                    table.ForeignKey(
                        name: "FK_UserIdentifiers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_Id",
                table: "UserIdentifiers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_TenantId_Key_Value",
                table: "UserIdentifiers",
                columns: new[] { "TenantId", "Key", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentifiers_UserId",
                table: "UserIdentifiers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserIdentifiers");
        }
    }
}
