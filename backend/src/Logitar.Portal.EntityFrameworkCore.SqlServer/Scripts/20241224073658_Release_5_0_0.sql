IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Dictionaries] (
    [DictionaryId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [EntityId] nvarchar(255) NOT NULL,
    [Locale] nvarchar(16) NOT NULL,
    [LocaleNormalized] nvarchar(16) NOT NULL,
    [EntryCount] int NOT NULL,
    [Entries] nvarchar(max) NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Dictionaries] PRIMARY KEY ([DictionaryId])
);

CREATE TABLE [Logs] (
    [LogId] bigint NOT NULL IDENTITY,
    [UniqueId] uniqueidentifier NOT NULL,
    [CorrelationId] nvarchar(255) NULL,
    [Method] nvarchar(255) NULL,
    [Destination] nvarchar(2048) NULL,
    [Source] nvarchar(2048) NULL,
    [AdditionalInformation] nvarchar(max) NULL,
    [OperationType] nvarchar(255) NULL,
    [OperationName] nvarchar(255) NULL,
    [ActivityType] nvarchar(255) NULL,
    [ActivityData] nvarchar(max) NULL,
    [StatusCode] int NULL,
    [IsCompleted] bit NOT NULL,
    [Level] nvarchar(255) NOT NULL,
    [HasErrors] bit NOT NULL,
    [StartedOn] datetime2 NOT NULL,
    [EndedOn] datetime2 NULL,
    [Duration] time NULL,
    [TenantId] uniqueidentifier NULL,
    [ActorId] nvarchar(255) NULL,
    [ApiKeyId] uniqueidentifier NULL,
    [UserId] uniqueidentifier NULL,
    [SessionId] uniqueidentifier NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY ([LogId])
);

CREATE TABLE [Realms] (
    [RealmId] int NOT NULL IDENTITY,
    [UniqueSlug] nvarchar(255) NOT NULL,
    [UniqueSlugNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [DefaultLocale] nvarchar(16) NULL,
    [Secret] nvarchar(64) NOT NULL,
    [Url] nvarchar(2048) NULL,
    [AllowedUniqueNameCharacters] nvarchar(255) NULL,
    [RequiredPasswordLength] int NOT NULL,
    [RequiredPasswordUniqueChars] int NOT NULL,
    [PasswordsRequireNonAlphanumeric] bit NOT NULL,
    [PasswordsRequireLowercase] bit NOT NULL,
    [PasswordsRequireUppercase] bit NOT NULL,
    [PasswordsRequireDigit] bit NOT NULL,
    [PasswordHashingStrategy] nvarchar(255) NOT NULL,
    [RequireUniqueEmail] bit NOT NULL,
    [CustomAttributes] nvarchar(max) NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Realms] PRIMARY KEY ([RealmId])
);

CREATE TABLE [Senders] (
    [SenderId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [EntityId] nvarchar(255) NOT NULL,
    [IsDefault] bit NOT NULL,
    [EmailAddress] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [Provider] nvarchar(255) NOT NULL,
    [Settings] nvarchar(max) NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Senders] PRIMARY KEY ([SenderId])
);

CREATE TABLE [Templates] (
    [TemplateId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [EntityId] nvarchar(255) NOT NULL,
    [UniqueKey] nvarchar(255) NOT NULL,
    [UniqueKeyNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [Subject] nvarchar(255) NOT NULL,
    [ContentType] nvarchar(10) NOT NULL,
    [ContentText] nvarchar(max) NOT NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Templates] PRIMARY KEY ([TemplateId])
);

CREATE TABLE [LogEvents] (
    [EventId] uniqueidentifier NOT NULL,
    [LogId] bigint NOT NULL,
    CONSTRAINT [PK_LogEvents] PRIMARY KEY ([EventId]),
    CONSTRAINT [FK_LogEvents_Logs_LogId] FOREIGN KEY ([LogId]) REFERENCES [Logs] ([LogId]) ON DELETE CASCADE
);

CREATE TABLE [LogExceptions] (
    [LogExceptionId] bigint NOT NULL IDENTITY,
    [LogId] bigint NOT NULL,
    [Type] nvarchar(255) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [HResult] int NOT NULL,
    [HelpLink] nvarchar(max) NULL,
    [Source] nvarchar(max) NULL,
    [StackTrace] nvarchar(max) NULL,
    [TargetSite] nvarchar(max) NULL,
    [Data] nvarchar(max) NULL,
    CONSTRAINT [PK_LogExceptions] PRIMARY KEY ([LogExceptionId]),
    CONSTRAINT [FK_LogExceptions_Logs_LogId] FOREIGN KEY ([LogId]) REFERENCES [Logs] ([LogId]) ON DELETE CASCADE
);

CREATE TABLE [Messages] (
    [MessageId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [EntityId] nvarchar(255) NOT NULL,
    [Subject] nvarchar(255) NOT NULL,
    [BodyType] nvarchar(10) NOT NULL,
    [BodyText] nvarchar(max) NOT NULL,
    [RecipientCount] int NOT NULL,
    [SenderId] int NULL,
    [SenderIsDefault] bit NOT NULL,
    [SenderAddress] nvarchar(255) NULL,
    [SenderPhoneNumber] nvarchar(20) NULL,
    [SenderDisplayName] nvarchar(255) NULL,
    [SenderProvider] nvarchar(255) NOT NULL,
    [TemplateId] int NULL,
    [TemplateUniqueKey] nvarchar(255) NOT NULL,
    [TemplateDisplayName] nvarchar(255) NULL,
    [IgnoreUserLocale] bit NOT NULL,
    [Locale] nvarchar(16) NULL,
    [Variables] nvarchar(max) NULL,
    [IsDemo] bit NOT NULL,
    [Status] nvarchar(255) NOT NULL,
    [ResultData] nvarchar(max) NULL,
    [StreamId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([MessageId]),
    CONSTRAINT [FK_Messages_Senders_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Senders] ([SenderId]) ON DELETE SET NULL,
    CONSTRAINT [FK_Messages_Templates_TemplateId] FOREIGN KEY ([TemplateId]) REFERENCES [Templates] ([TemplateId]) ON DELETE SET NULL
);

CREATE TABLE [Recipients] (
    [RecipientId] int NOT NULL IDENTITY,
    [MessageId] int NOT NULL,
    [Type] nvarchar(3) NOT NULL,
    [Address] nvarchar(255) NULL,
    [DisplayName] nvarchar(255) NULL,
    [PhoneNumber] nvarchar(20) NULL,
    [UserId] int NULL,
    [UserUniqueName] nvarchar(255) NULL,
    [UserEmailAddress] nvarchar(255) NULL,
    [UserFullName] nvarchar(767) NULL,
    [UserPicture] nvarchar(2048) NULL,
    CONSTRAINT [PK_Recipients] PRIMARY KEY ([RecipientId]),
    CONSTRAINT [FK_Recipients_Messages_MessageId] FOREIGN KEY ([MessageId]) REFERENCES [Messages] ([MessageId]) ON DELETE CASCADE
);

CREATE INDEX [IX_Dictionaries_CreatedBy] ON [Dictionaries] ([CreatedBy]);

CREATE INDEX [IX_Dictionaries_CreatedOn] ON [Dictionaries] ([CreatedOn]);

CREATE INDEX [IX_Dictionaries_EntityId] ON [Dictionaries] ([EntityId]);

CREATE INDEX [IX_Dictionaries_EntryCount] ON [Dictionaries] ([EntryCount]);

CREATE INDEX [IX_Dictionaries_Locale] ON [Dictionaries] ([Locale]);

CREATE UNIQUE INDEX [IX_Dictionaries_StreamId] ON [Dictionaries] ([StreamId]);

CREATE UNIQUE INDEX [IX_Dictionaries_TenantId_EntityId] ON [Dictionaries] ([TenantId], [EntityId]) WHERE [TenantId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_Dictionaries_TenantId_LocaleNormalized] ON [Dictionaries] ([TenantId], [LocaleNormalized]) WHERE [TenantId] IS NOT NULL;

CREATE INDEX [IX_Dictionaries_UpdatedBy] ON [Dictionaries] ([UpdatedBy]);

CREATE INDEX [IX_Dictionaries_UpdatedOn] ON [Dictionaries] ([UpdatedOn]);

CREATE INDEX [IX_Dictionaries_Version] ON [Dictionaries] ([Version]);

CREATE INDEX [IX_LogEvents_LogId] ON [LogEvents] ([LogId]);

CREATE INDEX [IX_LogExceptions_LogId] ON [LogExceptions] ([LogId]);

CREATE INDEX [IX_LogExceptions_Type] ON [LogExceptions] ([Type]);

CREATE INDEX [IX_Logs_ActivityType] ON [Logs] ([ActivityType]);

CREATE INDEX [IX_Logs_ActorId] ON [Logs] ([ActorId]);

CREATE INDEX [IX_Logs_ApiKeyId] ON [Logs] ([ApiKeyId]);

CREATE INDEX [IX_Logs_CorrelationId] ON [Logs] ([CorrelationId]);

CREATE INDEX [IX_Logs_Duration] ON [Logs] ([Duration]);

CREATE INDEX [IX_Logs_EndedOn] ON [Logs] ([EndedOn]);

CREATE INDEX [IX_Logs_HasErrors] ON [Logs] ([HasErrors]);

CREATE INDEX [IX_Logs_IsCompleted] ON [Logs] ([IsCompleted]);

CREATE INDEX [IX_Logs_Level] ON [Logs] ([Level]);

CREATE INDEX [IX_Logs_OperationName] ON [Logs] ([OperationName]);

CREATE INDEX [IX_Logs_OperationType] ON [Logs] ([OperationType]);

CREATE INDEX [IX_Logs_SessionId] ON [Logs] ([SessionId]);

CREATE INDEX [IX_Logs_StartedOn] ON [Logs] ([StartedOn]);

CREATE INDEX [IX_Logs_StatusCode] ON [Logs] ([StatusCode]);

CREATE INDEX [IX_Logs_TenantId] ON [Logs] ([TenantId]);

CREATE UNIQUE INDEX [IX_Logs_UniqueId] ON [Logs] ([UniqueId]);

CREATE INDEX [IX_Logs_UserId] ON [Logs] ([UserId]);

CREATE INDEX [IX_Messages_CreatedBy] ON [Messages] ([CreatedBy]);

CREATE INDEX [IX_Messages_CreatedOn] ON [Messages] ([CreatedOn]);

CREATE INDEX [IX_Messages_EntityId] ON [Messages] ([EntityId]);

CREATE INDEX [IX_Messages_IsDemo] ON [Messages] ([IsDemo]);

CREATE INDEX [IX_Messages_RecipientCount] ON [Messages] ([RecipientCount]);

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);

CREATE INDEX [IX_Messages_Status] ON [Messages] ([Status]);

CREATE UNIQUE INDEX [IX_Messages_StreamId] ON [Messages] ([StreamId]);

CREATE INDEX [IX_Messages_Subject] ON [Messages] ([Subject]);

CREATE INDEX [IX_Messages_TemplateId] ON [Messages] ([TemplateId]);

CREATE UNIQUE INDEX [IX_Messages_TenantId_EntityId] ON [Messages] ([TenantId], [EntityId]) WHERE [TenantId] IS NOT NULL;

CREATE INDEX [IX_Messages_UpdatedBy] ON [Messages] ([UpdatedBy]);

CREATE INDEX [IX_Messages_UpdatedOn] ON [Messages] ([UpdatedOn]);

CREATE INDEX [IX_Messages_Version] ON [Messages] ([Version]);

CREATE INDEX [IX_Realms_CreatedBy] ON [Realms] ([CreatedBy]);

CREATE INDEX [IX_Realms_CreatedOn] ON [Realms] ([CreatedOn]);

CREATE INDEX [IX_Realms_DisplayName] ON [Realms] ([DisplayName]);

CREATE UNIQUE INDEX [IX_Realms_StreamId] ON [Realms] ([StreamId]);

CREATE INDEX [IX_Realms_UniqueSlug] ON [Realms] ([UniqueSlug]);

CREATE UNIQUE INDEX [IX_Realms_UniqueSlugNormalized] ON [Realms] ([UniqueSlugNormalized]);

CREATE INDEX [IX_Realms_UpdatedBy] ON [Realms] ([UpdatedBy]);

CREATE INDEX [IX_Realms_UpdatedOn] ON [Realms] ([UpdatedOn]);

CREATE INDEX [IX_Realms_Version] ON [Realms] ([Version]);

CREATE INDEX [IX_Recipients_Address] ON [Recipients] ([Address]);

CREATE INDEX [IX_Recipients_DisplayName] ON [Recipients] ([DisplayName]);

CREATE INDEX [IX_Recipients_MessageId] ON [Recipients] ([MessageId]);

CREATE INDEX [IX_Recipients_PhoneNumber] ON [Recipients] ([PhoneNumber]);

CREATE INDEX [IX_Recipients_Type] ON [Recipients] ([Type]);

CREATE INDEX [IX_Recipients_UserId] ON [Recipients] ([UserId]);

CREATE INDEX [IX_Senders_CreatedBy] ON [Senders] ([CreatedBy]);

CREATE INDEX [IX_Senders_CreatedOn] ON [Senders] ([CreatedOn]);

CREATE INDEX [IX_Senders_DisplayName] ON [Senders] ([DisplayName]);

CREATE INDEX [IX_Senders_EmailAddress] ON [Senders] ([EmailAddress]);

CREATE INDEX [IX_Senders_EntityId] ON [Senders] ([EntityId]);

CREATE INDEX [IX_Senders_PhoneNumber] ON [Senders] ([PhoneNumber]);

CREATE INDEX [IX_Senders_Provider] ON [Senders] ([Provider]);

CREATE UNIQUE INDEX [IX_Senders_StreamId] ON [Senders] ([StreamId]);

CREATE UNIQUE INDEX [IX_Senders_TenantId_EntityId] ON [Senders] ([TenantId], [EntityId]) WHERE [TenantId] IS NOT NULL;

CREATE INDEX [IX_Senders_TenantId_IsDefault] ON [Senders] ([TenantId], [IsDefault]);

CREATE INDEX [IX_Senders_UpdatedBy] ON [Senders] ([UpdatedBy]);

CREATE INDEX [IX_Senders_UpdatedOn] ON [Senders] ([UpdatedOn]);

CREATE INDEX [IX_Senders_Version] ON [Senders] ([Version]);

CREATE INDEX [IX_Templates_ContentType] ON [Templates] ([ContentType]);

CREATE INDEX [IX_Templates_CreatedBy] ON [Templates] ([CreatedBy]);

CREATE INDEX [IX_Templates_CreatedOn] ON [Templates] ([CreatedOn]);

CREATE INDEX [IX_Templates_DisplayName] ON [Templates] ([DisplayName]);

CREATE INDEX [IX_Templates_EntityId] ON [Templates] ([EntityId]);

CREATE UNIQUE INDEX [IX_Templates_StreamId] ON [Templates] ([StreamId]);

CREATE INDEX [IX_Templates_Subject] ON [Templates] ([Subject]);

CREATE UNIQUE INDEX [IX_Templates_TenantId_EntityId] ON [Templates] ([TenantId], [EntityId]) WHERE [TenantId] IS NOT NULL;

CREATE UNIQUE INDEX [IX_Templates_TenantId_UniqueKeyNormalized] ON [Templates] ([TenantId], [UniqueKeyNormalized]) WHERE [TenantId] IS NOT NULL;

CREATE INDEX [IX_Templates_UniqueKey] ON [Templates] ([UniqueKey]);

CREATE INDEX [IX_Templates_UpdatedBy] ON [Templates] ([UpdatedBy]);

CREATE INDEX [IX_Templates_UpdatedOn] ON [Templates] ([UpdatedOn]);

CREATE INDEX [IX_Templates_Version] ON [Templates] ([Version]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241224073658_Release_5_0_0', N'9.0.0');

COMMIT;
GO
