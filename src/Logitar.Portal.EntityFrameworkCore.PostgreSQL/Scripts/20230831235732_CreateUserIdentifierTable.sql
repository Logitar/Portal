﻿START TRANSACTION;

CREATE TABLE "UserIdentifiers" (
    "UserIdentifierId" integer GENERATED BY DEFAULT AS IDENTITY,
    "UserId" integer NOT NULL,
    "Id" uuid NOT NULL,
    "TenantId" character varying(255) NULL,
    "Key" character varying(255) NOT NULL,
    "Value" character varying(255) NOT NULL,
    "ValueNormalized" character varying(255) NOT NULL,
    "CreatedBy" character varying(255) NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255) NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_UserIdentifiers" PRIMARY KEY ("UserIdentifierId"),
    CONSTRAINT "FK_UserIdentifiers_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_UserIdentifiers_Id" ON "UserIdentifiers" ("Id");

CREATE UNIQUE INDEX "IX_UserIdentifiers_TenantId_Key_Value" ON "UserIdentifiers" ("TenantId", "Key", "Value");

CREATE INDEX "IX_UserIdentifiers_UserId" ON "UserIdentifiers" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230831235732_CreateUserIdentifierTable', '7.0.10');

COMMIT;