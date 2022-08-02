START TRANSACTION;

ALTER TABLE "Users" ADD "DisabledAt" timestamp with time zone NULL;

ALTER TABLE "Users" ADD "DisabledById" uuid NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220731042022_AddedDisabledUser', '6.0.7');

COMMIT;
