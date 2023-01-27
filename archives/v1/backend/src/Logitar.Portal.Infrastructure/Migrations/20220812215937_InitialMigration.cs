using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Picture = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KeyHash = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsExpired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Sid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    EventType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventData = table.Column<string>(type: "jsonb", nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "JwtBlacklist",
                columns: table => new
                {
                    Sid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JwtBlacklist", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Sid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Errors = table.Column<string>(type: "jsonb", nullable: true),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Succeeded = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Request = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsDemo = table.Column<bool>(type: "boolean", nullable: false),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
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
                    TemplateContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TemplateDisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IgnoreUserLocale = table.Column<bool>(type: "boolean", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Variables = table.Column<string>(type: "jsonb", nullable: true),
                    Errors = table.Column<string>(type: "jsonb", nullable: true),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false),
                    Result = table.Column<string>(type: "jsonb", nullable: true),
                    Succeeded = table.Column<bool>(type: "boolean", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Alias = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AliasNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AllowedUsernameCharacters = table.Column<string>(type: "text", nullable: true),
                    RequireConfirmedAccount = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RequireUniqueEmail = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DefaultLocale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    PasswordSettings = table.Column<string>(type: "jsonb", nullable: true),
                    GoogleClientId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Sid);
                });

            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmSid = table.Column<int>(type: "integer", nullable: true),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Entries = table.Column<string>(type: "jsonb", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.Sid);
                    table.ForeignKey(
                        name: "FK_Dictionaries_Realms_RealmSid",
                        column: x => x.RealmSid,
                        principalTable: "Realms",
                        principalColumn: "Sid");
                });

            migrationBuilder.CreateTable(
                name: "Senders",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmSid = table.Column<int>(type: "integer", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Provider = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Settings = table.Column<string>(type: "jsonb", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.Sid);
                    table.ForeignKey(
                        name: "FK_Senders_Realms_RealmSid",
                        column: x => x.RealmSid,
                        principalTable: "Realms",
                        principalColumn: "Sid");
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmSid = table.Column<int>(type: "integer", nullable: true),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    KeyNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: "text/plain"),
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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmSid = table.Column<int>(type: "integer", nullable: true),
                    Username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UsernameNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    HasPassword = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailConfirmedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhoneNumberConfirmedById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAccountConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Picture = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SignedInAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisabledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisabledById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Sid);
                    table.ForeignKey(
                        name: "FK_Users_Realms_RealmSid",
                        column: x => x.RealmSid,
                        principalTable: "Realms",
                        principalColumn: "Sid");
                });

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

            migrationBuilder.CreateTable(
                name: "ExternalProviders",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    AddedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    RealmSid = table.Column<int>(type: "integer", nullable: false),
                    UserSid = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProviders", x => x.Sid);
                    table.ForeignKey(
                        name: "FK_ExternalProviders_Realms_RealmSid",
                        column: x => x.RealmSid,
                        principalTable: "Realms",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalProviders_Users_UserSid",
                        column: x => x.UserSid,
                        principalTable: "Users",
                        principalColumn: "Sid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Sid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserSid = table.Column<int>(type: "integer", nullable: false),
                    KeyHash = table.Column<string>(type: "text", nullable: true),
                    IsPersistent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SignedOutAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SignedOutById = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IpAddress = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "jsonb", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000")),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Sid);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserSid",
                        column: x => x.UserSid,
                        principalTable: "Users",
                        principalColumn: "Sid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actors_Id",
                table: "Actors",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Actors_Type",
                table: "Actors",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Id",
                table: "ApiKeys",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_IsExpired",
                table: "ApiKeys",
                column: "IsExpired");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Name",
                table: "ApiKeys",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_Id",
                table: "Dictionaries",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmSid_Locale",
                table: "Dictionaries",
                columns: new[] { "RealmSid", "Locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_Id",
                table: "Events",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_Id",
                table: "ExternalProviders",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_RealmSid_Key_Value",
                table: "ExternalProviders",
                columns: new[] { "RealmSid", "Key", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_UserSid",
                table: "ExternalProviders",
                column: "UserSid");

            migrationBuilder.CreateIndex(
                name: "IX_JwtBlacklist_ExpiresAt",
                table: "JwtBlacklist",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_JwtBlacklist_Id",
                table: "JwtBlacklist",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Id",
                table: "Logs",
                column: "Id",
                unique: true);

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
                name: "IX_Messages_IsDemo",
                table: "Messages",
                column: "IsDemo");

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
                name: "IX_Messages_Subject",
                table: "Messages",
                column: "Subject");

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
                name: "IX_RealmPasswordRecoverySenders_SenderSid",
                table: "RealmPasswordRecoverySenders",
                column: "SenderSid");

            migrationBuilder.CreateIndex(
                name: "IX_RealmPasswordRecoveryTemplates_TemplateSid",
                table: "RealmPasswordRecoveryTemplates",
                column: "TemplateSid");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_Alias",
                table: "Realms",
                column: "Alias");

            migrationBuilder.CreateIndex(
                name: "IX_Realms_AliasNormalized",
                table: "Realms",
                column: "AliasNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_Id",
                table: "Realms",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_Name",
                table: "Realms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_DisplayName",
                table: "Senders",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_EmailAddress",
                table: "Senders",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Id",
                table: "Senders",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_IsDefault",
                table: "Senders",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Provider",
                table: "Senders",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmSid",
                table: "Senders",
                column: "RealmSid");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Id",
                table: "Sessions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserSid",
                table: "Sessions",
                column: "UserSid");

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

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsAccountConfirmed",
                table: "Users",
                column: "IsAccountConfirmed");

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
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RealmSid_UsernameNormalized",
                table: "Users",
                columns: new[] { "RealmSid", "UsernameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actors");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "ExternalProviders");

            migrationBuilder.DropTable(
                name: "JwtBlacklist");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "RealmPasswordRecoverySenders");

            migrationBuilder.DropTable(
                name: "RealmPasswordRecoveryTemplates");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Senders");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Realms");
        }
    }
}
