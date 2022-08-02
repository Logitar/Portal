START TRANSACTION;

ALTER TABLE "Templates" ADD "ContentType" character varying(256) NOT NULL DEFAULT 'text/plain';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220729055833_AddTemplateContentType', '6.0.7');

COMMIT;
