# SSL Сертификаты для VkOrdApiWrapper

Этот документ описывает процесс настройки HTTPS с самоподписанными SSL сертификатами для VkOrdApiWrapper.

## Быстрый старт

### Linux/macOS
```bash
# Сделать скрипт исполняемым
chmod +x generate-ssl.sh

# Запустить генерацию сертификатов
./generate-ssl.sh
```

### Windows (PowerShell)
```powershell
# Запустить от имени администратора
.\generate-ssl.ps1
```

## Что делают скрипты

Скрипты создают:
- 📁 Папку `ssl/` с сертификатами
- 🔑 Приватный ключ (`vkord-api.key`)
- 📜 Сертификат (`vkord-api.crt`) 
- 📦 PFX файл (`vkord-api.pfx`) для .NET
- ⚙️ Конфигурационный файл (`cert.conf`)

## Параметры сертификата

- **Срок действия**: 10 лет (3650 дней)
- **Алгоритм**: RSA 2048 бит
- **Поддерживаемые домены**:
  - `localhost`
  - `*.localhost`
  - `vkord-api`
  - `*.vkord-api`
  - `127.0.0.1`
  - `::1`

## Конфигурация приложения

После генерации сертификатов приложение автоматически настроено для работы с HTTPS:

### Порты
- **HTTP**: 8080
- **HTTPS**: 5001

### Kestrel конфигурация
```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://+:8080"
      },
      "Https": {
        "Url": "https://+:5001",
        "Certificate": {
          "Path": "ssl/vkord-api.crt",
          "KeyPath": "ssl/vkord-api.key"
        }
      }
    }
  }
}
```

## Запуск приложения

### Локальная разработка
```bash
# С профилем httpsPROD
dotnet run --launch-profile httpsPROD
```

### Docker
```bash
# Сначала сгенерировать сертификаты
./generate-ssl.sh

# Затем запустить Docker Compose
docker-compose up -d
```

## Проверка работы HTTPS

### Curl
```bash
# С игнорированием самоподписанного сертификата
curl -k https://localhost:5001/

# Проверка health endpoint
curl -k https://localhost:5001/health
```

### Браузер
Откройте `https://localhost:5001` в браузере. Браузер покажет предупреждение о самоподписанном сертификате - это нормально.

## Использование с другими сервисами

### Игнорирование SSL ошибок в коде

#### C# HttpClient
```csharp
var handler = new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
};
var client = new HttpClient(handler);
```

#### JavaScript/Node.js
```javascript
// Только для разработки!
process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = 0;
```

#### Python requests
```python
import requests
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

response = requests.get('https://localhost:5001/', verify=False)
```

## Безопасность

⚠️ **ВАЖНО**: Это самоподписанные сертификаты для разработки и тестирования!

Для production среды рекомендуется:
- Использовать сертификаты от доверенного CA (Let's Encrypt, Cloudflare, etc.)
- Настроить автоматическое обновление сертификатов
- Использовать более строгие настройки SSL/TLS

## Troubleshooting

### Ошибка "Unable to configure HTTPS endpoint"
- Убедитесь что файлы сертификатов существуют в папке `ssl/`
- Проверьте права доступа к файлам сертификатов
- Убедитесь что пути к сертификатам правильные в конфигурации

### Ошибка "Certificate not found"
- Запустите скрипт генерации сертификатов заново
- Проверьте что папка `ssl/` содержит файлы `vkord-api.crt` и `vkord-api.key`

### Браузер не доверяет сертификату
- Это нормально для самоподписанных сертификатов
- Нажмите "Продолжить" или "Advanced" → "Proceed to localhost"
- Или добавьте сертификат в доверенные в настройках браузера

## Файлы

- `generate-ssl.sh` - Скрипт для Linux/macOS
- `generate-ssl.ps1` - Скрипт для Windows PowerShell
- `ssl/` - Папка с сгенерированными сертификатами
- `appsettings.Production.json` - Конфигурация Kestrel с HTTPS
- `docker-compose.yml` - Docker конфигурация с SSL volume
- `Properties/launchSettings.json` - Профили запуска с HTTPS
