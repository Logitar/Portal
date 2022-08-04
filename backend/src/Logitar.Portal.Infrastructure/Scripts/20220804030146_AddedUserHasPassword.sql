START TRANSACTION;

ALTER TABLE "Users" ADD "HasPassword" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220804030146_AddedUserHasPassword', '6.0.7');

COMMIT;
