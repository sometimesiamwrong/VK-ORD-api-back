-- ИСПРАВЛЕННЫЙ скрипт для RowVersion полей с триггерами для PostgreSQL
-- Правильный порядок операций: сначала обновляем NULL значения, потом устанавливаем ограничения

-- 1. Сначала обновляем все NULL и нулевые значения на 1
UPDATE "Users" SET "RowVersion" = 1 WHERE "RowVersion" IS NULL OR "RowVersion" = 0;
UPDATE "ApiCredentials" SET "RowVersion" = 1 WHERE "RowVersion" IS NULL OR "RowVersion" = 0;

-- 2. Теперь можем безопасно изменить тип и установить ограничения
ALTER TABLE "Users" 
    ALTER COLUMN "RowVersion" TYPE integer USING COALESCE("RowVersion", 1),
    ALTER COLUMN "RowVersion" SET DEFAULT 1,
    ALTER COLUMN "RowVersion" SET NOT NULL;

ALTER TABLE "ApiCredentials" 
    ALTER COLUMN "RowVersion" TYPE integer USING COALESCE("RowVersion", 1),
    ALTER COLUMN "RowVersion" SET DEFAULT 1,
    ALTER COLUMN "RowVersion" SET NOT NULL;

-- 3. Создаем функцию для автоматического обновления RowVersion при UPDATE
CREATE OR REPLACE FUNCTION update_row_version()
RETURNS TRIGGER AS $$
BEGIN
    -- Увеличиваем RowVersion при каждом обновлении
    NEW."RowVersion" = COALESCE(OLD."RowVersion", 0) + 1;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 4. Создаем функцию для установки начального значения RowVersion при INSERT
CREATE OR REPLACE FUNCTION set_initial_row_version()
RETURNS TRIGGER AS $$
BEGIN
    -- Устанавливаем начальное значение RowVersion = 1 при создании записи
    IF NEW."RowVersion" IS NULL OR NEW."RowVersion" = 0 THEN
        NEW."RowVersion" = 1;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 5. Удаляем старые триггеры если они существуют
DROP TRIGGER IF EXISTS update_users_row_version ON "Users";
DROP TRIGGER IF EXISTS update_api_credentials_row_version ON "ApiCredentials";
DROP TRIGGER IF EXISTS set_initial_users_row_version ON "Users";
DROP TRIGGER IF EXISTS set_initial_api_credentials_row_version ON "ApiCredentials";

-- 6. Создаем триггеры для INSERT (устанавливаем начальное значение)
CREATE TRIGGER set_initial_users_row_version
    BEFORE INSERT ON "Users"
    FOR EACH ROW
    EXECUTE FUNCTION set_initial_row_version();

CREATE TRIGGER set_initial_api_credentials_row_version
    BEFORE INSERT ON "ApiCredentials"
    FOR EACH ROW
    EXECUTE FUNCTION set_initial_row_version();

-- 7. Создаем триггеры для UPDATE (автоматическое увеличение версии)
CREATE TRIGGER update_users_row_version
    BEFORE UPDATE ON "Users"
    FOR EACH ROW
    EXECUTE FUNCTION update_row_version();

CREATE TRIGGER update_api_credentials_row_version
    BEFORE UPDATE ON "ApiCredentials"
    FOR EACH ROW
    EXECUTE FUNCTION update_row_version();
