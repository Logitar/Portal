CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Actors" (
    "ActorId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Id" uuid NOT NULL,
    "Type" character varying(255) NOT NULL,
    "IsDeleted" boolean NOT NULL,
    "DisplayName" character varying(255) NOT NULL,
    "EmailAddress" character varying(255) NULL,
    "PictureUrl" character varying(2048) NULL,
    CONSTRAINT "PK_Actors" PRIMARY KEY ("ActorId")
);

CREATE TABLE "Realms" (
    "RealmId" integer GENERATED BY DEFAULT AS IDENTITY,
    "UniqueSlug" character varying(255) NOT NULL,
    "UniqueSlugNormalized" character varying(255) NOT NULL,
    "DisplayName" character varying(255) NULL,
    "Description" text NULL,
    "DefaultLocale" character varying(16) NULL,
    "Secret" character varying(64) NOT NULL,
    "Url" character varying(2048) NULL,
    "RequireUniqueEmail" boolean NOT NULL,
    "RequireConfirmedAccount" boolean NOT NULL,
    "AllowedUniqueNameCharacters" character varying(255) NULL,
    "RequiredPasswordLength" integer NOT NULL,
    "RequiredPasswordUniqueChars" integer NOT NULL,
    "PasswordsRequireNonAlphanumeric" boolean NOT NULL,
    "PasswordsRequireLowercase" boolean NOT NULL,
    "PasswordsRequireUppercase" boolean NOT NULL,
    "PasswordsRequireDigit" boolean NOT NULL,
    "PasswordStrategy" character varying(255) NOT NULL,
    "ClaimMappings" text NULL,
    "CustomAttributes" text NULL,
    "AggregateId" character varying(255) NOT NULL,
    "CreatedBy" character varying(255) NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255) NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_Realms" PRIMARY KEY ("RealmId")
);

CREATE TABLE "Roles" (
    "RoleId" integer GENERATED BY DEFAULT AS IDENTITY,
    "TenantId" character varying(255) NULL,
    "UniqueName" character varying(255) NOT NULL,
    "UniqueNameNormalized" character varying(255) NOT NULL,
    "DisplayName" character varying(255) NULL,
    "Description" text NULL,
    "CustomAttributes" text NULL,
    "AggregateId" character varying(255) NOT NULL,
    "CreatedBy" character varying(255) NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255) NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_Roles" PRIMARY KEY ("RoleId")
);

CREATE TABLE "Users" (
    "UserId" integer GENERATED BY DEFAULT AS IDENTITY,
    "TenantId" character varying(255) NULL,
    "UniqueName" character varying(255) NOT NULL,
    "UniqueNameNormalized" character varying(255) NOT NULL,
    "HasPassword" boolean NOT NULL,
    "Password" character varying(255) NULL,
    "PasswordChangedBy" character varying(255) NULL,
    "PasswordChangedOn" timestamp with time zone NULL,
    "DisabledBy" character varying(255) NULL,
    "DisabledOn" timestamp with time zone NULL,
    "IsDisabled" boolean NOT NULL,
    "AuthenticatedOn" timestamp with time zone NULL,
    "AddressStreet" character varying(255) NULL,
    "AddressLocality" character varying(255) NULL,
    "AddressRegion" character varying(255) NULL,
    "AddressPostalCode" character varying(255) NULL,
    "AddressCountry" character varying(255) NULL,
    "AddressFormatted" character varying(1536) NULL,
    "AddressVerifiedBy" character varying(255) NULL,
    "AddressVerifiedOn" timestamp with time zone NULL,
    "IsAddressVerified" boolean NOT NULL,
    "EmailAddress" character varying(255) NULL,
    "EmailAddressNormalized" character varying(255) NULL,
    "EmailVerifiedBy" character varying(255) NULL,
    "EmailVerifiedOn" timestamp with time zone NULL,
    "IsEmailVerified" boolean NOT NULL,
    "PhoneCountryCode" character varying(16) NULL,
    "PhoneNumber" character varying(32) NULL,
    "PhoneExtension" character varying(16) NULL,
    "PhoneE164Formatted" character varying(16) NULL,
    "PhoneVerifiedBy" character varying(255) NULL,
    "PhoneVerifiedOn" timestamp with time zone NULL,
    "IsPhoneVerified" boolean NOT NULL,
    "IsConfirmed" boolean NOT NULL,
    "FirstName" character varying(255) NULL,
    "MiddleName" character varying(255) NULL,
    "LastName" character varying(255) NULL,
    "FullName" character varying(768) NULL,
    "Nickname" character varying(255) NULL,
    "Birthdate" timestamp with time zone NULL,
    "Gender" character varying(255) NULL,
    "Locale" character varying(16) NULL,
    "TimeZone" character varying(255) NULL,
    "Picture" character varying(2048) NULL,
    "Profile" character varying(2048) NULL,
    "Website" character varying(2048) NULL,
    "CustomAttributes" text NULL,
    "AggregateId" character varying(255) NOT NULL,
    "CreatedBy" character varying(255) NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255) NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("UserId")
);

CREATE TABLE "Sessions" (
    "SessionId" integer GENERATED BY DEFAULT AS IDENTITY,
    "UserId" integer NOT NULL,
    "IsPersistent" boolean NOT NULL,
    "Secret" character varying(255) NULL,
    "IsActive" boolean NOT NULL,
    "SignedOutBy" character varying(255) NULL,
    "SignedOutOn" timestamp with time zone NULL,
    "CustomAttributes" text NULL,
    "AggregateId" character varying(255) NOT NULL,
    "CreatedBy" character varying(255) NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255) NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_Sessions" PRIMARY KEY ("SessionId"),
    CONSTRAINT "FK_Sessions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE RESTRICT
);

CREATE TABLE "UserRoles" (
    "UserId" integer NOT NULL,
    "RoleId" integer NOT NULL,
    CONSTRAINT "PK_UserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("RoleId") ON DELETE CASCADE,
    CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE INDEX "IX_Actors_DisplayName" ON "Actors" ("DisplayName");

CREATE INDEX "IX_Actors_EmailAddress" ON "Actors" ("EmailAddress");

CREATE UNIQUE INDEX "IX_Actors_Id" ON "Actors" ("Id");

CREATE INDEX "IX_Actors_IsDeleted" ON "Actors" ("IsDeleted");

CREATE INDEX "IX_Actors_Type" ON "Actors" ("Type");

CREATE UNIQUE INDEX "IX_Realms_AggregateId" ON "Realms" ("AggregateId");

CREATE INDEX "IX_Realms_CreatedBy" ON "Realms" ("CreatedBy");

CREATE INDEX "IX_Realms_CreatedOn" ON "Realms" ("CreatedOn");

CREATE INDEX "IX_Realms_DisplayName" ON "Realms" ("DisplayName");

CREATE INDEX "IX_Realms_UniqueSlug" ON "Realms" ("UniqueSlug");

CREATE UNIQUE INDEX "IX_Realms_UniqueSlugNormalized" ON "Realms" ("UniqueSlugNormalized");

CREATE INDEX "IX_Realms_UpdatedBy" ON "Realms" ("UpdatedBy");

CREATE INDEX "IX_Realms_UpdatedOn" ON "Realms" ("UpdatedOn");

CREATE INDEX "IX_Realms_Version" ON "Realms" ("Version");

CREATE UNIQUE INDEX "IX_Roles_AggregateId" ON "Roles" ("AggregateId");

CREATE INDEX "IX_Roles_CreatedBy" ON "Roles" ("CreatedBy");

CREATE INDEX "IX_Roles_CreatedOn" ON "Roles" ("CreatedOn");

CREATE INDEX "IX_Roles_DisplayName" ON "Roles" ("DisplayName");

CREATE INDEX "IX_Roles_TenantId" ON "Roles" ("TenantId");

CREATE UNIQUE INDEX "IX_Roles_TenantId_UniqueNameNormalized" ON "Roles" ("TenantId", "UniqueNameNormalized");

CREATE INDEX "IX_Roles_UniqueName" ON "Roles" ("UniqueName");

CREATE INDEX "IX_Roles_UpdatedBy" ON "Roles" ("UpdatedBy");

CREATE INDEX "IX_Roles_UpdatedOn" ON "Roles" ("UpdatedOn");

CREATE INDEX "IX_Roles_Version" ON "Roles" ("Version");

CREATE UNIQUE INDEX "IX_Sessions_AggregateId" ON "Sessions" ("AggregateId");

CREATE INDEX "IX_Sessions_CreatedBy" ON "Sessions" ("CreatedBy");

CREATE INDEX "IX_Sessions_CreatedOn" ON "Sessions" ("CreatedOn");

CREATE INDEX "IX_Sessions_IsActive" ON "Sessions" ("IsActive");

CREATE INDEX "IX_Sessions_IsPersistent" ON "Sessions" ("IsPersistent");

CREATE INDEX "IX_Sessions_SignedOutOn" ON "Sessions" ("SignedOutOn");

CREATE INDEX "IX_Sessions_UpdatedBy" ON "Sessions" ("UpdatedBy");

CREATE INDEX "IX_Sessions_UpdatedOn" ON "Sessions" ("UpdatedOn");

CREATE INDEX "IX_Sessions_UserId" ON "Sessions" ("UserId");

CREATE INDEX "IX_Sessions_Version" ON "Sessions" ("Version");

CREATE INDEX "IX_UserRoles_RoleId" ON "UserRoles" ("RoleId");

CREATE INDEX "IX_Users_AddressFormatted" ON "Users" ("AddressFormatted");

CREATE UNIQUE INDEX "IX_Users_AggregateId" ON "Users" ("AggregateId");

CREATE INDEX "IX_Users_AuthenticatedOn" ON "Users" ("AuthenticatedOn");

CREATE INDEX "IX_Users_Birthdate" ON "Users" ("Birthdate");

CREATE INDEX "IX_Users_CreatedBy" ON "Users" ("CreatedBy");

CREATE INDEX "IX_Users_CreatedOn" ON "Users" ("CreatedOn");

CREATE INDEX "IX_Users_DisabledOn" ON "Users" ("DisabledOn");

CREATE INDEX "IX_Users_EmailAddress" ON "Users" ("EmailAddress");

CREATE INDEX "IX_Users_FirstName" ON "Users" ("FirstName");

CREATE INDEX "IX_Users_FullName" ON "Users" ("FullName");

CREATE INDEX "IX_Users_Gender" ON "Users" ("Gender");

CREATE INDEX "IX_Users_HasPassword" ON "Users" ("HasPassword");

CREATE INDEX "IX_Users_IsConfirmed" ON "Users" ("IsConfirmed");

CREATE INDEX "IX_Users_IsDisabled" ON "Users" ("IsDisabled");

CREATE INDEX "IX_Users_LastName" ON "Users" ("LastName");

CREATE INDEX "IX_Users_Locale" ON "Users" ("Locale");

CREATE INDEX "IX_Users_MiddleName" ON "Users" ("MiddleName");

CREATE INDEX "IX_Users_Nickname" ON "Users" ("Nickname");

CREATE INDEX "IX_Users_PasswordChangedOn" ON "Users" ("PasswordChangedOn");

CREATE INDEX "IX_Users_PhoneE164Formatted" ON "Users" ("PhoneE164Formatted");

CREATE INDEX "IX_Users_TenantId" ON "Users" ("TenantId");

CREATE INDEX "IX_Users_TenantId_EmailAddressNormalized" ON "Users" ("TenantId", "EmailAddressNormalized");

CREATE UNIQUE INDEX "IX_Users_TenantId_UniqueNameNormalized" ON "Users" ("TenantId", "UniqueNameNormalized");

CREATE INDEX "IX_Users_TimeZone" ON "Users" ("TimeZone");

CREATE INDEX "IX_Users_UniqueName" ON "Users" ("UniqueName");

CREATE INDEX "IX_Users_UpdatedBy" ON "Users" ("UpdatedBy");

CREATE INDEX "IX_Users_UpdatedOn" ON "Users" ("UpdatedOn");

CREATE INDEX "IX_Users_Version" ON "Users" ("Version");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230825031812_InitialMigration', '7.0.10');

COMMIT;
