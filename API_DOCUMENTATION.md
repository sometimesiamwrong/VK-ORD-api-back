# AdLawyer API Documentation

## Обзор

Документация API контроллеров и моделей данных для системы AdLawyer, интегрированной с VK ORD API.

## Содержание

1. [Аутентификация](#аутентификация)
2. [Пользователи](#пользователи)
3. [API-ключи](#api-ключи)
4. [DaData](#dadata)
5. [Контрагенты](#контрагенты)
6. [Договоры](#договоры)
7. [Креативы](#креативы)
8. [Медиа](#медиа)
9. [Статистика](#статистика)
10. [AI](#ai)
11. [Общие структуры](#общие-структуры)

---

## Аутентификация

### AuthController (`/api/auth`)

#### POST `/register` - Регистрация пользователя
**Входные данные:**
```json
{
  "userName": "string",
  "password": "string",
  "name": "string"
}
```

**Ответ:**
```json
{
  "token": "string",
  "tokenType": "string",
  "expiresIn": "number",
  "issuedAt": "datetime",
  "expiresAt": "datetime", 
  "refreshToken": "string"
}
```

#### POST `/login` - Вход в систему
**Входные данные:**
```json
{
  "userName": "string",
  "password": "string"
}
```

**Ответ:** `AuthResponse` (см. выше)

#### POST `/refresh` - Обновление токена
**Входные данные:** Refresh token из cookie

**Ответ:** `AuthResponse`

#### POST `/logout` - Выход из системы
**Ответ:** `200 OK`

---

## Пользователи

### UsersController (`/api/users`)

#### GET `/me` - Получить профиль текущего пользователя
**Ответ:**
```json
{
  "publicId": "guid",
  "userName": "string",
  "name": "string",
  "isActive": "boolean",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

#### PATCH `/me` - Обновить профиль пользователя
**Входные данные:**
```json
{
  "name": "string"
}
```

**Ответ:** `UserProfileResponse`

---

## API-ключи

### CredentialsController (`/api/credentials`)

#### GET `/{userId}` - Получить список API-ключей пользователя
**Ответ:** `List<ApiCredentialResponse>`

#### GET `/{userId}/{credentialPublicId}` - Получить API-ключ по ID
**Ответ:** `ApiCredentialResponse?`

#### POST `/` - Создать новый API-ключ
**Входные данные:**
```json
{
  "environment": "string",
  "tokenPlain": "string",
  "displayName": "string"
}
```

**Ответ:**
```json
{
  "publicId": "guid",
  "environment": "string",
  "displayName": "string",
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

#### PUT `/{id}` - Обновить API-ключ
**Входные данные:**
```json
{
  "environment": "string",
  "tokenPlain": "string", 
  "displayName": "string"
}
```

**Ответ:** `ApiCredentialResponse?`

#### DELETE `/{id}` - Удалить API-ключ
**Ответ:** `boolean`

---

## DaData

### DaDataController (`/api/dadata`)

#### GET `/party/{inn}` - Поиск компании по ИНН
**Ответ:**
```json
{
  "value": "string",
  "status": "string",
  "opf": {
    "type": "string",
    "code": "string",
    "full": "string",
    "short": "string"
  },
  "name": {
    "fullWithOpf": "string",
    "shortWithOpf": "string",
    "latin": "string",
    "full": "string",
    "short": "string"
  },
  "inn": "string",
  "ogrn": "string",
  "okpo": "string",
  "okato": "string",
  "oktmo": "string",
  "okogu": "string",
  "okfs": "string",
  "okved": "string",
  "fio": {
    "surname": "string",
    "name": "string",
    "patronymic": "string"
  },
  "type": "string",
  "phone": "string",
  "kpp": "string",
  "email": "string"
}
```

---

## Контрагенты

### ClientApiController (`/api/clientapi`)

#### POST `/party` - Поиск компании по ИНН (client api)
**Входные данные:** `FindPartyByInnQuery`
**Ответ:** `DaDataPartyShortResponse`

#### POST `/set-counterparty` - Создать контрагента в VK ОРД по ИНН
**Входные данные:**
```json
{
  "inn": "string",
  "types": ["enum"]
}
```

#### GET `/counterparties` - Получить список всех контрагентов
**Входные данные:** `GetCounterpartiesRequest` (query)
**Ответ:**
```json
{
  "data": [VkOrdPersonResponse],
  "totalCount": "number",
  "totalItemsCount": "number",
  "limit": "number"
}
```

#### GET `/counterparties/{externalId}` - Получить контрагента по external_id
**Ответ:**
```json
{
  "externalId": "string",
  "data": {
    "name": "string",
    "rsUrl": "string",
    "roles": ["string"],
    "juridicalDetails": {
      "type": "enum",
      "modelScheme": "string",
      "inn": "string",
      "kpp": "string",
      "phone": "string",
      "foreignEpaymentMethod": "string",
      "foreignRegistrationNumber": "string",
      "foreignInn": "string",
      "foreignOksmCountryCode": "string"
    }
  }
}
```

### CounterpartiesController (`/api/v1/counterparties`)

#### GET `/by-inn/{inn}` - Получить контрагентов по ИНН
**Параметры запроса:**
- `cacheOnly` (boolean, default: false)
- `forceRefresh` (boolean, default: false)
- `cacheTtlMinutes` (number, default: 60)
- `refreshThreshold` (number, default: 0.8)
- `includeRelatedData` (boolean, default: false)
- `maxResults` (number, default: 100)

**Ответ:**
```json
{
  "counterparties": [
    {
      "external_id": "string",
      "inn": "string",
      "name": "string",
      "rs_url": "string",
      "roles": ["string"],
      "juridical_details": {
        "inn": "string",
        "kpp": "string",
        "phone": "string",
        "foreign_epayment_method": "string",
        "foreign_registration_number": "string",
        "foreign_inn": "string",
        "foreign_oksm_country_code": "string"
      },
      "last_updated": "datetime",
      "sync_status": "string"
    }
  ],
  "totalCount": "number",
  "returnedCount": "number",
  "source": "enum",
  "retrievedAt": "datetime",
  "cacheExpiresAt": "datetime",
  "cacheCreatedAt": "datetime",
  "cacheVersion": "number",
  "dataHash": "string",
  "cacheStatistics": {
    "executionTimeMs": "number",
    "cacheHit": "boolean",
    "dataSizeBytes": "number",
    "recordCount": "number",
    "metrics": {}
  }
}
```

#### GET `/{inn}/contracts` - Получить договоры контрагента
**Параметры запроса:**
- `cacheOnly` (boolean, default: false)
- `forceRefresh` (boolean, default: false)
- `cacheTtlMinutes` (number, default: 60)
- `refreshThreshold` (number, default: 0.8)
- `maxResults` (number, default: 100)
- `includeAdditionalContracts` (boolean, default: true)

**Ответ:**
```json
{
  "contracts": [ContractDto],
  "totalCount": "number",
  "returnedCount": "number",
  "source": "enum",
  "retrievedAt": "datetime",
  "cacheExpiresAt": "datetime",
  "cacheCreatedAt": "datetime",
  "cacheVersion": "number",
  "dataHash": "string",
  "cacheStatistics": {}
}
```

#### GET `/{inn}/related` - Получить связанных контрагентов
**Параметры запроса:**
- `cacheOnly` (boolean, default: false)
- `forceRefresh` (boolean, default: false)
- `cacheTtlMinutes` (number, default: 60)
- `refreshThreshold` (number, default: 0.8)
- `maxResults` (number, default: 100)
- `relationTypes` (array of strings)

**Ответ:** `GetRelatedCounterpartiesResponse`

---

## Договоры

### ClientApiController (`/api/clientapi`)

#### POST `/create_contract` - Создать контракт в VK ОРД
**Входные данные:**
```json
{
  "externalId": "string",
  "clientExternalId": "string",
  "contractorExternalId": "string",
  "paySum": "number",
  "date": "string",
  "dateEnd": "string",
  "serial": "string"
}
```

### ContractsController (`/api/v1/contracts`)

#### GET `/between/{clientInn}/{contractorInn}` - Получить договор между контрагентами
**Ответ:**
```json
{
  "contract": {
    "id": "number",
    "externalId": "string",
    "type": "string",
    "clientExternalId": "string",
    "contractorExternalId": "string",
    "parentContractExternalId": "string",
    "amount": "decimal",
    "cachedAt": "datetime",
    "expiresAt": "datetime",
    "lastUpdated": "datetime",
    "syncStatus": "string"
  },
  "source": "enum",
  "retrievedAt": "datetime",
  "cacheExpiresAt": "datetime",
  "cacheCreatedAt": "datetime",
  "cacheVersion": "number",
  "dataHash": "string",
  "cacheStatistics": {}
}
```

#### GET `/{contractExternalId}/details` - Получить детали договора с креативами
**Ответ:**
```json
{
  "contract": ContractDto,
  "creatives": [
    {
      "id": "number",
      "externalId": "string",
      "erid": "string",
      "personExternalId": "string",
      "name": "string",
      "brand": "string",
      "category": "string",
      "description": "string",
      "payType": "string",
      "form": "string",
      "targeting": "string",
      "status": "string",
      "cachedAt": "datetime",
      "expiresAt": "datetime",
      "lastUpdated": "datetime",
      "syncStatus": "string"
    }
  ],
  "totalCreatives": "number",
  "returnedCreatives": "number",
  "source": "enum",
  "retrievedAt": "datetime",
  "cacheExpiresAt": "datetime",
  "cacheCreatedAt": "datetime",
  "cacheVersion": "number",
  "dataHash": "string",
  "cacheStatistics": {}
}
```

---

## Креативы

### ClientApiController (`/api/clientapi`)

#### POST `/create_creative` - Создать креатив в VK ОРД
**Входные данные:**
```json
{
  "externalId": "string",
  "contractExternalIds": ["string"],
  "mediaExternalIds": ["string"],
  "kktus": ["string"],
  "type": "enum",
  "targetUrls": ["string"],
  "targetAudience": "string",
  "texts": ["string"],
  "name": "string"
}
```

**Ответ:**
```json
{
  "erid": "string"
}
```

### CreativesController (`/api/creatives`)

#### POST `/` - Создать новый креатив
**Входные данные:** `CreateCreativeRequest`
**Ответ:** `VkOrdCreativeV3RequestResponse`

#### GET `/{externalId}` - Получить информацию о креативе
**Ответ:**
```json
{
  "erid": "string",
  "person_external_id": "string",
  "contract_external_ids": ["string"],
  "kktus": ["string"],
  "name": "string",
  "brand": "string",
  "category": "string",
  "description": "string",
  "pay_type": "enum",
  "form": "enum",
  "targeting": "string",
  "target_urls": ["string"],
  "texts": ["string"],
  "media_external_ids": ["string"],
  "flags": ["enum"]
}
```

#### GET `/` - Получить список креативов
**Входные данные:** `PageRequest` (query)
**Ответ:**
```json
{
  "data": [VkOrdCreativeV3Response],
  "totalCount": "number",
  "totalItemsCount": "number",
  "limit": "number"
}
```

#### GET `/by-erid/{erid}` - Получить креатив по ERID
**Ответ:** `VkOrdCreativeV3Response`

---

## Медиа

### MediaController (`/api/media`)

#### POST `/upload` - Загрузить медиа файл
**Входные данные:** `multipart/form-data`
```json
{
  "file": "IFormFile"
}
```

**Ответ:** `string` (external_id)

#### GET `/{externalId}` - Получить информацию о медиа файле
**Ответ:**
```json
{
  "filename": "string",
  "sha256": "string",
  "createDate": "string",
  "size": "number",
  "contentType": "string",
  "description": "string"
}
```

#### GET `/page` - Получить список медиа файлов
**Входные данные:** `PageRequest` (query)
**Ответ:**
```json
{
  "media": [VkOrdMediaInfoResponse],
  "totalCount": "number",
  "total_items_count": "number",
  "limit": "number"
}
```

---

## Статистика

### StatisticsController (`/api/v1/statistics`)

#### POST `/acts` - Получить статистику актов за период
**Входные данные:**
```json
{
  "startDate": "date",
  "endDate": "date",
  "counterpartyInn": "string",
  "contractExternalId": "string",
  "creativeExternalId": "string",
  "maxResults": "number"
}
```

**Ответ:**
```json
{
  "statistics": [
    {
      "id": "number",
      "externalId": "string",
      "creativeExternalId": "string",
      "padExternalId": "string",
      "showsCount": "number",
      "invoiceShowsCount": "number",
      "amount": "decimal",
      "amountPerEvent": "decimal",
      "payType": "string",
      "period": "string",
      "statisticsType": "string",
      "dateStartPlanned": "datetime",
      "dateEndPlanned": "datetime",
      "dateStartActual": "datetime",
      "dateEndActual": "datetime",
      "cachedAt": "datetime",
      "expiresAt": "datetime",
      "lastUpdated": "datetime",
      "syncStatus": "string"
    }
  ],
  "totalCount": "number",
  "returnedCount": "number",
  "totalAmount": "decimal",
  "totalShows": "number",
  "source": "enum",
  "retrievedAt": "datetime",
  "cacheExpiresAt": "datetime",
  "cacheCreatedAt": "datetime",
  "cacheVersion": "number",
  "dataHash": "string",
  "cacheStatistics": {}
}
```

#### GET `/acts` - Получить статистику актов (GET версия)
**Параметры запроса:**
- `startDate` (datetime)
- `endDate` (datetime)
- `creativeExternalId` (string, optional)
- `padExternalId` (string, optional)
- `cacheOnly` (boolean, default: false)
- `forceRefresh` (boolean, default: false)

**Ответ:** `GetActStatisticsResponse`

---

## AI

### AiController (`/api/ai`)

#### POST `/get-kkty_by-text` - Получить классификацию KKTY по тексту
**Входные данные:**
```json
{
  "text": "string"
}
```

**Ответ:**
```json
{
  "kkty": ["string"]
}
```

---

## Общие структуры

### CacheResponse (базовый класс)
Все ответы с кэшированием наследуются от этого класса:
```json
{
  "source": "enum (Cache=0, Api=1, Mixed=2)",
  "retrievedAt": "datetime",
  "cacheExpiresAt": "datetime",
  "cacheCreatedAt": "datetime",
  "cacheVersion": "number",
  "dataHash": "string",
  "cacheStatistics": {
    "executionTimeMs": "number",
    "cacheHit": "boolean",
    "dataSizeBytes": "number",
    "recordCount": "number",
    "metrics": {}
  }
}
```

### PageRequest
```json
{
  "page": "number",
  "limit": "number"
}
```

### DataSource (enum)
- `Cache = 0` - Из кэша
- `Api = 1` - Из API VK ORD  
- `Mixed = 2` - Смешанный (кэш + API)

---

## Аутентификация и авторизация

### Требования
- Все контроллеры требуют авторизации (кроме эндпоинтов аутентификации)
- Для работы с VK ORD API требуется заголовок `x-vkord-credential-id`
- Используется JWT токен для авторизации
- Refresh токен передается через HTTP cookie

### Заголовки
```
Authorization: Bearer <jwt_token>
x-vkord-credential-id: <credential_guid>
Content-Type: application/json
```

---

## Коды ответов

- `200 OK` - Успешный запрос
- `400 Bad Request` - Неверные параметры запроса
- `401 Unauthorized` - Не авторизован
- `403 Forbidden` - Нет доступа
- `404 Not Found` - Ресурс не найден
- `500 Internal Server Error` - Внутренняя ошибка сервера

---

## Примечания

1. Все даты и время передаются в формате ISO 8601
2. Десятичные числа передаются как строки для точности
3. Кэширование автоматически управляется системой
4. Все ответы с кэшированием содержат метаданные о источнике данных
5. Параметры `cacheOnly`, `forceRefresh`, `cacheTtlMinutes`, `refreshThreshold` доступны для всех кэшируемых эндпоинтов
