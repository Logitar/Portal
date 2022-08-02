START TRANSACTION;

ALTER TABLE "Templates" ADD "Subject" character varying(256) NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220729134405_AddedTemplateSubject', '6.0.7');

COMMIT;
