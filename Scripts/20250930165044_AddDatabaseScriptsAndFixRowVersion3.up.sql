-- Создание дополнительных индексов для оптимизации производительности

-- Индексы для таблицы Users
-- Поиск пользователей по дате создания (для пагинации)
CREATE INDEX IF NOT EXISTS "IX_Users_CreatedAt" ON "Users" ("CreatedAt");

-- Поиск активных пользователей
CREATE INDEX IF NOT EXISTS "IX_Users_IsActive_CreatedAt" ON "Users" ("IsActive", "CreatedAt")
WHERE "IsActive" = true;

-- Индексы для таблицы ApiCredentials
-- Поиск API Credentials по пользователю и окружению (составной индекс)
CREATE INDEX IF NOT EXISTS "IX_ApiCredentials_UserId_Environment" 
ON "ApiCredentials" ("UserId", "Environment");

-- Поиск ApiCredentials по дате обновления
CREATE INDEX IF NOT EXISTS "IX_ApiCredentials_UpdatedAt" 
ON "ApiCredentials" ("UpdatedAt");

-- Индексы для таблицы RefreshTokens
-- Поиск токенов по пользователю и дате истечения (для очистки просроченных)
CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_UserId_ExpiresAt" 
ON "RefreshTokens" ("UserId", "ExpiresAt");

-- Поиск активных токенов (не отозванных и не истекших)
CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_Active" 
ON "RefreshTokens" ("ExpiresAt", "RevokedAt") 
WHERE "RevokedAt" IS NULL;

-- Поиск по хешу токена (для быстрой валидации)
CREATE INDEX IF NOT EXISTS "IX_RefreshTokens_TokenHash" 
ON "RefreshTokens" ("TokenHash")
WHERE "RevokedAt" IS NULL;

-- Индексы для таблицы DatabaseScripts
-- Поиск скриптов по статусу выполнения
CREATE INDEX IF NOT EXISTS "IX_DatabaseScripts_IsSuccessful_ExecutedAt" 
ON "DatabaseScripts" ("IsSuccessful", "ExecutedAt");

-- Поиск неуспешных скриптов
CREATE INDEX IF NOT EXISTS "IX_DatabaseScripts_Failed" 
ON "DatabaseScripts" ("ExecutedAt") 
WHERE "IsSuccessful" = false;

-- Комментарии к таблицам для документации
COMMENT ON TABLE "Users" IS 'Пользователи системы с JWT-аутентификацией';
COMMENT ON TABLE "ApiCredentials" IS 'Зашифрованные API-ключи пользователей для VK ORD';
COMMENT ON TABLE "RefreshTokens" IS 'Refresh токены для JWT-аутентификации с ротацией';
COMMENT ON TABLE "DatabaseScripts" IS 'Лог выполненных SQL-скриптов миграций и обновлений';

-- Комментарии к важным колонкам
COMMENT ON COLUMN "Users"."RowVersion" IS 'Версия записи для optimistic concurrency control';
COMMENT ON COLUMN "ApiCredentials"."RowVersion" IS 'Версия записи для optimistic concurrency control';
COMMENT ON COLUMN "RefreshTokens"."TokenHash" IS 'SHA256 хеш refresh токена';
COMMENT ON COLUMN "DatabaseScripts"."ScriptHash" IS 'SHA256 хеш содержимого SQL-скрипта';
