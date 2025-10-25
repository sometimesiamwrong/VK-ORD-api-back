API Layer → MediatR → Service Layer → Repository Layer → PostgreSQL
```

Где:
- **API Layer**: Контроллеры ASP.NET Core для обработки HTTP-запросов
- **MediatR**: Реализация CQRS паттерна для разделения команд и запросов
- **Service Layer**: Бизнес-логика, кэширование, интеграция с внешними API
- **Repository Layer**: Абстракция доступа к данным PostgreSQL через EF Core
- **PostgreSQL**: Основная база данных для хранения данных

## Основные технологии

- **.NET 8**: Фреймворк для разработки приложения
- **PostgreSQL 17**: Реляционная база данных
- **Redis 7**: Кэширование и сессии
- **Serilog**: Структурированное логирование
- **MediatR**: Реализация CQRS паттерна
- **Refit**: HTTP-клиент для внешних API
- **EF Core**: ORM для работы с PostgreSQL

# Архитектура и схема взаимодействия

## Request Pipeline

Обработка HTTP-запроса проходит через следующий pipeline:

```
HTTP Request → Middleware → Controller → MediatR → Behavior → Handler → Service → Repository → Database
```

### Роль каждого слоя:

1. **Middleware**: Обработка на уровне HTTP pipeline (логирование, аутентификация, обработка исключений)
2. **Controller**: Маршрутизация, валидация входных данных, отправка запросов в MediatR
3. **MediatR Behavior**: Кросс-функциональная логика (например, установка credential ID)
4. **Handler**: Реализация бизнес-логики для конкретных команд/запросов
5. **Service**: Интеграция с внешними API, кэширование, сложная бизнес-логика
6. **Repository**: Доступ к данным PostgreSQL через EF Core
7. **Database**: Хранение и извлечение данных

## Dependency Injection

Все компоненты регистрируются в DI-контейнере .NET и получают зависимости через конструкторы. Это обеспечивает слабую связанность и тестируемость.

## Используемые паттерны

- **CQRS**: Через MediatR разделение команд (изменение состояния) и запросов (чтение данных)
- **Repository**: Абстракция доступа к данным
- **Service Layer**: Инкапсуляция бизнес-логики
- **Factory**: VkOrdApiClientFactory для создания клиентов внешних API

# API Controllers (HTTP Request Handlers)

## Назначение

Контроллеры отвечают за обработку входящих HTTP-запросов, их валидацию и маршрутизацию к соответствующим MediatR handlers. Они наследуются от `BaseController` и используют стандартные атрибуты ASP.NET Core.

## Основные контроллеры

| Контроллер | Назначение | Основные методы |
|------------|------------|-----------------|
| `AuthController` | Аутентификация и авторизация | login, register, refresh, logout |
| `UsersController` | Управление профилями пользователей | get, update |
| `CredentialsController` | Управление API credentials для VK ORD | create, update, delete, list |
| `CounterpartiesController` | Работа с контрагентами | get, create, update |
| `ContractsController` | Работа с договорами | get, create, update |
| `CreativesController` | Работа с креативами | get, create, update |
| `MediaController` | Загрузка и управление медиафайлами | upload, get, delete |
| `StatisticsController` | Получение статистики | get |
| `InvoicesController` | Работа с актами | get, create, update |
| `AiController` | AI-классификация (KKTU) | classify |
| `DaDataController` | Интеграция с DaData API | findByInn |

## Методы работы

Контроллеры используют:
- Атрибуты маршрутизации: `[Route]`, `[HttpGet]`, `[HttpPost]`
- Атрибуты авторизации: `[Authorize]`
- `IMediator` для отправки команд/запросов
- `IOptions<T>` для конфигурации

## Используемые сервисы

- `IMediator`: Отправка запросов в MediatR pipeline
- `IOptions<T>`: Доступ к конфигурации

## Взаимодействие с PostgreSQL

Косвенное через MediatR handlers и repositories. Контроллеры не работают с БД напрямую.

## Примеры

```csharp
[HttpPost("v1/login")]
[AllowAnonymous]
public async Task<AuthResponse> Login([FromBody] LoginRequest request)
{
    var query = new LoginUserQuery 
    { 
        UserName = request.UserName, 
        Password = request.Password, 
        Ip = HttpContext.Connection.RemoteIpAddress?.ToString() 
    };

    var tokens = await _mediator.Send(query);
    // ... установка cookie и возврат ответа
}
```

# MediatR Handlers (Command/Query Handlers)

## Назначение

Handlers реализуют бизнес-логику для команд (Commands) и запросов (Queries) в рамках CQRS паттерна. Они изолируют логику от контроллеров и обеспечивают тестируемость.

## Основные handlers

| Handler | Назначение | Используемые сервисы |
|---------|------------|----------------------|
| `LoginUserHandler` | Обработка входа пользователя | Repositories, ITokenService, IPasswordHasher |
| `RegisterUserHandler` | Регистрация нового пользователя | Repositories, IPasswordHasher |
| `RefreshTokenHandler` | Обновление access token | Repositories, ITokenService |
| `RevokeRefreshTokenHandler` | Отзыв refresh token | Repositories |
| `CreateCreativeHandler` | Создание креатива в VK ORD | VkOrdService, Repositories |
| `CreateContractHandler` | Создание договора в VK ORD | VkOrdService, Repositories |
| `CreateCounterpartyFromInnHandler` | Создание контрагента по ИНН | DaDataService, VkOrdService |
| `GetCounterpartiesHandler` | Получение списка контрагентов | VkOrdService |
| `FindPartyByInnHandler` | Поиск компании по ИНН в DaData | DaDataService |

## Методы работы

Handlers реализуют интерфейс `IRequestHandler<TRequest, TResponse>` и получают зависимости через DI в конструкторе.

## Используемые сервисы

- Repositories для доступа к БД
- `ITokenService` для генерации JWT
- `IPasswordHasher<User>` для хеширования паролей
- Внешние API clients (VkOrdApiClient, DaDataApiClient)

## Взаимодействие с PostgreSQL

Через repositories, например:
- `IGetUserByNameRepository`
- `ISaveUserRepository`
- `ICreateCreativeRepository`

## Примеры

```csharp
public class LoginUserHandler : IRequestHandler<LoginUserQuery, TokenPair>
{
    private readonly IGetUserByNameRepository _getUserByNameRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<User> _hasher;

    public async Task<TokenPair> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _getUserByNameRepository.GetByName(request.UserName, cancellationToken);
        if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            throw BrokenRuleCodes.InvalidCredentials.AsExn();
        }

        return await _tokenService.GenerateTokens(user, request.Ip);
    }
}
```

# MediatR Pipeline Behaviors

## Назначение

Behaviors выполняют кросс-функциональную логику до или после выполнения handlers, аналогично middleware для MediatR pipeline.

## VkOrdKeyBehavior

### Назначение
Автоматическое извлечение и установка `ApiCredentialPublicId` из HTTP заголовка `x-vkord-credential-id` для запросов, связанных с VK ORD API.

### Методы работы
Проверяет, реализует ли request интерфейс `IRequestWithVkOrdKey`. Если да, извлекает GUID из заголовка и устанавливает значение через рефлексию.

### Используемые сервисы
- `IHttpContextAccessor`: Доступ к HTTP контексту

### Примеры
```csharp
public class VkOrdKeyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IRequestWithVkOrdKey)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var idStr = httpContext.Request.Headers["x-vkord-credential-id"].FirstOrDefault();
            if (!Guid.TryParse(idStr, out var credentialId))
            {
                throw new FormatException($"Invalid GUID format for x-vkord-credential-id: {idStr}");
            }

            var prop = typeof(TRequest).GetProperty(nameof(IRequestWithVkOrdKey.ApiCredentialPublicId));
            prop?.SetValue(request, credentialId);
        }

        return await next();
    }
}
```

# Middleware Components

## Назначение

Middleware обрабатывают HTTP-запросы на уровне ASP.NET Core pipeline, до того как они достигнут контроллеров.

## GlobalExceptionHandlingMiddleware

### Назначение
Централизованная обработка всех исключений в приложении с преобразованием в соответствующие HTTP-ответы.

### Методы работы
Перехватывает исключения, определяет HTTP статус код на основе типа исключения и форматирует JSON-ответ.

### Обработка исключений
- `BrokenRulesException` → 400/401 (в зависимости от кода ошибки)
- `ArgumentException` → 400
- `UnauthorizedAccessException` → 401
- `HttpRequestException` → 502

### Используемые сервисы
- `ILogger`: Логирование исключений
- `JsonSerializerOptions`: Сериализация ответов

### Примеры
При выбросе `BrokenRulesException` с кодом 401 возвращает HTTP 401 с детальным описанием ошибки в JSON.

## ResultStatusCodeMiddleware

### Назначение
Установка корректных HTTP статус кодов на основе результатов обработки запроса.

### Методы работы
Анализирует результат из `HttpContext.Items` и устанавливает соответствующий статус код.

# External API Clients

## IVkOrdApiClient (Refit)

### Назначение
Интеграция с VK ORD API для работы с рекламными данными (креативы, договоры, контрагенты, статистика).

### Основные методы
- `GetPersons`, `CreateOrUpdatePerson`
- `GetContracts`, `CreateOrUpdateContract`
- `GetCreatives`, `CreateOrUpdateCreativeV3`
- `UploadMedia`
- `GetStatisticsListV2`, `CreateStatisticsV2`
- `GetInvoices`, `CreateFullInvoiceV3`

### Используемые сервисы
HttpClient с custom handlers: `VkOrdApiHeaderHandler`, `VkOrdApiErrorHandler`

### Взаимодействие
REST API через HTTPS, JSON с snake_case, авторизация Bearer token.

### Retry policy
Экспоненциальная задержка (50ms, 100ms, 200ms) при 429, максимум 3 попытки.

### Circuit breaker
Обработка ошибок через `VkOrdApiErrorHandler`, преобразование в `BrokenRulesException`.

### Примеры
```csharp
var response = await _vkOrdApiClient.CreateOrUpdateCreativeV3(externalId, creativeRequest);
```

## IDaDataApiClient (Refit)

### Назначение
Интеграция с DaData API для поиска компаний по ИНН.

### Основные методы
- `FindById`: Поиск компании по ИНН

### Используемые сервисы
HttpClient с `Authorization: Token` header.

### Примеры
```csharp
var company = await _daDataApiClient.FindById(inn);
```

## IOpenRouterApiClient (Refit)

### Назначение
Интеграция с OpenRouter API для AI-классификации текстов.

### Основные методы
- `ChatCompletion`: Отправка запроса к LLM модели

### Используемые сервисы
HttpClient с `Authorization: Bearer` header.

### Примеры
```csharp
var response = await _openRouterApiClient.ChatCompletion(request);
```

# Service Layer

## VkOrdService

### Назначение
Бизнес-логика для работы с VK ORD API, кэширование, синхронизация с локальной БД.

### Основные методы
- `GetCreatives`, `CreateCreative`
- `GetContracts`, `CreateContract`
- `GetCounterparties`, `CreateCounterparty`
- `UploadMedia`
- `GetStatistics`, `CreateStatistics`

### Используемые сервисы
- `IVkOrdApiClientFactory`: Создание API клиентов
- Repositories: Работа с БД
- `ICacheService`: Кэширование

### Взаимодействие с PostgreSQL
Сохранение данных из VK ORD в таблицы: `VkOrdCreatives`, `VkOrdContracts`, `VkOrdCounterparties`, `VkOrdInvoices`, `VkOrdStatistics`.

### Примеры
```csharp
public async Task<List<Creative>> GetCreatives(Guid credentialId)
{
    var cached = await _cacheService.GetAsync<List<Creative>>($"creatives:{credentialId}");
    if (cached != null) return cached;

    var apiClient = await _vkOrdApiClientFactory.CreateClient();
    var response = await apiClient.GetCreativesV1(new PageRequest());
    
    // Сохранение в БД и кэш
    await _createCreativeRepository.CreateCreative(response.Items);
    await _cacheService.SetAsync($"creatives:{credentialId}", response.Items, TimeSpan.FromMinutes(60));
    
    return response.Items;
}
```

## CacheService

### Назначение
Управление кэшированием данных в Redis/Memory.

### Основные методы
- `GetAsync`, `SetAsync`, `RemoveAsync`, `GetOrCreateAsync`

### Используемые сервисы
- `IDistributedCache`: Redis или In-Memory
- `JsonSerializerOptions`: Сериализация

### Взаимодействие с PostgreSQL
Косвенное, кэширует результаты запросов к БД.

### Примеры
Кэширование списка контрагентов на 60 минут.

## DatabaseScriptService

### Назначение
Выполнение SQL скриптов при запуске приложения (миграции, seed data).

### Основные методы
- `ExecutePendingScriptsAsync`
- `IsScriptExecutedAsync`
- `ExecuteScriptAsync`

### Используемые сервисы
- `AppDbContext`: Выполнение SQL
- `ILogger`: Логирование

### Взаимодействие с PostgreSQL
Прямое выполнение SQL через `ExecuteSqlRawAsync`, отслеживание в таблице `DatabaseScripts`.

### Примеры
```sql
-- Пример скрипта up.sql
CREATE INDEX CONCURRENTLY IF NOT EXISTS "IX_VkOrdCreatives_ExternalId" 
ON "VkOrdCreatives" ("ExternalId");
```

## UserService

### Назначение
Управление пользователями.

### Основные методы
- `GetUserProfile`, `UpdateUserProfile`

### Используемые сервисы
Repositories: `IGetUserByIdRepository`, `ISaveUserRepository`

### Взаимодействие с PostgreSQL
CRUD с таблицей `Users`.

## ApiCredentialService

### Назначение
Управление API credentials для VK ORD.

### Основные методы
- `CreateCredential`, `UpdateCredential`, `DeleteCredential`, `GetCredentials`

### Используемые сервисы
Repositories, `ISecretProtector` для шифрования.

### Взаимодействие с PostgreSQL
CRUD с таблицей `ApiCredentials`, токены зашифрованы.

## AiService

### Назначение
AI-классификация текстов через OpenRouter API.

### Основные методы
- `ClassifyTextByKktu`

### Используемые сервисы
- `IOpenRouterApiClient`

### Примеры
Классификация описания креатива для определения KKTU кодов.

## DaDataService

### Назначение
Поиск компаний по ИНН через DaData API.

### Основные методы
- `FindPartyByInn`

### Используемые сервисы
- `IDaDataApiClient`
- Repositories для кэширования

## FlowTemplateService

### Назначение
Управление шаблонами workflow.

### Основные методы
- `CreateFlowTemplate`, `UpdateFlowTemplate`, `GetFlowTemplates`, `ActivateFlowTemplate`

### Используемые сервисы
Repositories, `IWizardFlowTemplateService`

### Взаимодействие с PostgreSQL
CRUD с таблицей `FlowTemplates`.

# Repository Layer

## Назначение

Repository Layer обеспечивает абстракцию доступа к данным, изолируя бизнес-логику от деталей БД.

## Паттерн

Repository per Entity, интерфейсы в `Interfaces/`, реализации в `Implementations/`.

## Основные группы repositories

### Users
- `IGetUserByIdRepository`
- `IGetUserByNameRepository`
- `ISaveUserRepository`
- `IDeleteUserRepository`
- `IGetUsersListRepository`

### ApiCredentials
- `IGetApiCredentialByGuidRepository`
- `IGetApiCredentialByIdRepository`
- `ISaveApiCredentialRepository`
- `IDeleteApiCredentialRepository`

### RefreshTokens
- `IGetRefreshTokenByHashRepository`
- `ISaveRefreshTokenRepository`
- `IDeleteRefreshTokenRepository`

### VkOrd.Counterparty
- `IGetCounterpartyByIdRepository`
- `IGetPageCounterpartiesRepository`
- `ICreateCounterpartyRepository`

### VkOrd.Contract
- `IGetContractRepository`
- `IGetContractDetailsRepository`
- `ICreateOrUpdateContractRepository`
- `IGetPageContractRepository`

### VkOrd.Creative
- `IGetCreativeRepository`
- `IGetCreativeByEridRepository`
- `ICreateCreativeRepository`
- `IGetPageCreativesRepository`

### VkOrd.Invoice
- `IGetInvoiceRepository`
- `ICreateOrUpdateInvoiceRepository`
- `ISendInvoiceToErirRepository`
- `IAddContractsToInvoiceRepository`

### VkOrd.Statistics
- `IGetStatisticsListRepository`
- `ICreateOrUpdateStatisticsRepository`
- `IDeleteStatisticsRepository`

## Методы работы

Используют `AppDbContext` (EF Core), выполняют LINQ запросы, `AsNoTracking` для read-only.

## Используемые сервисы

- `AppDbContext`
- `ILogger`

## Взаимодействие с PostgreSQL

- **ORM**: Entity Framework Core 8
- **Миграции**: Code-First в `Domain/Migrations/`
- **Запросы**: LINQ to Entities → SQL
- **Кэширование**: Через `CacheService`
- **Connection pooling**: `Maximum Pool Size=50`

## Примеры

```csharp
public class GetUserByNameRepository : IGetUserByNameRepository
{
    private readonly AppDbContext _context;

    public async Task<User?> GetByName(string userName, CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
}
```

EF Core генерирует: `SELECT * FROM "Users" WHERE "UserName" = @p0`

# Startup Agents

## DatabaseScriptService (выполняется при запуске)

### Назначение
Автоматическое выполнение SQL скриптов при старте приложения.

### Методы работы
В `Program.cs` создается scope, получается сервис, вызывается `ExecutePendingScriptsAsync`.

### Взаимодействие с PostgreSQL
Проверяет таблицу `DatabaseScripts`, выполняет новые скрипты, записывает результат.

### Примеры
Выполнение скриптов для создания индексов, триггеров, функций, seed data.

# Взаимодействие с PostgreSQL

## ORM
Entity Framework Core 8.0

## Provider
Npgsql.EntityFrameworkCore.PostgreSQL

## DbContext
`AppDbContext` (`Domain/Data/AppDbContext.cs`)

## Миграции
- Code-First подход
- Миграции в `Domain/Migrations/`
- Применение: `dotnet ef database update`
- Примеры: `20251012135556_InitialCreate`, `20251017165619_AddVkOrdInvoicesTable`

## Основные таблицы

| Таблица | Назначение |
|---------|------------|
| `Users` | Пользователи системы |
| `RefreshTokens` | Refresh токены для JWT |
| `ApiCredentials` | Credentials для VK ORD API |
| `DatabaseScripts` | История выполненных SQL скриптов |
| `FlowTemplates` | Шаблоны workflow |
| `VkOrdCounterparties` | Контрагенты из VK ORD |
| `VkOrdContracts` | Договоры из VK ORD |
| `VkOrdCreatives` | Креативы из VK ORD |
| `VkOrdInvoices` | Акты из VK ORD |
| `VkOrdStatistics` | Статистика из VK ORD |
| `VkOrdMedia` | Медиафайлы из VK ORD |

## Запросы
- LINQ to Entities для типизированных запросов
- `ExecuteSqlRawAsync` для raw SQL
- `AsNoTracking` для read-only (оптимизация)

## Кэширование
- Уровень приложения: Redis/Memory через `IDistributedCache`
- Уровень БД: PostgreSQL query cache (автоматически)

## Connection String
`Host=79.174.89.150;Port=19474;Database=vk_user;Username=vkord_user;Password=***;Maximum Pool Size=50`

# Механизмы мониторинга и логирования

## Serilog

### Конфигурация
В `appsettings.json`, минимальный уровень Information.

### Sinks
- Console (для Docker logs)
- File (`logs/vkord-api-.txt` с ежедневной ротацией)

### Структурированное логирование
Использование параметров `{PropertyName}`.

### Примеры
```csharp
_logger.LogInformation("Successfully executed script: {ScriptName}", scriptName);
```

## Типовые сценарии (Use‑cases)

### 1. Аутентификация пользователя и обновление токена

**Описание**: Пользователь входит в систему, получая пару access/refresh токенов. После истечения срока действия access токена, он обновляется с использованием refresh токена.

**Последовательность**: 
1. **HTTP Request**: `POST /v1/login` с `UserName` и `Password`.
2. **Controller**: `AuthController` вызывает `IMediator.Send(LoginUserQuery)`.
3. **Handler**: `LoginUserHandler` проверяет учетные данные через `IGetUserByNameRepository` и `IPasswordHasher`, генерирует токены через `ITokenService`.
4. **Service**: `ITokenService` создает JWT и Refresh Token, сохраняет Refresh Token в `RefreshTokens` через `ISaveRefreshTokenRepository`.
5. **HTTP Response**: Возвращается `AuthResponse` с `AccessToken` и `RefreshToken` (устанавливается как HTTP-Only cookie).

**Пример входных данных (LoginRequest)**:
```json
{
  "userName": "123123",
  "password": "123"
}
```

**Пример выходных данных (AuthResponse)**:
```json
{
  "accessToken": "eyJ...",
  "expiresIn": 3600
}
```

### 2. Создание креатива в VK ORD

**Описание**: Пользователь создает новый креатив, который регистрируется во внешней системе VK ORD через API.

**Последовательность**:
1. **HTTP Request**: `POST /v1/creatives` с данными креатива и заголовком `x-vkord-credential-id`.
2. **Controller**: `CreativesController` вызывает `IMediator.Send(CreateCreativeCommand)`.
3. **MediatR Behavior**: `VkOrdKeyBehavior` извлекает `ApiCredentialPublicId` из заголовка.
4. **Handler**: `CreateCreativeHandler` вызывает `VkOrdService.CreateCreative`.
5. **Service**: `VkOrdService` получает `IVkOrdApiClient` через `IVkOrdApiClientFactory` и вызывает `CreateOrUpdateCreativeV3`.
6. **External API**: Запрос отправляется в VK ORD API. Обрабатываются ошибки (`VkOrdApiErrorHandler`) и повторные попытки (Retry policy).
7. **Repository**: `ICreateCreativeRepository` сохраняет данные креатива в локальную БД (`VkOrdCreatives`).
8. **HTTP Response**: Возвращается созданный `CreativeResponse`.

**Пример входных данных (CreateCreativeRequest)**:
```json
{
  "name": "Новый баннер",
  "type": "banner",
  "url": "https://example.com/banner.jpg",
  "format": "image"
}
```
**Header**: `x-vkord-credential-id: YOUR_API_CREDENTIAL_GUID`

**Пример выходных данных (CreativeResponse)**:
```json
{
  "publicId": "GUID_CREATIVE",
  "externalId": "VKORD_CREATIVE_ID",
  "name": "Новый баннер",
  "status": "active"
}
```

### 3. Поиск контрагента по ИНН

**Описание**: Пользователь ищет информацию о контрагенте по ИНН, используя DaData API.

**Последовательность**:
1. **HTTP Request**: `GET /v1/dadata/findByInn?inn=YOUR_INN`.
2. **Controller**: `DaDataController` вызывает `IMediator.Send(FindPartyByInnQuery)`.
3. **Handler**: `FindPartyByInnHandler` вызывает `DaDataService.FindPartyByInn`.
4. **Service**: `DaDataService` проверяет кэш через `ICacheService`. Если нет в кэше, получает `IDaDataApiClient` и вызывает `FindById`.
5. **External API**: Запрос отправляется в DaData API.
6. **Service**: Результат кэшируется в `ICacheService`.
7. **HTTP Response**: Возвращается `PartyResponse` с данными о компании.

**Пример входных данных (Query Parameter)**:
`inn=7707083893`

**Пример выходных данных (PartyResponse)**:
```json
{
  "inn": "7707083893",
  "kpp": "770701001",
  "fullName": "Публичное акционерное общество «Сбербанк России»",
  "shortName": "ПАО Сбербанк"
}
```

### 4. Получение статистики из кэша

**Описание**: Система получает статистические данные из кэша для повышения производительности, или запрашивает их из VK ORD API, если данные отсутствуют или устарели.

**Последовательность**:
1. **HTTP Request**: `GET /v1/statistics` с `x-vkord-credential-id`.
2. **Controller**: `StatisticsController` вызывает `IMediator.Send(GetStatisticsQuery)`.
3. **MediatR Behavior**: `VkOrdKeyBehavior` извлекает `ApiCredentialPublicId`.
4. **Handler**: `GetStatisticsHandler` вызывает `VkOrdService.GetStatistics`.
5. **Service**: `VkOrdService` пытается получить статистику из `ICacheService` по ключу `statistics:{credentialId}`.
6. **Service (Cache Hit)**: Если данные найдены в кэше, они немедленно возвращаются.
7. **Service (Cache Miss)**: Если данные отсутствуют, `VkOrdService` получает `IVkOrdApiClient` и вызывает `GetStatisticsListV2`.
8. **Repository**: `ICreateOrUpdateStatisticsRepository` сохраняет новые данные в локальную БД (`VkOrdStatistics`).
9. **Service**: Полученные данные сохраняются в `ICacheService` с TTL (например, 60 минут).
10. **HTTP Response**: Возвращается `StatisticsResponse`.

**Header**: `x-vkord-credential-id: YOUR_API_CREDENTIAL_GUID`

**Пример выходных данных (StatisticsResponse)**:
```json
[
  {
    "date": "2025-01-01",
    "views": 1000,
    "clicks": 50,
    "cost": 150.75
  },
  {
    "date": "2025-01-02",
    "views": 1200,
    "clicks": 60,
    "cost": 180.20
  }
]
```