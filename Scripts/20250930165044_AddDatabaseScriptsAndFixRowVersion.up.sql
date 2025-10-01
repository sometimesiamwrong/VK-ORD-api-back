START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930165044_AddDatabaseScriptsAndFixRowVersion') THEN
    ALTER TABLE "Users" DROP COLUMN IF EXISTS "RowVersion";
    ALTER TABLE "Users" ADD COLUMN "RowVersion" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930165044_AddDatabaseScriptsAndFixRowVersion') THEN
    ALTER TABLE "ApiCredentials" DROP COLUMN IF EXISTS "RowVersion";
    ALTER TABLE "ApiCredentials" ADD COLUMN "RowVersion" integer;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930165044_AddDatabaseScriptsAndFixRowVersion') THEN
    CREATE TABLE "DatabaseScripts" (
        "Id" uuid NOT NULL,
        "ScriptName" character varying(255) NOT NULL,
        "ScriptHash" character varying(64) NOT NULL,
        "ExecutedAt" timestamp with time zone NOT NULL,
        "Description" character varying(500),
        "IsSuccessful" boolean NOT NULL,
        "ErrorMessage" character varying(2000),
        CONSTRAINT "PK_DatabaseScripts" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930165044_AddDatabaseScriptsAndFixRowVersion') THEN
    CREATE UNIQUE INDEX "IX_DatabaseScripts_ScriptName" ON "DatabaseScripts" ("ScriptName");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250930165044_AddDatabaseScriptsAndFixRowVersion') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250930165044_AddDatabaseScriptsAndFixRowVersion', '8.0.8');
    END IF;
END $EF$;
COMMIT;

