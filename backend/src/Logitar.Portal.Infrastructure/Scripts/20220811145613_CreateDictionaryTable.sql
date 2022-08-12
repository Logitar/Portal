﻿START TRANSACTION;

CREATE TABLE "Dictionaries" (
    "Sid" integer GENERATED BY DEFAULT AS IDENTITY,
    "RealmSid" integer NULL,
    "Locale" character varying(16) NOT NULL,
    "Entries" jsonb NULL,
    "Id" uuid NOT NULL DEFAULT (uuid_generate_v4()),
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now()),
    "CreatedById" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
    "UpdatedAt" timestamp with time zone NULL,
    "UpdatedById" uuid NULL,
    "Version" integer NOT NULL DEFAULT 0,
    CONSTRAINT "PK_Dictionaries" PRIMARY KEY ("Sid"),
    CONSTRAINT "FK_Dictionaries_Realms_RealmSid" FOREIGN KEY ("RealmSid") REFERENCES "Realms" ("Sid")
);

CREATE UNIQUE INDEX "IX_Dictionaries_Id" ON "Dictionaries" ("Id");

CREATE UNIQUE INDEX "IX_Dictionaries_RealmSid_Locale" ON "Dictionaries" ("RealmSid", "Locale");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220811145613_CreateDictionaryTable', '6.0.7');

COMMIT;
