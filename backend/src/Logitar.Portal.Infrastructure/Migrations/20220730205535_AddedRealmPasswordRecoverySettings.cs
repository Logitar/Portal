using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class AddedRealmPasswordRecoverySettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RealmPasswordRecoverySenders",
                columns: table => new
                {
                    RealmSid = table.Column<int>(type: "integer", nullable: false),
                    SenderSid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealmPasswordRecoverySenders", x => x.RealmSid);
                    table.ForeignKey(
                        name: "FK_RealmPasswordRecoverySenders_Realms_RealmSid",
                        column: x => x.RealmSid,
                        principalTable: "Realms",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealmPasswordRecoverySenders_Senders_SenderSid",
                        column: x => x.SenderSid,
                        principalTable: "Senders",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealmPasswordRecoveryTemplates",
                columns: table => new
                {
                    RealmSid = table.Column<int>(type: "integer", nullable: false),
                    TemplateSid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealmPasswordRecoveryTemplates", x => x.RealmSid);
                    table.ForeignKey(
                        name: "FK_RealmPasswordRecoveryTemplates_Realms_RealmSid",
                        column: x => x.RealmSid,
                        principalTable: "Realms",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealmPasswordRecoveryTemplates_Templates_TemplateSid",
                        column: x => x.TemplateSid,
                        principalTable: "Templates",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RealmPasswordRecoverySenders_SenderSid",
                table: "RealmPasswordRecoverySenders",
                column: "SenderSid");

            migrationBuilder.CreateIndex(
                name: "IX_RealmPasswordRecoveryTemplates_TemplateSid",
                table: "RealmPasswordRecoveryTemplates",
                column: "TemplateSid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RealmPasswordRecoverySenders");

            migrationBuilder.DropTable(
                name: "RealmPasswordRecoveryTemplates");
        }
    }
}
