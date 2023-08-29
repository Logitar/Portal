START TRANSACTION;

CREATE TABLE "Logs" (
    "LogId" bigint GENERATED BY DEFAULT AS IDENTITY,
    "Id" uuid NOT NULL,
    "CorrelationId" character varying(255) NULL,
    "Method" character varying(255) NULL,
    "Destination" character varying(2048) NULL,
    "Source" character varying(2048) NULL,
    "AdditionalInformation" text NULL,
    "OperationType" character varying(255) NULL,
    "OperationName" character varying(255) NULL,
    "ActivityType" character varying(255) NULL,
    "ActivityData" text NULL,
    "StatusCode" integer NULL,
    "StartedOn" timestamp with time zone NOT NULL,
    "EndedOn" timestamp with time zone NULL,
    "Duration" interval NULL,
    "ActorId" uuid NOT NULL,
    "UserId" uuid NULL,
    "SessionId" uuid NULL,
    "IsCompleted" boolean NOT NULL,
    "Level" character varying(255) NOT NULL,
    "HasErrors" boolean NOT NULL,
    "Errors" text NULL,
    CONSTRAINT "PK_Logs" PRIMARY KEY ("LogId")
);

CREATE TABLE "LogEvents" (
    "LogId" bigint NOT NULL,
    "EventId" bigint NOT NULL,
    CONSTRAINT "PK_LogEvents" PRIMARY KEY ("LogId", "EventId"),
    CONSTRAINT "FK_LogEvents_Logs_LogId" FOREIGN KEY ("LogId") REFERENCES "Logs" ("LogId") ON DELETE CASCADE
);

CREATE INDEX "IX_Logs_ActivityType" ON "Logs" ("ActivityType");

CREATE INDEX "IX_Logs_ActorId" ON "Logs" ("ActorId");

CREATE INDEX "IX_Logs_CorrelationId" ON "Logs" ("CorrelationId");

CREATE INDEX "IX_Logs_Duration" ON "Logs" ("Duration");

CREATE INDEX "IX_Logs_EndedOn" ON "Logs" ("EndedOn");

CREATE INDEX "IX_Logs_HasErrors" ON "Logs" ("HasErrors");

CREATE UNIQUE INDEX "IX_Logs_Id" ON "Logs" ("Id");

CREATE INDEX "IX_Logs_IsCompleted" ON "Logs" ("IsCompleted");

CREATE INDEX "IX_Logs_Level" ON "Logs" ("Level");

CREATE INDEX "IX_Logs_Method" ON "Logs" ("Method");

CREATE INDEX "IX_Logs_OperationName" ON "Logs" ("OperationName");

CREATE INDEX "IX_Logs_OperationType" ON "Logs" ("OperationType");

CREATE INDEX "IX_Logs_SessionId" ON "Logs" ("SessionId");

CREATE INDEX "IX_Logs_StartedOn" ON "Logs" ("StartedOn");

CREATE INDEX "IX_Logs_StatusCode" ON "Logs" ("StatusCode");

CREATE INDEX "IX_Logs_UserId" ON "Logs" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20230825160151_CreateLoggingTables', '7.0.10');

COMMIT;