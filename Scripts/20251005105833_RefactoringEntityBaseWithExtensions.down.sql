START TRANSACTION;

DELETE FROM "ApiCredentials";
DELETE FROM "RefreshTokens";
DELETE FROM "Users";

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    DROP INDEX "IX_Users_PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    DROP INDEX "IX_RefreshTokens_PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    DROP INDEX "IX_DatabaseScripts_PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    DROP INDEX "IX_ApiCredentials_PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "Users" DROP COLUMN "IsDeleted";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "Users" DROP COLUMN "PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "RefreshTokens" DROP COLUMN "IsDeleted";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "RefreshTokens" DROP COLUMN "PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "RefreshTokens" DROP COLUMN "RowVersion";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "RefreshTokens" DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "DatabaseScripts" DROP COLUMN "CreatedAt";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "DatabaseScripts" DROP COLUMN "IsDeleted";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "DatabaseScripts" DROP COLUMN "PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "DatabaseScripts" DROP COLUMN "RowVersion";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "DatabaseScripts" DROP COLUMN "UpdatedAt";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" DROP COLUMN "IsDeleted";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" DROP COLUMN "PublicId";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "Users" ALTER COLUMN "RowVersion" DROP DEFAULT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" DROP CONSTRAINT "FK_ApiCredentials_Users_UserId";
    ALTER TABLE "RefreshTokens" DROP CONSTRAINT "FK_RefreshTokens_Users_UserId";
    ALTER TABLE "Users" ALTER COLUMN "Id" TYPE uuid USING "Id"::uuid;
    ALTER TABLE "Users" ALTER COLUMN "Id" DROP IDENTITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "RefreshTokens" DROP COLUMN "UserId";
    ALTER TABLE "RefreshTokens" ADD COLUMN "UserId" uuid NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "RefreshTokens" ALTER COLUMN "Id" TYPE uuid USING "Id"::uuid;
    ALTER TABLE "RefreshTokens" ALTER COLUMN "Id" DROP IDENTITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "DatabaseScripts" ALTER COLUMN "ErrorMessage" TYPE character varying(2000);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "DatabaseScripts" ALTER COLUMN "Id" TYPE uuid USING "Id"::uuid;
    ALTER TABLE "DatabaseScripts" ALTER COLUMN "Id" DROP IDENTITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" DROP COLUMN "UserId";
    ALTER TABLE "ApiCredentials" ADD COLUMN "UserId" uuid NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" ALTER COLUMN "RowVersion" DROP DEFAULT;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" ALTER COLUMN "Environment" TYPE character varying(20) USING "Environment"::character varying(20);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" ALTER COLUMN "Id" TYPE uuid USING "Id"::uuid;
    ALTER TABLE "ApiCredentials" ALTER COLUMN "Id" DROP IDENTITY;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions') THEN
    ALTER TABLE "ApiCredentials" ADD CONSTRAINT "FK_ApiCredentials_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    ALTER TABLE "RefreshTokens" ADD CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251005110620_RefactoringEntityBaseWithExtensions';
    END IF;
END $EF$;
COMMIT;

