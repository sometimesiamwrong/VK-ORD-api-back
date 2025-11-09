# Устранение проблемы с Hangfire Distributed Lock Timeout

## Проблема
```
Hangfire.PostgreSql.PostgreSqlDistributedLockException: Could not place a lock on the resource 'hangfire:lock:recurring-job:sync-erir-statuses': Lock timeout.
```

## Причины
1. **Предыдущий экземпляр приложения не завершился корректно** - блокировка не была освобождена
2. **Другой экземпляр Jobs приложения уже запущен** - конфликт блокировок
3. **Таймаут блокировки слишком короткий** - операция требует больше времени

## Решения

### 1. Автоматическое решение (уже реализовано)
В коде добавлены следующие улучшения:
- ✅ Увеличен `DistributedLockTimeout` до 1 минуты
- ✅ Автоматические повторные попытки (3 попытки с задержкой 5 секунд)
- ✅ Graceful degradation - приложение продолжит работу даже если не удалось зарегистрировать recurring job
- ✅ Подробное логирование для диагностики

### 2. Ручная очистка зависших блокировок

#### Вариант A: Через SQL (рекомендуется)
```bash
# 1. Подключитесь к базе данных
psql -U postgres -d adlawyer

# 2. Выполните скрипт очистки
\i src/Jobs/Scripts/clear-hangfire-locks.sql
```

Или напрямую:
```sql
-- Очистка блокировок старше 5 минут
DELETE FROM hangfire.lock 
WHERE acquired < NOW() - INTERVAL '5 minutes';
```

#### Вариант B: Перезапуск всех экземпляров
```bash
# 1. Остановите все экземпляры Jobs
docker-compose stop jobs
# или
pkill -f Jobs

# 2. Подождите 30 секунд

# 3. Запустите заново
docker-compose up -d jobs
```

### 3. Проверка состояния блокировок

```sql
-- Посмотреть все текущие блокировки
SELECT 
    resource,
    acquired,
    NOW() - acquired as age
FROM hangfire.lock 
ORDER BY acquired DESC;

-- Проверить конкретную блокировку
SELECT * FROM hangfire.lock 
WHERE resource LIKE '%sync-erir-statuses%';
```

### 4. Предотвращение проблемы в будущем

#### Docker Compose
Убедитесь, что в `docker-compose.yml` настроен корректный shutdown:
```yaml
jobs:
  stop_grace_period: 30s
  stop_signal: SIGTERM
```

#### Systemd Service
```ini
[Service]
TimeoutStopSec=30
KillMode=mixed
KillSignal=SIGTERM
```

## Мониторинг

### Логи
Приложение теперь логирует попытки регистрации recurring jobs:
```
[INF] Attempting to register recurring jobs (attempt 1/3)
[WRN] Failed to acquire distributed lock for recurring job registration (attempt 1/3). Retrying in 5000ms...
[INF] Successfully configured ERIR sync job to run every X minutes
```

### Hangfire Dashboard
Доступен по адресу: `http://localhost:5000/hangfire`
- Проверяйте раздел "Servers" - должен быть только один активный сервер
- Раздел "Recurring Jobs" - должна быть зарегистрирована задача `sync-erir-statuses`

## Диагностика

### 1. Проверить количество запущенных экземпляров
```bash
# Linux
ps aux | grep Jobs

# Windows
tasklist | findstr Jobs

# Docker
docker ps | grep jobs
```

### 2. Проверить Hangfire серверы в БД
```sql
SELECT 
    id,
    data,
    lastheartbeat,
    NOW() - lastheartbeat as heartbeat_age
FROM hangfire.server
ORDER BY lastheartbeat DESC;
```

### 3. Проверить логи приложения
```bash
# Docker
docker logs adlawyerapi-jobs-1 --tail 100

# Файловые логи
tail -f src/Jobs/logs/log-*.txt
```

## Конфигурация

Текущие настройки Hangfire (в `Program.cs`):
```csharp
DistributedLockTimeout = TimeSpan.FromMinutes(1)      // Таймаут блокировки
InvisibilityTimeout = TimeSpan.FromMinutes(30)        // Время невидимости задачи
QueuePollInterval = TimeSpan.FromSeconds(15)          // Интервал опроса очереди
ServerTimeout = TimeSpan.FromMinutes(5)               // Таймаут сервера
ShutdownTimeout = TimeSpan.FromSeconds(30)            // Таймаут остановки
```

## Контакты
При возникновении проблем проверьте:
1. Логи приложения
2. Состояние блокировок в БД
3. Количество запущенных экземпляров
4. Hangfire Dashboard

