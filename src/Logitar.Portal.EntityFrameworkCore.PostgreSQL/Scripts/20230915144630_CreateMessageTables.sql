START TRANSACTION;

CREATE TABLE "Messages" (
    "MessageId" integer GENERATED BY DEFAULT AS IDENTITY,
    "Subject" character varying(255) NOT NULL,
    "Body" text NOT NULL,
    "RecipientCount" integer NOT NULL,
    "RealmId" integer NULL,
    "RealmSummary" text NULL,
    "SenderId" integer NULL,
    "SenderSummary" text NOT NULL,
    "TemplateId" integer NULL,
    "TemplateSummary" text NOT NULL,
    "IgnoreUserLocale" boolean NOT NULL,
    "Locale" character varying(16) NULL,
    "Variables" text NULL,
    "IsDemo" boolean NOT NULL,
    "Result" text NULL,
    "Status" character varying(255) NOT NULL,
    "AggregateId" character varying(255) NOT NULL,
    "CreatedBy" character varying(255) NOT NULL,
    "CreatedOn" timestamp with time zone NOT NULL,
    "UpdatedBy" character varying(255) NOT NULL,
    "UpdatedOn" timestamp with time zone NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_Messages" PRIMARY KEY ("MessageId"),
    CONSTRAINT "FK_Messages_Realms_RealmId" FOREIGN KEY ("RealmId") REFERENCES "Realms" ("RealmId") ON DELETE SET NULL,
    CONSTRAINT "FK_Messages_Senders_SenderId" FOREIGN KEY ("SenderId") REFERENCES "Senders" ("SenderId") ON DELETE SET NULL,
    CONSTRAINT "FK_Messages_Templates_TemplateId" FOREIGN KEY ("TemplateId") REFERENCES "Templates" ("TemplateId") ON DELETE SET NULL
);

CREATE TABLE "Recipients" (
    "RecipientId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "MessageId" integer NOT NULL,
    "Type" character varying(255) NOT NULL,
    "Address" character varying(255) NOT NULL,
    "DisplayName" character varying(255) NULL,
    "UserId" integer NULL,
    "UserSummary" text NULL,
    CONSTRAINT "PK_Recipients" PRIMARY KEY ("RecipientId"),
    CONSTRAINT "FK_Recipients_Messages_MessageId" FOREIGN KEY ("MessageId") REFERENCES "Messages" ("MessageId") ON DELETE CASCADE,
    CONSTRAINT "FK_Recipients_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE SET NULL
);

CREATE UNIQUE INDEX "IX_Messages_AggregateId" ON "Messages" ("AggregateId");

CREATE INDEX "IX_Messages_CreatedBy" ON "Messages" ("CreatedBy");

CREATE INDEX "IX_Messages_CreatedOn" ON "Messages" ("CreatedOn");

CREATE INDEX "IX_Messages_RealmId" ON "Messages" ("RealmId");

CREATE INDEX "IX_Messages_SenderId" ON "Messages" ("SenderId");

CREATE INDEX "IX_Messages_TemplateId" ON "Messages" ("TemplateId");

CREATE INDEX "IX_Messages_UpdatedBy" ON "Messages" ("UpdatedBy");

CREATE INDEX "IX_Messages_UpdatedOn" ON "Messages" ("UpdatedOn");

CREATE INDEX "IX_Messages_Version" ON "Messages" ("Version");

CREATE INDEX "IX_Recipients_MessageId" ON "Recipients" ("MessageId");

CREATE INDEX "IX_Recipients_UserId" ON "Recipients" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230915144630_CreateMessageTables', '7.0.10');

COMMIT;
