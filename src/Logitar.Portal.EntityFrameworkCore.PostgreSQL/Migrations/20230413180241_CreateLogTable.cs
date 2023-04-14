using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Logging");

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "Logging",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Method = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Destination = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    Source = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    OperationType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OperationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    StartedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Errors = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Id",
                schema: "Logging",
                table: "Logs",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs",
                schema: "Logging");
        }
    }
}
