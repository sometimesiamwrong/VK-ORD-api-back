# VK ORD API Wrapper - Production Deployment Ready

## Релизная версия собрана успешно! 🚀

### Что было сделано:
- ✅ Собрана релизная версия приложения (.NET 8 Release)
- ✅ Создан архив для развертывания: `VkOrdApiWrapper-Release-20250923-111655.zip`
- ✅ Подготовлен файл `.env` с переменными окружения

### Структура релизной сборки:
```
publish/
├── VkOrdApiWrapper.dll          # Основная сборка
├── VkOrdApiWrapper.exe          # Исполняемый файл
├── appsettings.json             # Конфигурация
├── appsettings.Production.json  # Продакшн настройки
├── web.config                   # IIS конфигурация
├── runtimes/                    # Нативные библиотеки для разных ОС
└── [зависимости .dll]           # Все необходимые зависимости
```

## Варианты развертывания:

### 1. Развертывание на Windows Server/IIS

#### Требования:
- Windows Server 2016+ или Windows 10+
- .NET 8 Runtime (Hosting Bundle)
- IIS с ASP.NET Core Module

#### Шаги:
1. **Установите .NET 8 Runtime:**
   ```powershell
   # Скачайте и установите .NET 8 Hosting Bundle
   # https://dotnet.microsoft.com/download/dotnet/8.0
   ```

2. **Создайте сайт в IIS:**
   ```powershell
   # 1. Создайте папку для приложения
   New-Item -ItemType Directory -Path "C:\inetpub\wwwroot\VkOrdApi"

   # 2. Распакуйте архив в папку сайта
   Expand-Archive -Path "VkOrdApiWrapper-Release-*.zip" -DestinationPath "C:\inetpub\wwwroot\VkOrdApi"

   # 3. Настройте переменные окружения в web.config или через IIS
   ```

3. **Настройте переменные окружения:**
   ```xml
   <!-- В web.config добавьте: -->
   <environmentVariables>
     <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
     <environmentVariable name="JWT_SECRET_KEY" value="your_jwt_secret_here" />
     <environmentVariable name="OPENROUTER_API_KEY" value="your_openrouter_key_here" />
     <environmentVariable name="DADATA_API_TOKEN" value="your_dadata_token_here" />
     <environmentVariable name="ConnectionStrings__DefaultConnection" value="your_db_connection_string" />
   </environmentVariables>
   ```

### 2. Развертывание как Windows Service

```powershell
# 1. Установите как службу
sc.exe create VkOrdApi binPath="C:\path\to\publish\VkOrdApiWrapper.exe"

# 2. Настройте переменные окружения для службы
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", "Machine")
[Environment]::SetEnvironmentVariable("ASPNETCORE_URLS", "http://*:5000", "Machine")

# 3. Запустите службу
sc.exe start VkOrdApi
```

### 3. Развертывание на Linux

#### Требования:
- Linux с systemd
- .NET 8 Runtime

#### Шаги:
1. **Установите .NET 8:**
   ```bash
   # Ubuntu/Debian
   wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   sudo apt-get update
   sudo apt-get install -y dotnet-runtime-8.0
   ```

2. **Создайте пользователя и директорию:**
   ```bash
   sudo useradd -m -s /bin/bash vkord
   sudo mkdir -p /var/www/vkord-api
   sudo chown vkord:vkord /var/www/vkord-api
   ```

3. **Разверните приложение:**
   ```bash
   # Распакуйте архив
   unzip VkOrdApiWrapper-Release-*.zip -d /var/www/vkord-api/

   # Настройте переменные окружения
   sudo -u vkord tee /var/www/vkord-api/.env << EOF
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://*:5000
   JWT_SECRET_KEY=your_jwt_secret_here
   OPENROUTER_API_KEY=your_openrouter_key_here
   DADATA_API_TOKEN=your_dadata_token_here
   ConnectionStrings__DefaultConnection=your_db_connection_string
   EOF
   ```

4. **Создайте systemd service:**
   ```bash
   sudo tee /etc/systemd/system/vkord-api.service << EOF
   [Unit]
   Description=VK ORD API Wrapper
   After=network.target

   [Service]
   Type=simple
   User=vkord
   WorkingDirectory=/var/www/vkord-api
   ExecStart=/usr/bin/dotnet VkOrdApiWrapper.dll
   EnvironmentFile=/var/www/vkord-api/.env
   Restart=always
   RestartSec=10

   [Install]
   WantedBy=multi-user.target
   EOF

   sudo systemctl daemon-reload
   sudo systemctl enable vkord-api
   sudo systemctl start vkord-api
   ```

### 4. Docker развертывание (если Docker доступен)

Если на вашем хостинге есть Docker:

1. **Загрузите файлы на сервер:**
   - `VkOrdApiWrapper-Release-*.zip`
   - `docker-compose.yml`
   - `Dockerfile`
   - `.env` (с настроенными переменными)

2. **Запустите:**
   ```bash
   # Распакуйте исходники (если нужны)
   unzip VkOrdApiWrapper-Release-*.zip -d publish/

   # Настройте .env файл с реальными значениями
   nano .env

   # Запустите через Docker Compose
   docker-compose up -d
   ```

## Проверка развертывания:

### Health Check:
```bash
curl http://your-server:5000/health
```

### API документация (Swagger):
```
http://your-server:5000/swagger
```

### Логи приложения:
- Проверьте логи в `/var/log/vkord-api/` (Linux)
- Или в Event Viewer (Windows)
- Или через `docker-compose logs` (Docker)

## Необходимые переменные окружения:

| Переменная | Описание | Пример |
|------------|----------|---------|
| `ASPNETCORE_ENVIRONMENT` | Среда выполнения | `Production` |
| `ASPNETCORE_URLS` | URL для прослушивания | `http://*:5000` |
| `JWT_SECRET_KEY` | Секретный ключ JWT (минимум 32 символа) | `your_secure_jwt_secret_key_here` |
| `OPENROUTER_API_KEY` | API ключ OpenRouter | `sk-or-v1-xxxxx` |
| `DADATA_API_TOKEN` | API токен DaData | `your_dadata_token` |
| `ConnectionStrings__DefaultConnection` | Строка подключения к БД | `Host=localhost;Database=vkord;Username=user;Password=pass` |

## Безопасность:

⚠️ **Важно:**
1. Никогда не храните секреты в коде
2. Используйте HTTPS в продакшене
3. Регулярно обновляйте зависимости
4. Настройте firewall
5. Используйте strong пароли

## Мониторинг:

- Настройте логирование в файл или внешнюю систему
- Добавьте health checks
- Мониторьте использование ресурсов
- Настройте alerts для ошибок

---

**Создано:** 23 сентября 2025 г.
**Версия:** Release Build


