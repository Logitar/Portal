CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'Logging') THEN
        CREATE SCHEMA "Logging";
    END IF;
END $EF$;

CREATE TABLE "Logging"."Logs" (
    "LogId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Id" uuid NOT NULL,
    "CorrelationId" character varying(255) NULL,
    "Method" character varying(255) NULL,
    "Destination" character varying(65535) NULL,
    "Source" character varying(65535) NULL,
    "AdditionalInformation" text NULL,
    "OperationType" character varying(255) NULL,
    "OperationName" character varying(255) NULL,
    "StatusCode" integer NULL,
    "StartedOn" timestamp with time zone NOT NULL,
    "EndedOn" timestamp with time zone NULL,
    "Duration" interval NULL,
    "ActorId" uuid NOT NULL,
    "UserId" uuid NULL,
    "SessionId" uuid NULL,
    "IsCompleted" boolean NOT NULL,
    "Level" character varying(255) NOT NULL,
    "HasErrors" boolean NOT NULL,
    "Errors" jsonb NULL,
    CONSTRAINT "PK_Logs" PRIMARY KEY ("LogId")
);

CREATE TABLE "Messages" (
    "MessageId" integer GENERATED BY DEFAULT AS IDENTITY,
    "IsDemo" boolean NOT NULL,
    "Subject" character varying(255) NOT NULL,
    "Body" text NOT NULL,
    "Recipients" jsonb NOT NULL,
    "RecipientCount" integer NOT NULL,
    "RealmId" uuid NULL,
    "RealmUniqueName" character varying(255) NULL,
    "RealmUniqueNameNormalized" character varying(255) NULL,
    "RealmDisplayName" character varying(255) NULL,
    "SenderId" uuid NOT NULL,
    "SenderIsDefault" boolean NOT NULL,
    "SenderProvider" character varying(255) NOT NULL,
    "SenderEmailAddress" character varying(255) NOT NULL,
    "SenderDisplayName" character varying(255) NULL,
    "TemplateId" uuid NOT NULL,
    "TemplateUniqueName" character varying(255) NOT NULL,
    "TemplateDisplayName" character varying(255) NULL,
    "TemplateContentType" character varying(255) NOT NULL,
    "IgnoreUserLocale" boolean NOT NULL,
    "Locale" character varying(255) NULL,
    "Variables" jsonb NULL,
    "Errors" jsonb NULL,
    "HasErrors" boolean NOT NULL,
    "Result" jsonb NULL,
    "Succeeded" boolean NOT NULL,
    "AggregateId" character varying(255) NOT NULL,
    "Version" bigint NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_Messages" PRIMARY KEY ("MessageId")
);

CREATE TABLE "TokenBlacklist" (
    "BlacklistedTokenId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Id" uuid NOT NULL,
    "ExpiresOn" timestamp with time zone NULL,
    CONSTRAINT "PK_TokenBlacklist" PRIMARY KEY ("BlacklistedTokenId")
);

CREATE TABLE "Logging"."Activities" (
    "ActivityId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Id" uuid NOT NULL,
    "LogId" bigint NOT NULL,
    "Type" character varying(255) NOT NULL,
    "Data" jsonb NOT NULL,
    "StartedOn" timestamp with time zone NOT NULL,
    "EndedOn" timestamp with time zone NULL,
    "Duration" interval NULL,
    "IsCompleted" boolean NOT NULL,
    "Level" character varying(255) NOT NULL,
    "HasErrors" boolean NOT NULL,
    "Errors" jsonb NULL,
    CONSTRAINT "PK_Activities" PRIMARY KEY ("ActivityId"),
    CONSTRAINT "FK_Activities_Logs_LogId" FOREIGN KEY ("LogId") REFERENCES "Logging"."Logs" ("LogId") ON DELETE CASCADE
);

CREATE TABLE "Logging"."Events" (
    "EventId" bigint NOT NULL,
    "LogId" bigint NOT NULL,
    "ActivityId" bigint NULL,
    CONSTRAINT "PK_Events" PRIMARY KEY ("EventId"),
    CONSTRAINT "FK_Events_Activities_ActivityId" FOREIGN KEY ("ActivityId") REFERENCES "Logging"."Activities" ("ActivityId") ON DELETE CASCADE,
    CONSTRAINT "FK_Events_Logs_LogId" FOREIGN KEY ("LogId") REFERENCES "Logging"."Logs" ("LogId") ON DELETE CASCADE
);

CREATE TABLE "Dictionaries" (
    "DictionaryId" integer GENERATED BY DEFAULT AS IDENTITY,
    "RealmId" integer NULL,
    "Locale" character varying(255) NOT NULL,
    "Entries" jsonb NULL,
    "EntryCount" integer NOT NULL,
    "AggregateId" character varying(255) NOT NULL,
    "Version" bigint NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_Dictionaries" PRIMARY KEY ("DictionaryId")
);

CREATE TABLE "ExternalIdentifiers" (
    "ExternalIdentifierId" integer GENERATED BY DEFAULT AS IDENTITY,
    "Id" uuid NOT NULL,
    "RealmId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "Key" character varying(255) NOT NULL,
    "Value" character varying(255) NOT NULL,
    "ValueNormalized" character varying(255) NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_ExternalIdentifiers" PRIMARY KEY ("ExternalIdentifierId")
);

CREATE TABLE "Realms" (
    "RealmId" integer GENERATED BY DEFAULT AS IDENTITY,
    "UniqueName" character varying(255) NOT NULL,
    "UniqueNameNormalized" character varying(255) NOT NULL,
    "DisplayName" character varying(255) NULL,
    "Description" text NULL,
    "DefaultLocale" character varying(255) NULL,
    "Secret" character varying(255) NOT NULL,
    "Url" character varying(65535) NULL,
    "RequireConfirmedAccount" boolean NOT NULL,
    "RequireUniqueEmail" boolean NOT NULL,
    "UsernameSettings" jsonb NOT NULL,
    "PasswordSettings" jsonb NOT NULL,
    "ClaimMappings" jsonb NULL,
    "CustomAttributes" jsonb NULL,
    "PasswordRecoverySenderId" integer NULL,
    "PasswordRecoveryTemplateId" integer NULL,
    "AggregateId" character varying(255) NOT NULL,
    "Version" bigint NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_Realms" PRIMARY KEY ("RealmId")
);

CREATE TABLE "Senders" (
    "SenderId" integer GENERATED BY DEFAULT AS IDENTITY,
    "RealmId" integer NULL,
    "IsDefault" boolean NOT NULL,
    "EmailAddress" character varying(255) NOT NULL,
    "DisplayName" character varying(255) NULL,
    "Provider" character varying(255) NOT NULL,
    "Settings" jsonb NULL,
    "AggregateId" character varying(255) NOT NULL,
    "Version" bigint NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_Senders" PRIMARY KEY ("SenderId"),
    CONSTRAINT "FK_Senders_Realms_RealmId" FOREIGN KEY ("RealmId") REFERENCES "Realms" ("RealmId") ON DELETE RESTRICT
);

CREATE TABLE "Templates" (
    "TemplateId" integer GENERATED BY DEFAULT AS IDENTITY,
    "RealmId" integer NULL,
    "UniqueName" character varying(255) NOT NULL,
    "UniqueNameNormalized" character varying(255) NOT NULL,
    "DisplayName" character varying(255) NULL,
    "Description" text NULL,
    "Subject" character varying(255) NOT NULL,
    "ContentType" character varying(255) NOT NULL,
    "Contents" text NOT NULL,
    "AggregateId" character varying(255) NOT NULL,
    "Version" bigint NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_Templates" PRIMARY KEY ("TemplateId"),
    CONSTRAINT "FK_Templates_Realms_RealmId" FOREIGN KEY ("RealmId") REFERENCES "Realms" ("RealmId") ON DELETE RESTRICT
);

CREATE TABLE "Users" (
    "UserId" integer GENERATED BY DEFAULT AS IDENTITY,
    "RealmId" integer NULL,
    "Username" character varying(255) NOT NULL,
    "UsernameNormalized" character varying(255) NOT NULL,
    "PasswordChangedById" uuid NULL,
    "PasswordChangedBy" jsonb NULL,
    "PasswordChangedOn" timestamp with time zone NULL,
    "Password" character varying(255) NULL,
    "HasPassword" boolean NOT NULL,
    "DisabledById" uuid NULL,
    "DisabledBy" jsonb NULL,
    "DisabledOn" timestamp with time zone NULL,
    "IsDisabled" boolean NOT NULL,
    "SignedInOn" timestamp with time zone NULL,
    "AddressLine1" character varying(255) NULL,
    "AddressLine2" character varying(255) NULL,
    "AddressLocality" character varying(255) NULL,
    "AddressPostalCode" character varying(255) NULL,
    "AddressCountry" character varying(255) NULL,
    "AddressRegion" character varying(255) NULL,
    "AddressFormatted" character varying(65535) NULL,
    "AddressVerifiedById" uuid NULL,
    "AddressVerifiedBy" jsonb NULL,
    "AddressVerifiedOn" timestamp with time zone NULL,
    "IsAddressVerified" boolean NOT NULL,
    "EmailAddress" character varying(255) NULL,
    "EmailAddressNormalized" character varying(255) NULL,
    "EmailVerifiedById" uuid NULL,
    "EmailVerifiedBy" jsonb NULL,
    "EmailVerifiedOn" timestamp with time zone NULL,
    "IsEmailVerified" boolean NOT NULL,
    "PhoneCountryCode" character varying(255) NULL,
    "PhoneNumber" character varying(255) NULL,
    "PhoneExtension" character varying(255) NULL,
    "PhoneE164Formatted" character varying(255) NULL,
    "PhoneVerifiedById" uuid NULL,
    "PhoneVerifiedBy" jsonb NULL,
    "PhoneVerifiedOn" timestamp with time zone NULL,
    "IsPhoneVerified" boolean NOT NULL,
    "IsConfirmed" boolean NOT NULL,
    "FirstName" character varying(255) NULL,
    "MiddleName" character varying(255) NULL,
    "LastName" character varying(255) NULL,
    "FullName" character varying(65535) NULL,
    "Nickname" character varying(255) NULL,
    "Birthdate" timestamp with time zone NULL,
    "Gender" character varying(255) NULL,
    "Locale" character varying(255) NULL,
    "TimeZone" character varying(255) NULL,
    "Picture" character varying(65535) NULL,
    "Profile" character varying(65535) NULL,
    "Website" character varying(65535) NULL,
    "CustomAttributes" jsonb NULL,
    "AggregateId" character varying(255) NOT NULL,
    "Version" bigint NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_Users_Realms_RealmId" FOREIGN KEY ("RealmId") REFERENCES "Realms" ("RealmId") ON DELETE RESTRICT
);

CREATE TABLE "Sessions" (
    "SessionId" integer GENERATED BY DEFAULT AS IDENTITY,
    "UserId" integer NOT NULL,
    "Key" character varying(255) NULL,
    "IsPersistent" boolean NOT NULL,
    "SignedOutById" uuid NULL,
    "SignedOutBy" jsonb NULL,
    "SignedOutOn" timestamp with time zone NULL,
    "IsActive" boolean NOT NULL,
    "IpAddress" character varying(255) NULL,
    "AdditionalInformation" text NULL,
    "CustomAttributes" jsonb NULL,
    "AggregateId" character varying(255) NOT NULL,
    "Version" bigint NOT NULL,
    "CreatedById" uuid NOT NULL,
    "CreatedBy" jsonb NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedById" uuid NULL,
    "UpdatedBy" jsonb NULL,
    "UpdatedOn" timestamp with time zone NULL,
    CONSTRAINT "PK_Sessions" PRIMARY KEY ("SessionId"),
    CONSTRAINT "FK_Sessions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE RESTRICT
);

CREATE UNIQUE INDEX "IX_Activities_Id" ON "Logging"."Activities" ("Id");

CREATE INDEX "IX_Activities_LogId" ON "Logging"."Activities" ("LogId");

CREATE UNIQUE INDEX "IX_Dictionaries_AggregateId" ON "Dictionaries" ("AggregateId");

CREATE INDEX "IX_Dictionaries_CreatedById" ON "Dictionaries" ("CreatedById");

CREATE INDEX "IX_Dictionaries_CreatedOn" ON "Dictionaries" ("CreatedOn");

CREATE INDEX "IX_Dictionaries_EntryCount" ON "Dictionaries" ("EntryCount");

CREATE INDEX "IX_Dictionaries_Locale" ON "Dictionaries" ("Locale");

CREATE UNIQUE INDEX "IX_Dictionaries_RealmId_Locale" ON "Dictionaries" ("RealmId", "Locale");

CREATE INDEX "IX_Dictionaries_UpdatedById" ON "Dictionaries" ("UpdatedById");

CREATE INDEX "IX_Dictionaries_UpdatedOn" ON "Dictionaries" ("UpdatedOn");

CREATE INDEX "IX_Events_ActivityId" ON "Logging"."Events" ("ActivityId");

CREATE INDEX "IX_Events_LogId" ON "Logging"."Events" ("LogId");

CREATE INDEX "IX_ExternalIdentifiers_CreatedById" ON "ExternalIdentifiers" ("CreatedById");

CREATE UNIQUE INDEX "IX_ExternalIdentifiers_Id" ON "ExternalIdentifiers" ("Id");

CREATE UNIQUE INDEX "IX_ExternalIdentifiers_RealmId_Key_ValueNormalized" ON "ExternalIdentifiers" ("RealmId", "Key", "ValueNormalized");

CREATE INDEX "IX_ExternalIdentifiers_UpdatedById" ON "ExternalIdentifiers" ("UpdatedById");

CREATE INDEX "IX_ExternalIdentifiers_UserId" ON "ExternalIdentifiers" ("UserId");

CREATE UNIQUE INDEX "IX_Logs_Id" ON "Logging"."Logs" ("Id");

CREATE UNIQUE INDEX "IX_Messages_AggregateId" ON "Messages" ("AggregateId");

CREATE INDEX "IX_Messages_CreatedById" ON "Messages" ("CreatedById");

CREATE INDEX "IX_Messages_CreatedOn" ON "Messages" ("CreatedOn");

CREATE INDEX "IX_Messages_HasErrors" ON "Messages" ("HasErrors");

CREATE INDEX "IX_Messages_IsDemo" ON "Messages" ("IsDemo");

CREATE INDEX "IX_Messages_RealmDisplayName" ON "Messages" ("RealmDisplayName");

CREATE INDEX "IX_Messages_RealmId" ON "Messages" ("RealmId");

CREATE INDEX "IX_Messages_RealmUniqueName" ON "Messages" ("RealmUniqueName");

CREATE INDEX "IX_Messages_SenderDisplayName" ON "Messages" ("SenderDisplayName");

CREATE INDEX "IX_Messages_SenderEmailAddress" ON "Messages" ("SenderEmailAddress");

CREATE INDEX "IX_Messages_SenderId" ON "Messages" ("SenderId");

CREATE INDEX "IX_Messages_Subject" ON "Messages" ("Subject");

CREATE INDEX "IX_Messages_Succeeded" ON "Messages" ("Succeeded");

CREATE INDEX "IX_Messages_TemplateDisplayName" ON "Messages" ("TemplateDisplayName");

CREATE INDEX "IX_Messages_TemplateId" ON "Messages" ("TemplateId");

CREATE INDEX "IX_Messages_TemplateUniqueName" ON "Messages" ("TemplateUniqueName");

CREATE INDEX "IX_Messages_UpdatedById" ON "Messages" ("UpdatedById");

CREATE INDEX "IX_Messages_UpdatedOn" ON "Messages" ("UpdatedOn");

CREATE UNIQUE INDEX "IX_Realms_AggregateId" ON "Realms" ("AggregateId");

CREATE INDEX "IX_Realms_CreatedById" ON "Realms" ("CreatedById");

CREATE INDEX "IX_Realms_CreatedOn" ON "Realms" ("CreatedOn");

CREATE INDEX "IX_Realms_DisplayName" ON "Realms" ("DisplayName");

CREATE UNIQUE INDEX "IX_Realms_PasswordRecoverySenderId" ON "Realms" ("PasswordRecoverySenderId");

CREATE UNIQUE INDEX "IX_Realms_PasswordRecoveryTemplateId" ON "Realms" ("PasswordRecoveryTemplateId");

CREATE INDEX "IX_Realms_UniqueName" ON "Realms" ("UniqueName");

CREATE UNIQUE INDEX "IX_Realms_UniqueNameNormalized" ON "Realms" ("UniqueNameNormalized");

CREATE INDEX "IX_Realms_UpdatedById" ON "Realms" ("UpdatedById");

CREATE INDEX "IX_Realms_UpdatedOn" ON "Realms" ("UpdatedOn");

CREATE UNIQUE INDEX "IX_Senders_AggregateId" ON "Senders" ("AggregateId");

CREATE INDEX "IX_Senders_CreatedById" ON "Senders" ("CreatedById");

CREATE INDEX "IX_Senders_CreatedOn" ON "Senders" ("CreatedOn");

CREATE INDEX "IX_Senders_DisplayName" ON "Senders" ("DisplayName");

CREATE INDEX "IX_Senders_EmailAddress" ON "Senders" ("EmailAddress");

CREATE INDEX "IX_Senders_Provider" ON "Senders" ("Provider");

CREATE UNIQUE INDEX "IX_Senders_RealmId_IsDefault" ON "Senders" ("RealmId", "IsDefault") WHERE "IsDefault" = true;

CREATE INDEX "IX_Senders_UpdatedById" ON "Senders" ("UpdatedById");

CREATE INDEX "IX_Senders_UpdatedOn" ON "Senders" ("UpdatedOn");

CREATE UNIQUE INDEX "IX_Sessions_AggregateId" ON "Sessions" ("AggregateId");

CREATE INDEX "IX_Sessions_CreatedById" ON "Sessions" ("CreatedById");

CREATE INDEX "IX_Sessions_CreatedOn" ON "Sessions" ("CreatedOn");

CREATE INDEX "IX_Sessions_IsActive" ON "Sessions" ("IsActive");

CREATE INDEX "IX_Sessions_IsPersistent" ON "Sessions" ("IsPersistent");

CREATE INDEX "IX_Sessions_SignedOutById" ON "Sessions" ("SignedOutById");

CREATE INDEX "IX_Sessions_SignedOutOn" ON "Sessions" ("SignedOutOn");

CREATE INDEX "IX_Sessions_UpdatedById" ON "Sessions" ("UpdatedById");

CREATE INDEX "IX_Sessions_UpdatedOn" ON "Sessions" ("UpdatedOn");

CREATE INDEX "IX_Sessions_UserId" ON "Sessions" ("UserId");

CREATE UNIQUE INDEX "IX_Templates_AggregateId" ON "Templates" ("AggregateId");

CREATE INDEX "IX_Templates_CreatedById" ON "Templates" ("CreatedById");

CREATE INDEX "IX_Templates_CreatedOn" ON "Templates" ("CreatedOn");

CREATE INDEX "IX_Templates_DisplayName" ON "Templates" ("DisplayName");

CREATE UNIQUE INDEX "IX_Templates_RealmId_UniqueNameNormalized" ON "Templates" ("RealmId", "UniqueNameNormalized");

CREATE INDEX "IX_Templates_UniqueName" ON "Templates" ("UniqueName");

CREATE INDEX "IX_Templates_UpdatedById" ON "Templates" ("UpdatedById");

CREATE INDEX "IX_Templates_UpdatedOn" ON "Templates" ("UpdatedOn");

CREATE INDEX "IX_TokenBlacklist_ExpiresOn" ON "TokenBlacklist" ("ExpiresOn");

CREATE UNIQUE INDEX "IX_TokenBlacklist_Id" ON "TokenBlacklist" ("Id");

CREATE INDEX "IX_Users_AddressFormatted" ON "Users" ("AddressFormatted");

CREATE INDEX "IX_Users_AddressVerifiedById" ON "Users" ("AddressVerifiedById");

CREATE UNIQUE INDEX "IX_Users_AggregateId" ON "Users" ("AggregateId");

CREATE INDEX "IX_Users_CreatedById" ON "Users" ("CreatedById");

CREATE INDEX "IX_Users_CreatedOn" ON "Users" ("CreatedOn");

CREATE INDEX "IX_Users_DisabledById" ON "Users" ("DisabledById");

CREATE INDEX "IX_Users_DisabledOn" ON "Users" ("DisabledOn");

CREATE INDEX "IX_Users_EmailAddress" ON "Users" ("EmailAddress");

CREATE INDEX "IX_Users_EmailVerifiedById" ON "Users" ("EmailVerifiedById");

CREATE INDEX "IX_Users_FirstName" ON "Users" ("FirstName");

CREATE INDEX "IX_Users_FullName" ON "Users" ("FullName");

CREATE INDEX "IX_Users_IsConfirmed" ON "Users" ("IsConfirmed");

CREATE INDEX "IX_Users_IsDisabled" ON "Users" ("IsDisabled");

CREATE INDEX "IX_Users_LastName" ON "Users" ("LastName");

CREATE INDEX "IX_Users_MiddleName" ON "Users" ("MiddleName");

CREATE INDEX "IX_Users_Nickname" ON "Users" ("Nickname");

CREATE INDEX "IX_Users_PasswordChangedById" ON "Users" ("PasswordChangedById");

CREATE INDEX "IX_Users_PasswordChangedOn" ON "Users" ("PasswordChangedOn");

CREATE INDEX "IX_Users_PhoneE164Formatted" ON "Users" ("PhoneE164Formatted");

CREATE INDEX "IX_Users_PhoneVerifiedById" ON "Users" ("PhoneVerifiedById");

CREATE INDEX "IX_Users_RealmId_EmailAddressNormalized" ON "Users" ("RealmId", "EmailAddressNormalized");

CREATE UNIQUE INDEX "IX_Users_RealmId_UsernameNormalized" ON "Users" ("RealmId", "UsernameNormalized");

CREATE INDEX "IX_Users_SignedInOn" ON "Users" ("SignedInOn");

CREATE INDEX "IX_Users_UpdatedById" ON "Users" ("UpdatedById");

CREATE INDEX "IX_Users_UpdatedOn" ON "Users" ("UpdatedOn");

CREATE INDEX "IX_Users_Username" ON "Users" ("Username");

ALTER TABLE "Dictionaries" ADD CONSTRAINT "FK_Dictionaries_Realms_RealmId" FOREIGN KEY ("RealmId") REFERENCES "Realms" ("RealmId") ON DELETE RESTRICT;

ALTER TABLE "ExternalIdentifiers" ADD CONSTRAINT "FK_ExternalIdentifiers_Realms_RealmId" FOREIGN KEY ("RealmId") REFERENCES "Realms" ("RealmId") ON DELETE CASCADE;

ALTER TABLE "ExternalIdentifiers" ADD CONSTRAINT "FK_ExternalIdentifiers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE;

ALTER TABLE "Realms" ADD CONSTRAINT "FK_Realms_Senders_PasswordRecoverySenderId" FOREIGN KEY ("PasswordRecoverySenderId") REFERENCES "Senders" ("SenderId") ON DELETE RESTRICT;

ALTER TABLE "Realms" ADD CONSTRAINT "FK_Realms_Templates_PasswordRecoveryTemplateId" FOREIGN KEY ("PasswordRecoveryTemplateId") REFERENCES "Templates" ("TemplateId") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230415044658_Release_v2_0_0', '7.0.5');

COMMIT;
