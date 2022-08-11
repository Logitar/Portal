START TRANSACTION;

ALTER TABLE "Realms" ADD "DefaultLocale" character varying(16) NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220811140321_AddedRealmDefaultLocale', '6.0.7');

COMMIT;
