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
GO

CREATE TABLE [Dictionaries] (
    [DictionaryId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [Locale] nvarchar(16) NOT NULL,
    [LocaleNormalized] nvarchar(16) NOT NULL,
    [EntryCount] int NOT NULL,
    [Entries] nvarchar(max) NULL,
    [AggregateId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NOT NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Dictionaries] PRIMARY KEY ([DictionaryId])
);
GO

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
    [TenantId] nvarchar(255) NULL,
    [ActorId] nvarchar(255) NOT NULL,
    [ApiKeyId] nvarchar(255) NULL,
    [UserId] nvarchar(255) NULL,
    [SessionId] nvarchar(255) NULL,
    CONSTRAINT [PK_Logs] PRIMARY KEY ([LogId])
);
GO

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
    [AggregateId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NOT NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Realms] PRIMARY KEY ([RealmId])
);
GO

CREATE TABLE [Senders] (
    [SenderId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [IsDefault] bit NOT NULL,
    [EmailAddress] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [Provider] nvarchar(255) NOT NULL,
    [Settings] nvarchar(max) NULL,
    [AggregateId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NOT NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Senders] PRIMARY KEY ([SenderId])
);
GO

CREATE TABLE [Templates] (
    [TemplateId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [UniqueKey] nvarchar(255) NOT NULL,
    [UniqueKeyNormalized] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [Description] nvarchar(max) NULL,
    [Subject] nvarchar(255) NOT NULL,
    [ContentType] nvarchar(10) NOT NULL,
    [ContentText] nvarchar(max) NOT NULL,
    [AggregateId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NOT NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Templates] PRIMARY KEY ([TemplateId])
);
GO

CREATE TABLE [LogEvents] (
    [EventId] uniqueidentifier NOT NULL,
    [LogId] bigint NOT NULL,
    CONSTRAINT [PK_LogEvents] PRIMARY KEY ([EventId]),
    CONSTRAINT [FK_LogEvents_Logs_LogId] FOREIGN KEY ([LogId]) REFERENCES [Logs] ([LogId]) ON DELETE CASCADE
);
GO

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
GO

CREATE TABLE [Messages] (
    [MessageId] int NOT NULL IDENTITY,
    [TenantId] nvarchar(255) NULL,
    [Subject] nvarchar(255) NOT NULL,
    [BodyType] nvarchar(10) NOT NULL,
    [BodyText] nvarchar(max) NOT NULL,
    [RecipientCount] int NOT NULL,
    [SenderId] int NULL,
    [SenderIsDefault] bit NOT NULL,
    [SenderAddress] nvarchar(255) NOT NULL,
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
    [AggregateId] nvarchar(255) NOT NULL,
    [Version] bigint NOT NULL,
    [CreatedBy] nvarchar(255) NOT NULL,
    [CreatedOn] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(255) NOT NULL,
    [UpdatedOn] datetime2 NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([MessageId]),
    CONSTRAINT [FK_Messages_Senders_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Senders] ([SenderId]) ON DELETE SET NULL,
    CONSTRAINT [FK_Messages_Templates_TemplateId] FOREIGN KEY ([TemplateId]) REFERENCES [Templates] ([TemplateId]) ON DELETE SET NULL
);
GO

CREATE TABLE [Recipients] (
    [RecipientId] int NOT NULL IDENTITY,
    [MessageId] int NOT NULL,
    [Type] nvarchar(3) NOT NULL,
    [Address] nvarchar(255) NOT NULL,
    [DisplayName] nvarchar(255) NULL,
    [UserId] int NULL,
    [UserUniqueName] nvarchar(255) NULL,
    [UserEmailAddress] nvarchar(255) NULL,
    [UserFullName] nvarchar(767) NULL,
    [UserPicture] nvarchar(2048) NULL,
    CONSTRAINT [PK_Recipients] PRIMARY KEY ([RecipientId]),
    CONSTRAINT [FK_Recipients_Messages_MessageId] FOREIGN KEY ([MessageId]) REFERENCES [Messages] ([MessageId]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Dictionaries_AggregateId] ON [Dictionaries] ([AggregateId]);
GO

CREATE INDEX [IX_Dictionaries_CreatedBy] ON [Dictionaries] ([CreatedBy]);
GO

CREATE INDEX [IX_Dictionaries_CreatedOn] ON [Dictionaries] ([CreatedOn]);
GO

CREATE INDEX [IX_Dictionaries_EntryCount] ON [Dictionaries] ([EntryCount]);
GO

CREATE INDEX [IX_Dictionaries_Locale] ON [Dictionaries] ([Locale]);
GO

CREATE UNIQUE INDEX [IX_Dictionaries_TenantId_LocaleNormalized] ON [Dictionaries] ([TenantId], [LocaleNormalized]) WHERE [TenantId] IS NOT NULL;
GO

CREATE INDEX [IX_Dictionaries_UpdatedBy] ON [Dictionaries] ([UpdatedBy]);
GO

CREATE INDEX [IX_Dictionaries_UpdatedOn] ON [Dictionaries] ([UpdatedOn]);
GO

CREATE INDEX [IX_Dictionaries_Version] ON [Dictionaries] ([Version]);
GO

CREATE INDEX [IX_LogEvents_LogId] ON [LogEvents] ([LogId]);
GO

CREATE INDEX [IX_LogExceptions_LogId] ON [LogExceptions] ([LogId]);
GO

CREATE INDEX [IX_LogExceptions_Type] ON [LogExceptions] ([Type]);
GO

CREATE INDEX [IX_Logs_ActivityType] ON [Logs] ([ActivityType]);
GO

CREATE INDEX [IX_Logs_ActorId] ON [Logs] ([ActorId]);
GO

CREATE INDEX [IX_Logs_ApiKeyId] ON [Logs] ([ApiKeyId]);
GO

CREATE INDEX [IX_Logs_CorrelationId] ON [Logs] ([CorrelationId]);
GO

CREATE INDEX [IX_Logs_Duration] ON [Logs] ([Duration]);
GO

CREATE INDEX [IX_Logs_EndedOn] ON [Logs] ([EndedOn]);
GO

CREATE INDEX [IX_Logs_HasErrors] ON [Logs] ([HasErrors]);
GO

CREATE INDEX [IX_Logs_IsCompleted] ON [Logs] ([IsCompleted]);
GO

CREATE INDEX [IX_Logs_Level] ON [Logs] ([Level]);
GO

CREATE INDEX [IX_Logs_OperationName] ON [Logs] ([OperationName]);
GO

CREATE INDEX [IX_Logs_OperationType] ON [Logs] ([OperationType]);
GO

CREATE INDEX [IX_Logs_SessionId] ON [Logs] ([SessionId]);
GO

CREATE INDEX [IX_Logs_StartedOn] ON [Logs] ([StartedOn]);
GO

CREATE INDEX [IX_Logs_StatusCode] ON [Logs] ([StatusCode]);
GO

CREATE INDEX [IX_Logs_TenantId] ON [Logs] ([TenantId]);
GO

CREATE UNIQUE INDEX [IX_Logs_UniqueId] ON [Logs] ([UniqueId]);
GO

CREATE INDEX [IX_Logs_UserId] ON [Logs] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Messages_AggregateId] ON [Messages] ([AggregateId]);
GO

CREATE INDEX [IX_Messages_CreatedBy] ON [Messages] ([CreatedBy]);
GO

CREATE INDEX [IX_Messages_CreatedOn] ON [Messages] ([CreatedOn]);
GO

CREATE INDEX [IX_Messages_IsDemo] ON [Messages] ([IsDemo]);
GO

CREATE INDEX [IX_Messages_RecipientCount] ON [Messages] ([RecipientCount]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Messages_Status] ON [Messages] ([Status]);
GO

CREATE INDEX [IX_Messages_Subject] ON [Messages] ([Subject]);
GO

CREATE INDEX [IX_Messages_TemplateId] ON [Messages] ([TemplateId]);
GO

CREATE INDEX [IX_Messages_TenantId] ON [Messages] ([TenantId]);
GO

CREATE INDEX [IX_Messages_UpdatedBy] ON [Messages] ([UpdatedBy]);
GO

CREATE INDEX [IX_Messages_UpdatedOn] ON [Messages] ([UpdatedOn]);
GO

CREATE INDEX [IX_Messages_Version] ON [Messages] ([Version]);
GO

CREATE UNIQUE INDEX [IX_Realms_AggregateId] ON [Realms] ([AggregateId]);
GO

CREATE INDEX [IX_Realms_CreatedBy] ON [Realms] ([CreatedBy]);
GO

CREATE INDEX [IX_Realms_CreatedOn] ON [Realms] ([CreatedOn]);
GO

CREATE INDEX [IX_Realms_DisplayName] ON [Realms] ([DisplayName]);
GO

CREATE INDEX [IX_Realms_UniqueSlug] ON [Realms] ([UniqueSlug]);
GO

CREATE UNIQUE INDEX [IX_Realms_UniqueSlugNormalized] ON [Realms] ([UniqueSlugNormalized]);
GO

CREATE INDEX [IX_Realms_UpdatedBy] ON [Realms] ([UpdatedBy]);
GO

CREATE INDEX [IX_Realms_UpdatedOn] ON [Realms] ([UpdatedOn]);
GO

CREATE INDEX [IX_Realms_Version] ON [Realms] ([Version]);
GO

CREATE INDEX [IX_Recipients_Address] ON [Recipients] ([Address]);
GO

CREATE INDEX [IX_Recipients_DisplayName] ON [Recipients] ([DisplayName]);
GO

CREATE INDEX [IX_Recipients_MessageId] ON [Recipients] ([MessageId]);
GO

CREATE INDEX [IX_Recipients_Type] ON [Recipients] ([Type]);
GO

CREATE INDEX [IX_Recipients_UserId] ON [Recipients] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Senders_AggregateId] ON [Senders] ([AggregateId]);
GO

CREATE INDEX [IX_Senders_CreatedBy] ON [Senders] ([CreatedBy]);
GO

CREATE INDEX [IX_Senders_CreatedOn] ON [Senders] ([CreatedOn]);
GO

CREATE INDEX [IX_Senders_DisplayName] ON [Senders] ([DisplayName]);
GO

CREATE INDEX [IX_Senders_EmailAddress] ON [Senders] ([EmailAddress]);
GO

CREATE INDEX [IX_Senders_Provider] ON [Senders] ([Provider]);
GO

CREATE INDEX [IX_Senders_TenantId_IsDefault] ON [Senders] ([TenantId], [IsDefault]);
GO

CREATE INDEX [IX_Senders_UpdatedBy] ON [Senders] ([UpdatedBy]);
GO

CREATE INDEX [IX_Senders_UpdatedOn] ON [Senders] ([UpdatedOn]);
GO

CREATE INDEX [IX_Senders_Version] ON [Senders] ([Version]);
GO

CREATE UNIQUE INDEX [IX_Templates_AggregateId] ON [Templates] ([AggregateId]);
GO

CREATE INDEX [IX_Templates_ContentType] ON [Templates] ([ContentType]);
GO

CREATE INDEX [IX_Templates_CreatedBy] ON [Templates] ([CreatedBy]);
GO

CREATE INDEX [IX_Templates_CreatedOn] ON [Templates] ([CreatedOn]);
GO

CREATE INDEX [IX_Templates_DisplayName] ON [Templates] ([DisplayName]);
GO

CREATE INDEX [IX_Templates_Subject] ON [Templates] ([Subject]);
GO

CREATE UNIQUE INDEX [IX_Templates_TenantId_UniqueKeyNormalized] ON [Templates] ([TenantId], [UniqueKeyNormalized]) WHERE [TenantId] IS NOT NULL;
GO

CREATE INDEX [IX_Templates_UniqueKey] ON [Templates] ([UniqueKey]);
GO

CREATE INDEX [IX_Templates_UpdatedBy] ON [Templates] ([UpdatedBy]);
GO

CREATE INDEX [IX_Templates_UpdatedOn] ON [Templates] ([UpdatedOn]);
GO

CREATE INDEX [IX_Templates_Version] ON [Templates] ([Version]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240305193931_LogitarPortal_4_0_0', N'8.0.2');
GO

COMMIT;
GO
