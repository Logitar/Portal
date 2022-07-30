START TRANSACTION;

ALTER TABLE "Messages" ADD "IsDemo" boolean NOT NULL DEFAULT FALSE;

CREATE INDEX "IX_Messages_IsDemo" ON "Messages" ("IsDemo");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220730151846_AddedMessageIsDemo', '6.0.7');

COMMIT;
