using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class LogitarPortal_4_0_0 : Migration
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
                    LocaleNormalized = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    EntryCount = table.Column<int>(type: "integer", nullable: false),
                    Entries = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.DictionaryId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Method = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Destination = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Source = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    OperationType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OperationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActivityData = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActorId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ApiKeyId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
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
                    AllowedUniqueNameCharacters = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RequiredPasswordLength = table.Column<int>(type: "integer", nullable: false),
                    RequiredPasswordUniqueChars = table.Column<int>(type: "integer", nullable: false),
                    PasswordsRequireNonAlphanumeric = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireLowercase = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireUppercase = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordsRequireDigit = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHashingStrategy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RequireUniqueEmail = table.Column<bool>(type: "boolean", nullable: false),
                    CustomAttributes = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Settings = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senders", x => x.SenderId);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UniqueKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueKeyNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ContentText = table.Column<string>(type: "text", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "LogEvents",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEvents", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_LogEvents_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogExceptions",
                columns: table => new
                {
                    LogExceptionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    HResult = table.Column<int>(type: "integer", nullable: false),
                    HelpLink = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    TargetSite = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogExceptions", x => x.LogExceptionId);
                    table.ForeignKey(
                        name: "FK_LogExceptions_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BodyType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    BodyText = table.Column<string>(type: "text", nullable: false),
                    RecipientCount = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: true),
                    SenderIsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    SenderAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SenderDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SenderProvider = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TemplateId = table.Column<int>(type: "integer", nullable: true),
                    TemplateUniqueKey = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TemplateDisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IgnoreUserLocale = table.Column<bool>(type: "boolean", nullable: false),
                    Locale = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Variables = table.Column<string>(type: "text", nullable: true),
                    IsDemo = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ResultData = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_Senders_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Senders",
                        principalColumn: "SenderId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Messages_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    RecipientId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    UserUniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserEmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserFullName = table.Column<string>(type: "character varying(767)", maxLength: 767, nullable: true),
                    UserPicture = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
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
                name: "IX_Dictionaries_TenantId_LocaleNormalized",
                table: "Dictionaries",
                columns: new[] { "TenantId", "LocaleNormalized" },
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

            migrationBuilder.CreateIndex(
                name: "IX_LogEvents_LogId",
                table: "LogEvents",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_LogId",
                table: "LogExceptions",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_Type",
                table: "LogExceptions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActivityType",
                table: "Logs",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActorId",
                table: "Logs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ApiKeyId",
                table: "Logs",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CorrelationId",
                table: "Logs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Duration",
                table: "Logs",
                column: "Duration");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_EndedOn",
                table: "Logs",
                column: "EndedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_HasErrors",
                table: "Logs",
                column: "HasErrors");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IsCompleted",
                table: "Logs",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Level",
                table: "Logs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationName",
                table: "Logs",
                column: "OperationName");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationType",
                table: "Logs",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SessionId",
                table: "Logs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StartedOn",
                table: "Logs",
                column: "StartedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StatusCode",
                table: "Logs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_TenantId",
                table: "Logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UniqueId",
                table: "Logs",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AggregateId",
                table: "Messages",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedBy",
                table: "Messages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_CreatedOn",
                table: "Messages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IsDemo",
                table: "Messages",
                column: "IsDemo");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientCount",
                table: "Messages",
                column: "RecipientCount");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Status",
                table: "Messages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Subject",
                table: "Messages",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TemplateId",
                table: "Messages",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TenantId",
                table: "Messages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedBy",
                table: "Messages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UpdatedOn",
                table: "Messages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Version",
                table: "Messages",
                column: "Version");

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
                name: "IX_Recipients_Address",
                table: "Recipients",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_DisplayName",
                table: "Recipients",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_MessageId",
                table: "Recipients",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_Type",
                table: "Recipients",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_UserId",
                table: "Recipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_AggregateId",
                table: "Senders",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Senders_CreatedBy",
                table: "Senders",
                column: "CreatedBy");

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
                name: "IX_Senders_Provider",
                table: "Senders",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_TenantId_IsDefault",
                table: "Senders",
                columns: new[] { "TenantId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedBy",
                table: "Senders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_UpdatedOn",
                table: "Senders",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Senders_Version",
                table: "Senders",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_AggregateId",
                table: "Templates",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_ContentType",
                table: "Templates",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedBy",
                table: "Templates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedOn",
                table: "Templates",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_DisplayName",
                table: "Templates",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Subject",
                table: "Templates",
                column: "Subject");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_TenantId_UniqueKeyNormalized",
                table: "Templates",
                columns: new[] { "TenantId", "UniqueKeyNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UniqueKey",
                table: "Templates",
                column: "UniqueKey");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedBy",
                table: "Templates",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UpdatedOn",
                table: "Templates",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Version",
                table: "Templates",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropTable(
                name: "LogEvents");

            migrationBuilder.DropTable(
                name: "LogExceptions");

            migrationBuilder.DropTable(
                name: "Realms");

            migrationBuilder.DropTable(
                name: "Recipients");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Senders");

            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
