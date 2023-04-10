using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    RealmId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DefaultLocale = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Secret = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    RequireConfirmedAccount = table.Column<bool>(type: "boolean", nullable: false),
                    RequireUniqueEmail = table.Column<bool>(type: "boolean", nullable: false),
                    UsernameSettings = table.Column<string>(type: "jsonb", nullable: false),
                    PasswordSettings = table.Column<string>(type: "jsonb", nullable: false),
                    ClaimMappings = table.Column<string>(type: "jsonb", nullable: true),
                    CustomAttributes = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Realms", x => x.RealmId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UsernameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordChangedById = table.Column<Guid>(type: "uuid", nullable: true),
                    PasswordChangedBy = table.Column<string>(type: "jsonb", nullable: true),
                    PasswordChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    HasPassword = table.Column<bool>(type: "boolean", nullable: false),
                    DisabledById = table.Column<Guid>(type: "uuid", nullable: true),
                    DisabledBy = table.Column<string>(type: "jsonb", nullable: true),
                    DisabledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    SignedInOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AddressLine1 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLine2 = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLocality = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressPostalCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressCountry = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressRegion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressFormatted = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    AddressVerifiedById = table.Column<Guid>(type: "uuid", nullable: true),
                    AddressVerifiedBy = table.Column<string>(type: "jsonb", nullable: true),
                    AddressVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAddressVerified = table.Column<bool>(type: "boolean", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailAddressNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailVerifiedById = table.Column<Guid>(type: "uuid", nullable: true),
                    EmailVerifiedBy = table.Column<string>(type: "jsonb", nullable: true),
                    EmailVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneExtension = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneE164Formatted = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneVerifiedById = table.Column<Guid>(type: "uuid", nullable: true),
                    PhoneVerifiedBy = table.Column<string>(type: "jsonb", nullable: true),
                    PhoneVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPhoneVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    Nickname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Locale = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Picture = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    Profile = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    Website = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    CustomAttributes = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Realms_RealmId",
                        column: x => x.RealmId,
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalIdentifiers",
                columns: table => new
                {
                    ExternalIdentifierId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RealmId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ValueNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalIdentifiers", x => x.ExternalIdentifierId);
                    table.ForeignKey(
                        name: "FK_ExternalIdentifiers_Realms_RealmId",
                        column: x => x.RealmId,
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalIdentifiers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsPersistent = table.Column<bool>(type: "boolean", nullable: false),
                    SignedOutById = table.Column<Guid>(type: "uuid", nullable: true),
                    SignedOutBy = table.Column<string>(type: "jsonb", nullable: true),
                    SignedOutOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    CustomAttributes = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Sessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_CreatedById",
                table: "ExternalIdentifiers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_Id",
                table: "ExternalIdentifiers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_RealmId_Key_ValueNormalized",
                table: "ExternalIdentifiers",
                columns: new[] { "RealmId", "Key", "ValueNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_UpdatedById",
                table: "ExternalIdentifiers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_UserId",
                table: "ExternalIdentifiers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_AggregateId",
                table: "Realms",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_CreatedById",
                table: "Realms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_CreatedOn",
                table: "Realms",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_DisplayName",
                table: "Realms",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UniqueName",
                table: "Realms",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UniqueNameNormalized",
                table: "Realms",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UpdatedById",
                table: "Realms",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UpdatedOn",
                table: "Realms",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AggregateId",
                table: "Sessions",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedById",
                table: "Sessions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedOn",
                table: "Sessions",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IsActive",
                table: "Sessions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_IsPersistent",
                table: "Sessions",
                column: "IsPersistent");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SignedOutById",
                table: "Sessions",
                column: "SignedOutById");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SignedOutOn",
                table: "Sessions",
                column: "SignedOutOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedById",
                table: "Sessions",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedOn",
                table: "Sessions",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressFormatted",
                table: "Users",
                column: "AddressFormatted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressVerifiedById",
                table: "Users",
                column: "AddressVerifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AggregateId",
                table: "Users",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedOn",
                table: "Users",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DisabledById",
                table: "Users",
                column: "DisabledById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DisabledOn",
                table: "Users",
                column: "DisabledOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                table: "Users",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailVerifiedById",
                table: "Users",
                column: "EmailVerifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsConfirmed",
                table: "Users",
                column: "IsConfirmed");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDisabled",
                table: "Users",
                column: "IsDisabled");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastName",
                table: "Users",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MiddleName",
                table: "Users",
                column: "MiddleName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Nickname",
                table: "Users",
                column: "Nickname");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordChangedById",
                table: "Users",
                column: "PasswordChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordChangedOn",
                table: "Users",
                column: "PasswordChangedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneE164Formatted",
                table: "Users",
                column: "PhoneE164Formatted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneVerifiedById",
                table: "Users",
                column: "PhoneVerifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmId_EmailAddressNormalized",
                table: "Users",
                columns: new[] { "RealmId", "EmailAddressNormalized" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmId_UsernameNormalized",
                table: "Users",
                columns: new[] { "RealmId", "UsernameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SignedInOn",
                table: "Users",
                column: "SignedInOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedById",
                table: "Users",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedOn",
                table: "Users",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalIdentifiers");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Realms");
        }
    }
}
