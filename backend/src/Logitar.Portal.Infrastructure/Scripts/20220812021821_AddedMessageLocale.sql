START TRANSACTION;

ALTER TABLE "Messages" ADD "IgnoreUserLocale" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Messages" ADD "Locale" character varying(16) NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220812021821_AddedMessageLocale', '6.0.7');

COMMIT;
