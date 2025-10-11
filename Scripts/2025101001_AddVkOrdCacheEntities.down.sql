START TRANSACTION;


DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdContractParty";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdCounterpartyRelation";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdCreativeContract";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdCreativeMedia";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdStatisticsCache";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdCounterpartyCache";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdContractCache";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdMediaCache";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DROP TABLE "VkOrdCreativeCache";
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities') THEN
    DELETE FROM "__EFMigrationsHistory"
    WHERE "MigrationId" = '20251010165809_AddVkOrdCacheEntities';
    END IF;
END $EF$;
COMMIT;

