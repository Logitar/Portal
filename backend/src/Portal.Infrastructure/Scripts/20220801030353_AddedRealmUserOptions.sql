START TRANSACTION;

ALTER TABLE "Realms" ALTER COLUMN "RequireConfirmedAccount" SET DEFAULT FALSE;

ALTER TABLE "Realms" ADD "AllowedUsernameCharacters" text NULL;

ALTER TABLE "Realms" ADD "RequireUniqueEmail" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220801030353_AddedRealmUserOptions', '6.0.7');

COMMIT;
