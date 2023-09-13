START TRANSACTION;

ALTER TABLE "Realms" ADD "PasswordRecoverySenderId" integer NULL;

CREATE UNIQUE INDEX "IX_Realms_PasswordRecoverySenderId" ON "Realms" ("PasswordRecoverySenderId");

ALTER TABLE "Realms" ADD CONSTRAINT "FK_Realms_Senders_PasswordRecoverySenderId" FOREIGN KEY ("PasswordRecoverySenderId") REFERENCES "Senders" ("SenderId") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230913014025_AddPasswordRecoverySender', '7.0.10');

COMMIT;
