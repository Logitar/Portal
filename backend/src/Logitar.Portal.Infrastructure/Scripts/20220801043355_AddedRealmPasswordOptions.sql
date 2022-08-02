START TRANSACTION;

ALTER TABLE "Realms" ADD "PasswordSettings" jsonb NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220801043355_AddedRealmPasswordOptions', '6.0.7');

COMMIT;
