﻿START TRANSACTION;

CREATE TABLE "ApiKeys" (
    "ApiKeyId" integer GENERATED BY DEFAULT AS IDENTITY,
    "TenantId" character varying(255) NULL,
    "Secret" character varying(255) NULL,
    "Title" character varying(255) NOT NULL,
    "Description" text NULL,
    "ExpiresOn" timestamp with time zone NULL,
    "AuthenticatedOn" timestamp with time zone NULL,
    "CustomAttributes" text NULL,
    "AggregateId" character varying(255) NOT NULL,
    "CreatedBy" character varying(255) NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255) NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_ApiKeys" PRIMARY KEY ("ApiKeyId")
);

CREATE TABLE "ApiKeyRoles" (
    "ApiKeyId" integer NOT NULL,
    "RoleId" integer NOT NULL,
    CONSTRAINT "PK_ApiKeyRoles" PRIMARY KEY ("ApiKeyId", "RoleId"),
    CONSTRAINT "FK_ApiKeyRoles_ApiKeys_ApiKeyId" FOREIGN KEY ("ApiKeyId") REFERENCES "ApiKeys" ("ApiKeyId") ON DELETE CASCADE,
    CONSTRAINT "FK_ApiKeyRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "Roles" ("RoleId") ON DELETE CASCADE
);

CREATE INDEX "IX_ApiKeyRoles_RoleId" ON "ApiKeyRoles" ("RoleId");

CREATE UNIQUE INDEX "IX_ApiKeys_AggregateId" ON "ApiKeys" ("AggregateId");

CREATE INDEX "IX_ApiKeys_AuthenticatedOn" ON "ApiKeys" ("AuthenticatedOn");

CREATE INDEX "IX_ApiKeys_CreatedBy" ON "ApiKeys" ("CreatedBy");

CREATE INDEX "IX_ApiKeys_CreatedOn" ON "ApiKeys" ("CreatedOn");

CREATE INDEX "IX_ApiKeys_ExpiresOn" ON "ApiKeys" ("ExpiresOn");

CREATE INDEX "IX_ApiKeys_TenantId" ON "ApiKeys" ("TenantId");

CREATE INDEX "IX_ApiKeys_Title" ON "ApiKeys" ("Title");

CREATE INDEX "IX_ApiKeys_UpdatedBy" ON "ApiKeys" ("UpdatedBy");

CREATE INDEX "IX_ApiKeys_UpdatedOn" ON "ApiKeys" ("UpdatedOn");

CREATE INDEX "IX_ApiKeys_Version" ON "ApiKeys" ("Version");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230901034942_CreateApiKeyTables', '7.0.10');

COMMIT;
