START TRANSACTION;

ALTER TABLE "Realms" ADD "Url" character varying(2048) NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220730171429_AddedRealmAudience', '6.0.7');

COMMIT;
