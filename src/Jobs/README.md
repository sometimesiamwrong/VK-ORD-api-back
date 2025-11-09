# VK ORD Background Jobs

Фоновые задачи для синхронизации данных из VK ORД API с использованием Hangfire.

## Компоненты

- **Hangfire** - библиотека для фоновых задач
- **PostgreSQL** - хранилище для Hangfire (общая БД с основным приложением)
- **Redis** - кэширование (опционально)

## Recurring Jobs

### sync-erir-statuses
Синхронизирует статусы ЕРИР для всех логических аккаунтов:
- **Расписание**: Каждые N минут (настраивается в `appsettings.json`)
- **Функции**:
  - Получает список всех логических аккаунтов
  - Для каждого аккаунта синхронизирует статусы контрагентов, договоров, креативов, счетов и статистики
  - Использует AsyncLocal для передачи credentials в репозитории

## Запуск

### Локально (для разработки)
```bash
cd src/Jobs
dotnet run
```

Hangfire Dashboard будет доступен по адресу: `http://localhost:5002/hangfire`

### Docker
```bash
# Сборка и запуск
docker-compose up -d jobs

# Просмотр логов
docker-compose logs -f jobs

# Перезапуск
docker-compose restart jobs

# Остановка
docker-compose stop jobs
```

## Конфигурация

### appsettings.json
```json
{
  "Jobs": {
    "ErirSyncIntervalMinutes": 30
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=vkord;Username=vkord_user;Password=..."
  }
}
```

### Переменные окружения (Docker)
- `ConnectionStrings__DefaultConnection` - строка подключения к PostgreSQL
- `RedisSettings__ConnectionString` - строка подключения к Redis
- `ASPNETCORE_ENVIRONMENT` - окружение (Development/Production)

## Устранение проблем

### PostgreSqlDistributedLockException: Lock timeout

**Проблема**: Не удается получить блокировку для recurring job.

**Быстрое решение**:
```sql
-- Подключитесь к PostgreSQL
psql -U vkord_user -d vkord

-- Очистите зависшие блокировки
DELETE FROM hangfire.lock 
WHERE acquired < NOW() - INTERVAL '5 minutes';
```

**Подробнее**: см. [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

### Несколько экземпляров запущены одновременно

Hangfire использует distributed locks для предотвращения одновременного выполнения задач.
Убедитесь, что запущен только один экземпляр Jobs:

```bash
# Docker
docker ps | grep jobs

# Процессы
ps aux | grep Jobs  # Linux
tasklist | findstr Jobs  # Windows
```

## Мониторинг

### Hangfire Dashboard
- URL: `http://localhost:5002/hangfire`
- Показывает:
  - Активные серверы
  - Recurring jobs и их расписание
  - Историю выполнения задач
  - Очереди задач
  - Статистику

### Логи
Логи сохраняются в:
- Docker: `/app/logs` (mapped to volume `jobs_logs`)
- Локально: `src/Jobs/logs/`

Формат: `log-{Date}.txt`

### Метрики
Hangfire автоматически собирает метрики:
- Количество успешных/неудачных задач
- Время выполнения
- Количество повторных попыток

## Разработка

### Добавление новой задачи

1. Создайте класс в `Jobs/Jobs/`:
```csharp
public class MyNewJob
{
    public async Task Execute()
    {
        // Ваш код
    }
}
```

2. Зарегистрируйте в DI (`Program.cs`):
```csharp
builder.Services.AddScoped<MyNewJob>();
```

3. Добавьте recurring job:
```csharp
RecurringJob.AddOrUpdate<MyNewJob>(
    "my-new-job",
    job => job.Execute(),
    Cron.Hourly());
```

### Тестирование
```bash
# Запустите приложение
dotnet run

# Откройте Hangfire Dashboard
# Перейдите в раздел "Recurring Jobs"
# Нажмите "Trigger now" для ручного запуска
```

## Производительность

### Текущие настройки
- **WorkerCount**: 1 - предотвращает конфликты конкурентного доступа
- **ServerTimeout**: 5 минут
- **DistributedLockTimeout**: 1 минута
- **QueuePollInterval**: 15 секунд

### Рекомендации
- Для Production можно увеличить WorkerCount до 2-3
- Используйте Redis для кэширования данных VK ORД API
- Мониторьте время выполнения через Hangfire Dashboard

## Безопасность

- ✅ Credentials хранятся зашифрованными в БД
- ✅ AsyncLocal обеспечивает изоляцию между задачами
- ✅ Hangfire Dashboard доступен только локально (для Production настройте авторизацию)

## Архитектура

```
Jobs Application
├── Program.cs                  # Конфигурация и регистрация jobs
├── Jobs/
│   └── SyncErirStatusesJob.cs # Задача синхронизации
├── Services/
│   ├── IErirStatusSyncService.cs
│   └── ErirStatusSyncService.cs
└── Configuration/
    └── JobsConfiguration.cs
```

### Взаимодействие с WebApp
Jobs использует:
- Репозитории из `WebApp.Repositories`
- Сервисы из `WebApp.Services`
- Domain модели из `Domain.Entities`

### AsyncLocal Pattern
```csharp
// В Job
using var credentialContext = _apiClientFactory.SetCredentialContext(credential);

// Все вызовы репозиториев автоматически используют этот credential
await _counterpartyRepository.Get(externalId, cancellationToken);
await _contractRepository.Get(externalId, cancellationToken);
```

## См. также
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - устранение проблем
- [Scripts/clear-hangfire-locks.sql](./Scripts/clear-hangfire-locks.sql) - SQL скрипты для обслуживания

