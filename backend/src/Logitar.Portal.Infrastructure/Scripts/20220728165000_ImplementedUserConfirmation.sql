START TRANSACTION;

ALTER TABLE "Users" RENAME COLUMN "PhoneNumberConfirmed" TO "PhoneNumberConfirmedAt";

ALTER TABLE "Users" RENAME COLUMN "EmailConfirmed" TO "EmailConfirmedAt";

ALTER TABLE "Users" ADD "EmailConfirmedById" uuid NULL;

ALTER TABLE "Users" ADD "PhoneNumberConfirmedById" uuid NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220728165000_ImplementedUserConfirmation', '6.0.7');

COMMIT;
