START TRANSACTION;

ALTER TABLE "Users" ADD "IsAccountConfirmed" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Users" ADD "IsDisabled" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Users" ADD "IsEmailConfirmed" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Users" ADD "IsPhoneNumberConfirmed" boolean NOT NULL DEFAULT FALSE;

CREATE INDEX "IX_Users_IsAccountConfirmed" ON "Users" ("IsAccountConfirmed");

CREATE INDEX "IX_Users_IsDisabled" ON "Users" ("IsDisabled");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220731050701_AddedUserBooleanColumns', '6.0.7');

COMMIT;
