START TRANSACTION;

ALTER TABLE "Realms" ADD "GoogleClientId" character varying(256) NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220803201449_AddedRealmGoogleClientId', '6.0.7');

COMMIT;
