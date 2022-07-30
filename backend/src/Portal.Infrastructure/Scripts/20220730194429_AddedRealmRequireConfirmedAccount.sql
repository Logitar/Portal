START TRANSACTION;

ALTER TABLE "Realms" ADD "RequireConfirmedAccount" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220730194429_AddedRealmRequireConfirmedAccount', '6.0.7');

COMMIT;
