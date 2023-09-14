START TRANSACTION;

ALTER TABLE "Dictionaries" ADD "EntryCount" integer NOT NULL DEFAULT 0;

CREATE INDEX "IX_Dictionaries_EntryCount" ON "Dictionaries" ("EntryCount");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230914011248_AddDictionaryEntryCount', '7.0.10');

COMMIT;
