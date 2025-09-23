# VK ORD API Wrapper - Запуск на Ubuntu 24.04 LTS

## 🚀 Быстрый запуск

```bash
# Скачайте скрипт
wget https://raw.githubusercontent.com/your-repo/vkord-api-wrapper/main/run.sh
chmod +x run.sh

# Запустите приложение
./run.sh
```

## 📋 Что делает скрипт

### Для Ubuntu 24.04 LTS:
✅ **Автоматически определяет** версию Ubuntu 24.04
✅ **Использует репозиторий Ubuntu 22.04** для совместимости с .NET 8
✅ **Устанавливает необходимые пакеты** (wget, apt-transport-https, ca-certificates)
✅ **Добавляет Microsoft repository** для .NET 8
✅ **Устанавливает .NET 8 Runtime**
✅ **Проверяет установку** и сообщает об ошибках

### Общие функции:
✅ **Восстанавливает NuGet пакеты**
✅ **Собирает проект в режиме Release**
✅ **Запускает ASP.NET Core приложение**
✅ **Настраивает Swagger UI** на порту 5000

## 🎯 Использование

### Полный запуск (рекомендуется):
```bash
./run.sh
```

### Только проверка зависимостей:
```bash
./run.sh --check-deps
```

### Только сборка проекта:
```bash
./run.sh --build-only
```

### Справка:
```bash
./run.sh --help
```

## 🔧 Переменные окружения

Создайте файл `.env` в той же папке для настройки:

```bash
# .env файл
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://*:5000
JWT_SECRET_KEY=your_secure_jwt_secret_key_here_min_32_chars
OPENROUTER_API_KEY=sk-or-v1-your_openrouter_api_key
DADATA_API_TOKEN=your_dadata_api_token
REDIS_CONFIGURATION=localhost:6379
PORT=5000
```

## 🌐 Доступ к приложению

После запуска приложение будет доступно по адресам:

- **Swagger UI (API документация):** http://localhost:5000/swagger
- **Health check:** http://localhost:5000/health
- **Основные API endpoints:** http://localhost:5000/api/

## 🛠 Технические детали

### .NET 8 на Ubuntu 24.04
Ubuntu 24.04 - новая LTS версия, и Microsoft пока не выпустила официальный пакет .NET 8 для неё. Скрипт использует проверенное решение:

1. **Обнаруживает Ubuntu 24.04**
2. **Использует репозиторий Ubuntu 22.04** (который совместим)
3. **Устанавливает .NET 8 Runtime** без проблем

### Системные требования
- Ubuntu 24.04 LTS
- sudo права
- Интернет-соединение
- Минимум 2GB RAM
- Минимум 5GB свободного места

## 🔍 Логи и диагностика

Скрипт показывает подробные логи:
- ✅ Успешные операции (зеленый цвет)
- ⚠️ Предупреждения (желтый цвет)
- ❌ Ошибки (красный цвет)

При проблемах проверьте:
```bash
# Проверить версию .NET
dotnet --version

# Проверить запущенные процессы
ps aux | grep dotnet

# Проверить порты
netstat -tlnp | grep :5000
```

## 🆘 Устранение проблем

### Если .NET не устанавливается:
```bash
# Очистить кэш apt
sudo apt-get clean
sudo apt-get autoclean

# Переустановить пакеты
sudo apt-get install --reinstall ca-certificates apt-transport-https

# Повторить установку
./run.sh --check-deps
```

### Если приложение не запускается:
```bash
# Проверить логи (если приложение запустилось)
journalctl -u vkord-api 2>/dev/null || echo "Systemd сервис не найден"

# Проверить порт
curl http://localhost:5000/health
```

## 📞 Поддержка

Скрипт поддерживает следующие дистрибутивы:
- ✅ Ubuntu 24.04 LTS (оптимизировано)
- ✅ Ubuntu 22.04, 20.04
- ✅ Debian 11/12
- ✅ CentOS/RHEL/AlmaLinux/Rocky Linux
- ✅ Fedora
- ✅ Alpine Linux
- ✅ Любая Unix система (локальная установка без sudo)

---

**Дата создания:** 23 сентября 2025 г.
**Версия скрипта:** Ubuntu 24.04 Optimized
