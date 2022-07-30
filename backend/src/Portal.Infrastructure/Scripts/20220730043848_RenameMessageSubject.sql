START TRANSACTION;

ALTER TABLE "Messages" RENAME COLUMN "TemplateSubject" TO "Subject";

ALTER INDEX "IX_Messages_TemplateSubject" RENAME TO "IX_Messages_Subject";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220730043848_RenameMessageSubject', '6.0.7');

COMMIT;
