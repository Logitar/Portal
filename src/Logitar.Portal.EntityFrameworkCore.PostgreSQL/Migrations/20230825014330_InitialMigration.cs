using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    ActorId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PictureUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.ActorId);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    RealmId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueSlug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueSlugNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DefaultLocale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Secret = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    RequireUniqueEmail = table.Column<bool>(type: "boolean", nullable: false),
                    RequireConfirmedAccount = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedUniqueNameCharacters = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RequiredPasswordLength = table.Column<int>(type: "integer", nullable: false),
                    RequiredPasswordUniqueChars = table.Column<int>(type: "integer", nullable: false),
                    PasswordsRequireNonAlphanumeric = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireLowercase = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireUppercase = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireDigit = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordStrategy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ClaimMappings = table.Column<string>(type: "text", nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.RealmId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HasPassword = table.Column<bool>(type: "boolean", nullable: false),
                    Password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordChangedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisabledBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DisabledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    AuthenticatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AddressStreet = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLocality = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressRegion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressPostalCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressCountry = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressFormatted = table.Column<string>(type: "character varying(1536)", maxLength: 1536, nullable: true),
                    AddressVerifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAddressVerified = table.Column<bool>(type: "boolean", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailAddressNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailVerifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    PhoneExtension = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneE164Formatted = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneVerifiedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPhoneVerified = table.Column<bool>(type: "boolean", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "character varying(768)", maxLength: 768, nullable: true),
                    Nickname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    TimeZone = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Picture = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Profile = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Website = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IsPersistent = table.Column<bool>(type: "boolean", nullable: false),
                    Secret = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SignedOutBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SignedOutOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actors_DisplayName",
                table: "Actors",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_EmailAddress",
                table: "Actors",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_Id",
                table: "Actors",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actors_IsDeleted",
                table: "Actors",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Actors_Type",
                table: "Actors",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_AggregateId",
                table: "Realms",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_CreatedBy",
                table: "Realms",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_CreatedOn",
                table: "Realms",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_DisplayName",
                table: "Realms",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UniqueSlug",
                table: "Realms",
                column: "UniqueSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UniqueSlugNormalized",
                table: "Realms",
                column: "UniqueSlugNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UpdatedBy",
                table: "Realms",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_UpdatedOn",
                table: "Realms",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_Version",
                table: "Realms",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_AggregateId",
                table: "Roles",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedBy",
                table: "Roles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_CreatedOn",
                table: "Roles",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DisplayName",
                table: "Roles",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId",
                table: "Roles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId_UniqueNameNormalized",
                table: "Roles",
                columns: new[] { "TenantId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UniqueName",
                table: "Roles",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UpdatedBy",
                table: "Roles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UpdatedOn",
                table: "Roles",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Version",
                table: "Roles",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AggregateId",
                table: "Sessions",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_CreatedBy",
                table: "Sessions",
                column: "CreatedBy");

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
                name: "IX_Sessions_SignedOutOn",
                table: "Sessions",
                column: "SignedOutOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedBy",
                table: "Sessions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedOn",
                table: "Sessions",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Version",
                table: "Sessions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressFormatted",
                table: "Users",
                column: "AddressFormatted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AggregateId",
                table: "Users",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthenticatedOn",
                table: "Users",
                column: "AuthenticatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Birthdate",
                table: "Users",
                column: "Birthdate");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedOn",
                table: "Users",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DisabledOn",
                table: "Users",
                column: "DisabledOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                table: "Users",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Gender",
                table: "Users",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Users_HasPassword",
                table: "Users",
                column: "HasPassword");

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
                name: "IX_Users_Locale",
                table: "Users",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MiddleName",
                table: "Users",
                column: "MiddleName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Nickname",
                table: "Users",
                column: "Nickname");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PasswordChangedOn",
                table: "Users",
                column: "PasswordChangedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneE164Formatted",
                table: "Users",
                column: "PhoneE164Formatted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_EmailAddressNormalized",
                table: "Users",
                columns: new[] { "TenantId", "EmailAddressNormalized" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_UniqueNameNormalized",
                table: "Users",
                columns: new[] { "TenantId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TimeZone",
                table: "Users",
                column: "TimeZone");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UniqueName",
                table: "Users",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedBy",
                table: "Users",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedOn",
                table: "Users",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Version",
                table: "Users",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "Realms");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
