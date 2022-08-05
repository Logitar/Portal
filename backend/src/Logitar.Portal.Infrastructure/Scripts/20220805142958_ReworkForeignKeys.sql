START TRANSACTION;

ALTER TABLE "ExternalProvider" DROP CONSTRAINT "FK_ExternalProvider_Users_UserSid";

ALTER TABLE "Sessions" DROP CONSTRAINT "FK_Sessions_Users_UserSid";

ALTER TABLE "ExternalProvider" DROP CONSTRAINT "PK_ExternalProvider";

ALTER TABLE "ExternalProvider" RENAME TO "ExternalProviders";

ALTER INDEX "IX_ExternalProvider_UserSid" RENAME TO "IX_ExternalProviders_UserSid";

ALTER INDEX "IX_ExternalProvider_Key_Value" RENAME TO "IX_ExternalProviders_Key_Value";

ALTER INDEX "IX_ExternalProvider_Id" RENAME TO "IX_ExternalProviders_Id";

ALTER TABLE "ExternalProviders" ADD CONSTRAINT "PK_ExternalProviders" PRIMARY KEY ("Sid");

ALTER TABLE "ExternalProviders" ADD CONSTRAINT "FK_ExternalProviders_Users_UserSid" FOREIGN KEY ("UserSid") REFERENCES "Users" ("Sid") ON DELETE CASCADE;

ALTER TABLE "Sessions" ADD CONSTRAINT "FK_Sessions_Users_UserSid" FOREIGN KEY ("UserSid") REFERENCES "Users" ("Sid");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220805142958_ReworkForeignKeys', '6.0.7');

COMMIT;
