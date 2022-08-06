START TRANSACTION;

DROP INDEX "IX_ExternalProviders_Key_Value";

ALTER TABLE "ExternalProviders" ALTER COLUMN "UserSid" SET NOT NULL;
ALTER TABLE "ExternalProviders" ALTER COLUMN "UserSid" SET DEFAULT 0;

ALTER TABLE "ExternalProviders" ADD "RealmSid" integer NOT NULL DEFAULT 0;

CREATE UNIQUE INDEX "IX_ExternalProviders_RealmSid_Key_Value" ON "ExternalProviders" ("RealmSid", "Key", "Value");

ALTER TABLE "ExternalProviders" ADD CONSTRAINT "FK_ExternalProviders_Realms_RealmSid" FOREIGN KEY ("RealmSid") REFERENCES "Realms" ("Sid") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220806141123_RefactoredExternalProviders', '6.0.7');

COMMIT;
