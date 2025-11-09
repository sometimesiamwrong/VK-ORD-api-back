# Конфигурация окружений AdLawyer API

## Обзор

Проект поддерживает разделение конфигурации для различных окружений:
- **Development** - локальная разработка
- **Production** - продакшен сервер

## Структура файлов конфигурации

### WebApp

```
src/WebApp/
├── appsettings.json                    # Базовые настройки (по умолчанию для Development)
├── appsettings.Development.json        # Настройки для локальной разработки
└── appsettings.Production.json         # Настройки для продакшена
```

### Jobs

```
src/Jobs/
├── appsettings.json                    # Базовые настройки (по умолчанию для Development)
├── appsettings.Development.json        # Настройки для локальной разработки
└── appsettings.Production.json         # Настройки для продакшена
```

## Как работает переключение окружений

### Локальная разработка (Development)

По умолчанию при запуске через `dotnet run` используется окружение **Development**.

```bash
cd src/WebApp
dotnet run
# Использует appsettings.json + appsettings.Development.json
```

Настройки из `appsettings.Development.json` **переопределяют** настройки из `appsettings.json`.

### Production

При развертывании через `deploy.sh` автоматически устанавливается переменная окружения `ASPNETCORE_ENVIRONMENT=Production`.

Это можно увидеть в файлах systemd служб:

```ini
[Service]
Environment=ASPNETCORE_ENVIRONMENT=Production
```

При запуске приложение использует: `appsettings.json` + `appsettings.Production.json`

### Ручная установка окружения

#### Windows (PowerShell)
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run
```

#### Windows (CMD)
```cmd
set ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

#### Linux/macOS
```bash
export ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

## Основные различия между окружениями

### Database Connections

#### Development
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=adlawyer_dev;Username=postgres;Password=postgres;Maximum Pool Size=50;"
  }
}
```

**Особенности:**
- Локальная база данных PostgreSQL
- База данных для разработки (`adlawyer_dev`)
- Стандартные учетные данные

#### Production
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=79.174.89.150;Port=19474;Database=vk_user_root;Username=vkord_user_root;Password=QZF-hL2-y4v-hYX;Maximum Pool Size=50;"
  }
}
```

**Особенности:**
- Удаленный сервер PostgreSQL
- Продакшен база данных
- Безопасные учетные данные

### Logging

#### Development
- Уровень логирования: **Debug**
- Детальное логирование EF Core: **Включено**
- Формат: Plain text (читаемый)
- Логи в файлах: `logs/jobs-dev-.txt`, `logs/vkord-api-.txt`

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "Microsoft.EntityFrameworkCore": "Information"
      }
    }
  }
}
```

#### Production
- Уровень логирования: **Information**
- Детальное логирование EF Core: **Отключено**
- Формат: JSON (для парсинга и анализа)
- Логи в файлах: `logs/jobs-production-.txt`, `logs/vkord-api-.txt`
- Ротация логов: 30 дней, 10MB на файл

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "Serilog.Formatting.Json.JsonFormatter, Serilog"
        }
      }
    ]
  }
}
```

### Redis Cache

#### Development
```json
{
  "RedisSettings": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "VkOrdApi_Dev",
    "DefaultDatabase": 1
  }
}
```

**Особенности:**
- Локальный Redis
- Отдельная база данных (1) для изоляции
- Префикс `VkOrdApi_Dev`

#### Production
```json
{
  "RedisSettings": {
    "ConnectionString": "redis:6379",
    "InstanceName": "VkOrdApi",
    "DefaultDatabase": 0
  }
}
```

**Особенности:**
- Redis в Docker контейнере
- База данных 0 (по умолчанию)
- Префикс `VkOrdApi`

### VkOrd API Settings

#### Development
```json
{
  "VkOrd": {
    "ApiToken": "3c58cc4ec81f4a68a1bf9f84a22e0826",
    "UseProduction": false,
    "MaxConcurrentRequests": 5
  }
}
```

**Особенности:**
- Тестовый API токен
- Песочница VK API (`UseProduction: false`)
- Ограниченное количество параллельных запросов

#### Production
```json
{
  "VkOrd": {
    "ApiToken": "159c21eaecc34823abb138f5724b5637",
    "UseProduction": true,
    "MaxConcurrentRequests": 10
  }
}
```

**Особенности:**
- Продакшен API токен
- Боевой VK API (`UseProduction: true`)
- Увеличенное количество параллельных запросов

### JWT Settings

#### Development
```json
{
  "JwtSettings": {
    "SecretKey": "dev-secret-key-for-jwt-tokens-must-be-at-least-32-characters-long-and-secure-123456789",
    "ExpiryMinutes": 1440
  }
}
```

#### Production
```json
{
  "JwtSettings": {
    "SecretKey": "r3alS3cr3tK3yF0rJWTt0k3ns-2024-06-Production!",
    "ExpiryMinutes": 1440
  }
}
```

**Важно:** Секретный ключ в Production должен быть уникальным и безопасным!

### Jobs Configuration

#### Development
```json
{
  "Jobs": {
    "ErirSyncIntervalMinutes": 5,
    "BatchSize": 1000,
    "ContinueOnError": true
  },
  "VkOrd": {
    "ConcurrencySettings": {
      "MaxParallelRequests": 3,
      "RateLimitPerMinute": 50
    }
  }
}
```

**Особенности:**
- Редкая синхронизация (каждые 5 минут)
- Маленький размер батча для тестирования
- Меньше параллельных запросов

#### Production
```json
{
  "Jobs": {
    "ErirSyncIntervalMinutes": 1,
    "BatchSize": 60000,
    "ContinueOnError": true
  },
  "VkOrd": {
    "ConcurrencySettings": {
      "MaxParallelRequests": 5,
      "RateLimitPerMinute": 100
    }
  }
}
```

**Особенности:**
- Частая синхронизация (каждую минуту)
- Большой размер батча для эффективности
- Больше параллельных запросов

### Entity Framework DbContext

#### Development
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging();  // Включено
    options.EnableDetailedErrors();         // Включено
});
```

**Особенности:**
- Детальное логирование SQL запросов
- Показ параметров в логах
- Детальные сообщения об ошибках

#### Production
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    // EnableSensitiveDataLogging - ОТКЛЮЧЕНО
    // EnableDetailedErrors - ОТКЛЮЧЕНО
});
```

**Особенности:**
- Без детального логирования (производительность)
- Без чувствительных данных в логах (безопасность)

## Проверка текущего окружения

### В коде

```csharp
var environment = builder.Environment.EnvironmentName;
Log.Information("Starting application in {Environment} environment", environment);

if (builder.Environment.IsDevelopment())
{
    // Код только для Development
}

if (builder.Environment.IsProduction())
{
    // Код только для Production
}
```

### В логах при запуске

```
[2025-01-09 10:00:00 INF] Starting application in Development environment
[2025-01-09 10:00:00 INF] Configuring database connection for Development environment
```

или

```
[2025-01-09 10:00:00 INF] Starting application in Production environment
[2025-01-09 10:00:00 INF] Configuring database connection for Production environment
```

### Через systemd

```bash
# Проверить переменные окружения службы
sudo systemctl show adlawyer-webapp | grep Environment

# Должно показать:
# Environment=ASPNETCORE_ENVIRONMENT=Production
```

## Безопасность

### ⚠️ Важные правила:

1. **Никогда не коммитьте** `appsettings.Production.json` с реальными паролями и токенами в публичный репозиторий
2. **Используйте переменные окружения** для чувствительных данных в Production
3. **Разные секретные ключи** для Development и Production
4. **Не логируйте чувствительные данные** в Production

### Использование переменных окружения

Можно переопределить любую настройку через переменные окружения:

```bash
# Формат: Section__SubSection__Key
export ConnectionStrings__DefaultConnection="Host=...;Database=..."
export JwtSettings__SecretKey="your-secret-key"
export VkOrd__ApiToken="your-api-token"
```

В systemd service:

```ini
[Service]
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ConnectionStrings__DefaultConnection=Host=...
Environment=JwtSettings__SecretKey=your-secret-key
```

## Добавление нового окружения (Staging)

1. Создайте файлы конфигурации:
   - `appsettings.Staging.json` в WebApp
   - `appsettings.Staging.json` в Jobs

2. Настройте переменную окружения:
   ```bash
   export ASPNETCORE_ENVIRONMENT=Staging
   ```

3. Обновите `deploy.sh` для поддержки Staging:
   ```bash
   ENVIRONMENT="Staging"
   ```

## Troubleshooting

### Приложение использует неправильную конфигурацию

**Проверьте:**
1. Переменную окружения `ASPNETCORE_ENVIRONMENT`
2. Наличие файла `appsettings.{Environment}.json`
3. Логи при запуске приложения

### База данных не подключается

**Проверьте:**
1. Строку подключения в соответствующем `appsettings.{Environment}.json`
2. Доступность сервера БД
3. Логи EF Core

```bash
# Включить детальное логирование EF Core временно
export Logging__LogLevel__Microsoft.EntityFrameworkCore=Debug
```

### Настройки не применяются

**Помните порядок приоритета:**
1. Переменные окружения (высший приоритет)
2. `appsettings.{Environment}.json`
3. `appsettings.json`
4. Значения по умолчанию в коде

---

**Дата обновления:** 2025-01-09  
**Версия:** 1.0

