START TRANSACTION;

CREATE TABLE "RealmPasswordRecoverySenders" (
    "RealmSid" integer NOT NULL,
    "SenderSid" integer NOT NULL,
    CONSTRAINT "PK_RealmPasswordRecoverySenders" PRIMARY KEY ("RealmSid"),
    CONSTRAINT "FK_RealmPasswordRecoverySenders_Realms_RealmSid" FOREIGN KEY ("RealmSid") REFERENCES "Realms" ("Sid") ON DELETE CASCADE,
    CONSTRAINT "FK_RealmPasswordRecoverySenders_Senders_SenderSid" FOREIGN KEY ("SenderSid") REFERENCES "Senders" ("Sid") ON DELETE CASCADE
);

CREATE TABLE "RealmPasswordRecoveryTemplates" (
    "RealmSid" integer NOT NULL,
    "TemplateSid" integer NOT NULL,
    CONSTRAINT "PK_RealmPasswordRecoveryTemplates" PRIMARY KEY ("RealmSid"),
    CONSTRAINT "FK_RealmPasswordRecoveryTemplates_Realms_RealmSid" FOREIGN KEY ("RealmSid") REFERENCES "Realms" ("Sid") ON DELETE CASCADE,
    CONSTRAINT "FK_RealmPasswordRecoveryTemplates_Templates_TemplateSid" FOREIGN KEY ("TemplateSid") REFERENCES "Templates" ("Sid") ON DELETE CASCADE
);

CREATE INDEX "IX_RealmPasswordRecoverySenders_SenderSid" ON "RealmPasswordRecoverySenders" ("SenderSid");

CREATE INDEX "IX_RealmPasswordRecoveryTemplates_TemplateSid" ON "RealmPasswordRecoveryTemplates" ("TemplateSid");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220730205535_AddedRealmPasswordRecoverySettings', '6.0.7');

COMMIT;
