using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Logitar.Portal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SecretHash = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExpiresOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.ApiKeyId);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    ActorId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventData = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "JwtBlacklist",
                columns: table => new
                {
                    BlacklistedJwtId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JwtBlacklist", x => x.BlacklistedJwtId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    RecipientCount = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SenderIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    SenderAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SenderDisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SenderProvider = table.Column<int>(type: "integer", nullable: false),
                    TemplateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TemplateKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TemplateKeyNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TemplateDisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TemplateContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RealmId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RealmAlias = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RealmAliasNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RealmDisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IgnoreUserLocale = table.Column<bool>(type: "boolean", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Variables = table.Column<string>(type: "jsonb", nullable: true),
                    IsDemo = table.Column<bool>(type: "boolean", nullable: false),
                    Errors = table.Column<string>(type: "jsonb", nullable: true),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false),
                    Result = table.Column<string>(type: "jsonb", nullable: true),
                    HasSucceeded = table.Column<bool>(type: "boolean", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "ProviderTypes",
                columns: table => new
                {
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderTypes", x => x.Value);
                });

            migrationBuilder.CreateTable(
                name: "RecipientTypes",
                columns: table => new
                {
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipientTypes", x => x.Value);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    RecipientId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UserLocale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.RecipientId);
                    table.ForeignKey(
                        name: "FK_Recipients_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    DictionaryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Entries = table.Column<string>(type: "jsonb", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.DictionaryId);
                });

            migrationBuilder.CreateTable(
                name: "ExternalProviders",
                columns: table => new
                {
                    ExternalProviderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RealmId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AddedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AddedBy = table.Column<string>(type: "jsonb", nullable: false),
                    AddedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProviders", x => x.ExternalProviderId);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    RealmId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Alias = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AliasNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DefaultLocale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    RequireConfirmedAccount = table.Column<bool>(type: "boolean", nullable: false),
                    RequireUniqueEmail = table.Column<bool>(type: "boolean", nullable: false),
                    UsernameSettings = table.Column<string>(type: "jsonb", nullable: false),
                    PasswordSettings = table.Column<string>(type: "jsonb", nullable: false),
                    PasswordRecoverySenderId = table.Column<int>(type: "integer", nullable: true),
                    PasswordRecoveryTemplateId = table.Column<int>(type: "integer", nullable: true),
                    JwtSecret = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    GoogleClientId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.RealmId);
                });

            migrationBuilder.CreateTable(
                name: "Senders",
                columns: table => new
                {
                    SenderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    Settings = table.Column<string>(type: "jsonb", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.SenderId);
                    table.ForeignKey(
                        name: "FK_Senders_Realms_RealmId",
                        column: x => x.RealmId,
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    Key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    KeyNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Contents = table.Column<string>(type: "text", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UpdatedBy = table.Column<string>(type: "jsonb", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                    table.ForeignKey(
                        name: "FK_Templates_Realms_RealmId",
                        column: x => x.RealmId,
                        principalTable: "Realms",
                        principalColumn: "RealmId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RealmId = table.Column<int>(type: "integer", nullable: true),
                    Username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UsernameNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    PasswordChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HasPassword = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmedBy = table.Column<string>(type: "jsonb", nullable: true),
                    EmailConfirmedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberNormalized = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    PhoneNumberConfirmedBy = table.Column<string>(type: "jsonb", nullable: true),
                    PhoneNumberConfirmedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    IsAccountConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MiddleName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    FullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Picture = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    SignedInOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DisabledById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DisabledBy = table.Column<string>(type: "jsonb", nullable: true),
                    DisabledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    KeyHash = table.Column<string>(type: "text", nullable: true),
                    IsPersistent = table.Column<bool>(type: "boolean", nullable: false),
                    SignedOutById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SignedOutBy = table.Column<string>(type: "jsonb", nullable: true),
                    SignedOutOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedBy = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
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

            migrationBuilder.InsertData(
                table: "ProviderTypes",
                columns: new[] { "Value", "Name" },
                values: new object[] { 0, "SendGrid" });

            migrationBuilder.InsertData(
                table: "RecipientTypes",
                columns: new[] { "Value", "Name" },
                values: new object[,]
                {
                    { 0, "To" },
                    { 1, "CC" },
                    { 2, "Bcc" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_AggregateId",
                table: "ApiKeys",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedById",
                table: "ApiKeys",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_CreatedOn",
                table: "ApiKeys",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_ExpiresOn",
                table: "ApiKeys",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Title",
                table: "ApiKeys",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UpdatedById",
                table: "ApiKeys",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UpdatedOn",
                table: "ApiKeys",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_AggregateId",
                table: "Dictionaries",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedById",
                table: "Dictionaries",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_CreatedOn",
                table: "Dictionaries",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_RealmId_Locale",
                table: "Dictionaries",
                columns: new[] { "RealmId", "Locale" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedById",
                table: "Dictionaries",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Dictionaries_UpdatedOn",
                table: "Dictionaries",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AggregateType_AggregateId",
                table: "Events",
                columns: new[] { "AggregateType", "AggregateId" });

            migrationBuilder.CreateIndex(
                name: "IX_Events_Version",
                table: "Events",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_AddedById",
                table: "ExternalProviders",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_Id",
                table: "ExternalProviders",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_RealmId_Key_Value",
                table: "ExternalProviders",
                columns: new[] { "RealmId", "Key", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalProviders_UserId",
                table: "ExternalProviders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JwtBlacklist_ExpiresOn",
                table: "JwtBlacklist",
                column: "ExpiresOn");

            migrationBuilder.CreateIndex(
                name: "IX_JwtBlacklist_Id",
                table: "JwtBlacklist",
                column: "Id",
                unique: true);

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
                name: "IX_Messages_HasSucceeded",
                table: "Messages",
                column: "HasSucceeded");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsDemo",
                table: "Messages",
                column: "IsDemo");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmAlias",
                table: "Messages",
                column: "RealmAlias");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmAliasNormalized",
                table: "Messages",
                column: "RealmAliasNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmDisplayName",
                table: "Messages",
                column: "RealmDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RealmId",
                table: "Messages",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderAddress",
                table: "Messages",
                column: "SenderAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderDisplayName",
                table: "Messages",
                column: "SenderDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Subject",
                table: "Messages",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateDisplayName",
                table: "Messages",
                column: "TemplateDisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateKey",
                table: "Messages",
                column: "TemplateKey");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateKeyNormalized",
                table: "Messages",
                column: "TemplateKeyNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedById",
                table: "Messages",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedOn",
                table: "Messages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderTypes_Name",
                table: "ProviderTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_AggregateId",
                table: "Realms",
                column: "AggregateId",
                unique: true);

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
                name: "IX_Realms_PasswordRecoverySenderId",
                table: "Realms",
                column: "PasswordRecoverySenderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Realms_PasswordRecoveryTemplateId",
                table: "Realms",
                column: "PasswordRecoveryTemplateId",
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
                name: "IX_Recipients_Id",
                table: "Recipients",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_MessageId",
                table: "Recipients",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipientTypes_Name",
                table: "RecipientTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_AggregateId",
                table: "Senders",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_CreatedById",
                table: "Senders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_CreatedOn",
                table: "Senders",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_DisplayName",
                table: "Senders",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_EmailAddress",
                table: "Senders",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_IsDefault",
                table: "Senders",
                column: "IsDefault",
                filter: "\"IsDefault\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Provider",
                table: "Senders",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_RealmId",
                table: "Senders",
                column: "RealmId");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedById",
                table: "Senders",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedOn",
                table: "Senders",
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
                name: "IX_Sessions_IpAddress",
                table: "Sessions",
                column: "IpAddress");

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
                name: "IX_Templates_AggregateId",
                table: "Templates",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedById",
                table: "Templates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedOn",
                table: "Templates",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_DisplayName",
                table: "Templates",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Key",
                table: "Templates",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_RealmId_KeyNormalized",
                table: "Templates",
                columns: new[] { "RealmId", "KeyNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedById",
                table: "Templates",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedOn",
                table: "Templates",
                column: "UpdatedOn");

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
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailConfirmedById",
                table: "Users",
                column: "EmailConfirmedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailNormalized",
                table: "Users",
                column: "EmailNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FirstName",
                table: "Users",
                column: "FirstName");

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
                name: "IX_Users_PasswordChangedOn",
                table: "Users",
                column: "PasswordChangedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumberConfirmedById",
                table: "Users",
                column: "PhoneNumberConfirmedById");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Dictionaries_Realms_RealmId",
                table: "Dictionaries",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "RealmId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalProviders_Realms_RealmId",
                table: "ExternalProviders",
                column: "RealmId",
                principalTable: "Realms",
                principalColumn: "RealmId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalProviders_Users_UserId",
                table: "ExternalProviders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Senders_PasswordRecoverySenderId",
                table: "Realms",
                column: "PasswordRecoverySenderId",
                principalTable: "Senders",
                principalColumn: "SenderId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Realms_Templates_PasswordRecoveryTemplateId",
                table: "Realms",
                column: "PasswordRecoveryTemplateId",
                principalTable: "Templates",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Senders_Realms_RealmId",
                table: "Senders");

            migrationBuilder.DropForeignKey(
                name: "FK_Templates_Realms_RealmId",
                table: "Templates");

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
                name: "ProviderTypes");

            migrationBuilder.DropTable(
                name: "Recipients");

            migrationBuilder.DropTable(
                name: "RecipientTypes");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Realms");

            migrationBuilder.DropTable(
                name: "Senders");

            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
