START TRANSACTION;

ALTER TABLE "Messages" DROP CONSTRAINT "FK_Messages_Realms_RealmId";

ALTER TABLE "Messages" DROP COLUMN "RealmSummary";

CREATE INDEX "IX_Messages_IsDemo" ON "Messages" ("IsDemo");

CREATE INDEX "IX_Messages_RecipientCount" ON "Messages" ("RecipientCount");

CREATE INDEX "IX_Messages_Status" ON "Messages" ("Status");

CREATE INDEX "IX_Messages_Subject" ON "Messages" ("Subject");

ALTER TABLE "Messages" ADD CONSTRAINT "FK_Messages_Realms_RealmId" FOREIGN KEY ("RealmId") REFERENCES "Realms" ("RealmId") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230915224615_CompleteMessageTables', '7.0.10');

COMMIT;
