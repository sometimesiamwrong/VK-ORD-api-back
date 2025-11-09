# Скрипт развертывания AdLawyer API

## Описание

`deploy.sh` - это скрипт для автоматического развертывания AdLawyer API на сервере Ubuntu/Debian. Скрипт выполняет:

1. ✅ Публикацию проектов WebApp и Jobs
2. ✅ Создание/обновление служб systemd
3. ✅ Настройку прав доступа
4. ✅ Автозапуск служб
5. ✅ (Опционально) Настройку CLO туннелей для публичного доступа

## Быстрый старт

### На сервере

```bash
# 1. Клонировать репозиторий (если еще не клонирован)
cd /root
git clone <repository_url> AdLawyerApi
cd AdLawyerApi

# 2. Сделать скрипт исполняемым
chmod +x deploy.sh

# 3. Запустить развертывание
sudo ./deploy.sh
```

### Через GitLab CI/CD

При пуше в ветку `main` автоматически запустится пайплайн, который:
1. Подключится к серверу по SSH
2. Обновит код из репозитория
3. Запустит `deploy.sh`

## Конфигурация

Все настройки находятся в начале файла `deploy.sh`. Основные параметры:

### Общие настройки

```bash
REPO_ROOT="/root/AdLawyerApi"           # Путь к репозиторию на сервере
DOTNET_VERSION="net8.0"                 # Версия .NET
ENVIRONMENT="Production"                # Окружение (Production/Development/Staging)
```

### WebApp

```bash
WEBAPP_SERVICE_NAME="adlawyer-webapp"   # Имя службы systemd
WEBAPP_PORT="5000"                      # Порт для WebApp
WEBAPP_INSTALL_DIR="/var/www/adlawyer-webapp"  # Директория установки
WEBAPP_USER="www-data"                  # Пользователь для запуска службы
```

### Jobs

```bash
JOBS_SERVICE_NAME="adlawyer-jobs"       # Имя службы systemd
JOBS_PORT="5001"                        # Порт для Jobs
JOBS_INSTALL_DIR="/var/www/adlawyer-jobs"      # Директория установки
JOBS_USER="www-data"                    # Пользователь для запуска службы
```

### CLO Туннели (опционально)

```bash
ENABLE_CLO_TUNNEL=true                  # Включить/выключить CLO туннели
CLO_BIN="/root/clo"                     # Путь к бинарю CLO
```

## Структура после развертывания

```
/var/www/
├── adlawyer-webapp/          # WebApp приложение
│   ├── WebApp                # Исполняемый файл
│   ├── appsettings.json      # Конфигурация
│   ├── logs/                 # Логи приложения
│   └── ...
└── adlawyer-jobs/            # Jobs приложение
    ├── Jobs                  # Исполняемый файл
    ├── appsettings.json      # Конфигурация
    ├── logs/                 # Логи приложения
    └── ...

/etc/systemd/system/
├── adlawyer-webapp.service   # Служба WebApp
├── adlawyer-jobs.service     # Служба Jobs
├── clo-webapp-tunnel.service # CLO туннель для WebApp (опционально)
└── clo-jobs-tunnel.service   # CLO туннель для Jobs (опционально)
```

## Управление службами

### Основные команды

```bash
# Перезапуск служб
sudo systemctl restart adlawyer-webapp
sudo systemctl restart adlawyer-jobs

# Остановка служб
sudo systemctl stop adlawyer-webapp
sudo systemctl stop adlawyer-jobs

# Статус служб
sudo systemctl status adlawyer-webapp
sudo systemctl status adlawyer-jobs

# Просмотр логов в реальном времени
sudo journalctl -u adlawyer-webapp -f
sudo journalctl -u adlawyer-jobs -f

# Просмотр последних 100 строк логов
sudo journalctl -u adlawyer-webapp -n 100
sudo journalctl -u adlawyer-jobs -n 100

# Отключить автозапуск службы
sudo systemctl disable adlawyer-webapp
sudo systemctl disable adlawyer-jobs

# Включить автозапуск службы
sudo systemctl enable adlawyer-webapp
sudo systemctl enable adlawyer-jobs
```

### CLO Туннели

```bash
# Статус туннелей
sudo systemctl status clo-webapp-tunnel
sudo systemctl status clo-jobs-tunnel

# Получить публичный URL (из логов)
sudo journalctl -u clo-webapp-tunnel -n 20 | grep "https://"
sudo journalctl -u clo-jobs-tunnel -n 20 | grep "https://"

# Перезапуск туннелей
sudo systemctl restart clo-webapp-tunnel
sudo systemctl restart clo-jobs-tunnel
```

## Изменение конфигурации

### Изменить порт WebApp

1. Откройте `deploy.sh`
2. Измените `WEBAPP_PORT="5000"` на нужный порт
3. Запустите `sudo ./deploy.sh`

### Изменить директорию установки

1. Откройте `deploy.sh`
2. Измените `WEBAPP_INSTALL_DIR` или `JOBS_INSTALL_DIR`
3. Запустите `sudo ./deploy.sh`

### Отключить CLO туннели

1. Откройте `deploy.sh`
2. Измените `ENABLE_CLO_TUNNEL=true` на `ENABLE_CLO_TUNNEL=false`
3. Запустите `sudo ./deploy.sh`

## Требования

### На сервере должны быть установлены:

- **Ubuntu/Debian** (или совместимая система с systemd)
- **.NET 8.0 SDK** (скрипт попытается установить автоматически)
- **Git** (для клонирования репозитория)
- **CLO** (опционально, для публичных туннелей)

### Установка .NET SDK вручную:

```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
export PATH="$PATH:$HOME/.dotnet"
```

### Установка Git:

```bash
sudo apt update
sudo apt install git -y
```

## Troubleshooting

### Служба не запускается

```bash
# Проверить статус и ошибки
sudo systemctl status adlawyer-webapp
sudo journalctl -u adlawyer-webapp -n 50

# Проверить права доступа
ls -la /var/www/adlawyer-webapp

# Проверить конфигурацию
cat /etc/systemd/system/adlawyer-webapp.service
```

### База данных недоступна

1. Проверьте строку подключения в `appsettings.Production.json`
2. Убедитесь, что PostgreSQL запущен и доступен
3. Проверьте логи приложения

```bash
sudo journalctl -u adlawyer-webapp -f | grep -i "database\|connection"
```

### Порт уже занят

```bash
# Проверить, что использует порт
sudo netstat -tlnp | grep :5000

# Или с помощью lsof
sudo lsof -i :5000

# Изменить порт в deploy.sh и перезапустить
```

### Недостаточно прав

```bash
# Убедитесь, что пользователь www-data существует
id www-data

# Если нет, создайте его
sudo useradd -r -s /bin/false www-data

# Проверьте права на директории
sudo chown -R www-data:www-data /var/www/adlawyer-webapp
sudo chown -R www-data:www-data /var/www/adlawyer-jobs
```

## Безопасность

### Рекомендации:

1. **Не храните секреты в репозитории** - используйте переменные окружения или secrets manager
2. **Используйте отдельного пользователя** (не root) для запуска приложений
3. **Настройте firewall**:
   ```bash
   sudo ufw allow 5000/tcp  # WebApp
   sudo ufw allow 5001/tcp  # Jobs (если нужен внешний доступ)
   sudo ufw enable
   ```
4. **Используйте HTTPS** через nginx или другой reverse proxy
5. **Регулярно обновляйте** зависимости и систему

## GitLab CI/CD переменные

Для работы автоматического деплоя через GitLab CI/CD нужно настроить следующие переменные в Settings → CI/CD → Variables:

- `SSH_PRIVATE_KEY` - приватный SSH ключ для доступа к серверу
- `SSH_HOST` - адрес сервера (IP или домен)
- `SSH_USER` - пользователь SSH (обычно `root`)

### Генерация SSH ключа:

```bash
# На вашем компьютере
ssh-keygen -t ed25519 -C "gitlab-ci"

# Скопируйте публичный ключ на сервер
ssh-copy-id root@your-server-ip

# Приватный ключ добавьте в GitLab переменные
cat ~/.ssh/id_ed25519
```

## Логи

### Расположение логов:

1. **Системные логи (systemd)**:
   - Команда: `journalctl -u <service-name>`
   - Хранятся в `/var/log/journal/`

2. **Логи приложения**:
   - WebApp: `/var/www/adlawyer-webapp/logs/`
   - Jobs: `/var/www/adlawyer-jobs/logs/`

### Полезные команды для логов:

```bash
# Все логи службы
sudo journalctl -u adlawyer-webapp --no-pager

# Логи за последний час
sudo journalctl -u adlawyer-webapp --since "1 hour ago"

# Логи за конкретный период
sudo journalctl -u adlawyer-webapp --since "2025-01-09 10:00:00" --until "2025-01-09 11:00:00"

# Фильтр по приоритету (errors only)
sudo journalctl -u adlawyer-webapp -p err

# Экспорт логов в файл
sudo journalctl -u adlawyer-webapp > webapp-logs.txt
```

## Обновление

Для обновления приложения просто запустите скрипт заново:

```bash
cd /root/AdLawyerApi
git pull origin main
sudo ./deploy.sh
```

Скрипт автоматически:
- Остановит старые службы
- Удалит старые файлы
- Опубликует новую версию
- Запустит обновленные службы

## Откат (Rollback)

Для отката к предыдущей версии:

```bash
cd /root/AdLawyerApi
git log --oneline -n 5  # Посмотреть последние коммиты
git checkout <commit-hash>  # Откатиться к нужному коммиту
sudo ./deploy.sh
```

После проверки вернитесь на main:

```bash
git checkout main
```

## Поддержка

При возникновении проблем:

1. Проверьте логи служб
2. Проверьте конфигурацию в `deploy.sh`
3. Убедитесь, что все зависимости установлены
4. Проверьте права доступа и пользователей

---

**Версия документации**: 1.0  
**Дата**: 2025-01-09  
**Автор**: AdLawyer Team

