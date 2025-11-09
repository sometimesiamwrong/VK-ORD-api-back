# 🚀 Быстрый старт - Развертывание AdLawyer API

Это краткое руководство для быстрого развертывания проекта на новом сервере.

## Предварительные требования

- Ubuntu 20.04+ или Debian 11+
- Root доступ к серверу
- Git установлен
- Интернет соединение

## Шаг 1: Подготовка сервера

```bash
# Обновить систему
sudo apt update && sudo apt upgrade -y

# Установить необходимые пакеты
sudo apt install -y curl wget git

# Установить .NET 8.0 SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
echo 'export PATH="$PATH:$HOME/.dotnet"' >> ~/.bashrc
source ~/.bashrc
rm dotnet-install.sh

# Проверить установку
dotnet --version
```

## Шаг 2: Клонирование репозитория

```bash
# Перейти в домашнюю директорию
cd /root

# Клонировать репозиторий
git clone <ваш_repository_url> AdLawyerApi
cd AdLawyerApi

# Или если уже клонировали в другое место:
# cd /path/to/AdLawyerApi
```

## Шаг 3: Настройка конфигурации (опционально)

Если нужны нестандартные настройки:

```bash
# Создать файл конфигурации из примера
cp deploy.config.example.sh deploy.config.sh

# Отредактировать конфигурацию
nano deploy.config.sh

# Раскомментировать и изменить нужные параметры, например:
# WEBAPP_PORT="8000"
# JOBS_PORT="8001"
# ENABLE_CLO_TUNNEL=false
```

## Шаг 4: Запуск развертывания

```bash
# Сделать скрипт исполняемым
chmod +x deploy.sh

# Запустить развертывание
sudo ./deploy.sh
```

Скрипт автоматически:
- ✅ Установит .NET SDK (если не установлен)
- ✅ Скомпилирует проекты WebApp и Jobs
- ✅ Создаст службы systemd
- ✅ Настроит автозапуск
- ✅ Запустит оба сервиса
- ✅ Покажет статус служб

## Шаг 5: Проверка работы

```bash
# Проверить статус служб
sudo systemctl status adlawyer-webapp
sudo systemctl status adlawyer-jobs

# Проверить логи
sudo journalctl -u adlawyer-webapp -n 50
sudo journalctl -u adlawyer-jobs -n 50

# Проверить, что порты слушаются
sudo netstat -tlnp | grep -E ':(5000|5001)'

# Или
sudo ss -tlnp | grep -E ':(5000|5001)'
```

## Шаг 6: Тестирование API

```bash
# Проверить health endpoint WebApp
curl http://localhost:5000/health

# Проверить Swagger UI (если доступен)
curl http://localhost:5000/swagger/index.html

# Проверить Hangfire Dashboard Jobs
curl http://localhost:5001/hangfire
```

## Шаг 7: Настройка CLO туннеля (опционально)

Если нужен публичный доступ через CLO:

```bash
# Скачать CLO бинарь
wget https://clo.cloudpub.ru/clo -O /root/clo
chmod +x /root/clo

# Перезапустить деплой для создания туннелей
sudo ./deploy.sh

# Получить публичный URL
sudo journalctl -u clo-webapp-tunnel -n 20 | grep "https://"
sudo journalctl -u clo-jobs-tunnel -n 20 | grep "https://"
```

## Настройка Nginx (рекомендуется для Production)

```bash
# Установить Nginx
sudo apt install -y nginx

# Создать конфигурацию
sudo nano /etc/nginx/sites-available/adlawyer-api

# Вставить базовую конфигурацию:
```

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Активировать конфигурацию
sudo ln -s /etc/nginx/sites-available/adlawyer-api /etc/nginx/sites-enabled/

# Проверить конфигурацию
sudo nginx -t

# Перезапустить Nginx
sudo systemctl restart nginx

# Для HTTPS установить Certbot
sudo apt install -y certbot python3-certbot-nginx
sudo certbot --nginx -d your-domain.com
```

## Настройка GitLab CI/CD для автоматического деплоя

### 1. Настроить SSH ключи

На вашем компьютере:

```bash
# Сгенерировать SSH ключ
ssh-keygen -t ed25519 -C "gitlab-ci-deploy" -f ~/.ssh/gitlab-ci

# Скопировать публичный ключ на сервер
ssh-copy-id -i ~/.ssh/gitlab-ci.pub root@your-server-ip

# Показать приватный ключ для копирования
cat ~/.ssh/gitlab-ci
```

### 2. Добавить переменные в GitLab

Перейдите в GitLab: **Settings → CI/CD → Variables** и добавьте:

| Key | Value | Protected | Masked |
|-----|-------|-----------|--------|
| `SSH_PRIVATE_KEY` | содержимое приватного ключа | ✅ | ✅ |
| `SSH_HOST` | IP или домен сервера | ✅ | ❌ |
| `SSH_USER` | `root` | ❌ | ❌ |

### 3. Проверка работы CI/CD

```bash
# На вашем компьютере
cd AdLawyerApi
git add .
git commit -m "Test deployment"
git push origin main

# CI/CD пайплайн запустится автоматически
# Проверить можно в GitLab: CI/CD → Pipelines
```

## Полезные команды для управления

```bash
# Перезапустить службы
sudo systemctl restart adlawyer-webapp
sudo systemctl restart adlawyer-jobs

# Остановить службы
sudo systemctl stop adlawyer-webapp
sudo systemctl stop adlawyer-jobs

# Посмотреть логи в реальном времени
sudo journalctl -u adlawyer-webapp -f
sudo journalctl -u adlawyer-jobs -f

# Обновить приложение
cd /root/AdLawyerApi
git pull origin main
sudo ./deploy.sh

# Откатиться к предыдущей версии
cd /root/AdLawyerApi
git log --oneline -n 10  # посмотреть коммиты
git checkout <commit-hash>
sudo ./deploy.sh
git checkout main  # вернуться обратно
```

## Troubleshooting

### Служба не запускается

```bash
# Смотрим детальные логи
sudo journalctl -xe -u adlawyer-webapp

# Проверяем права
ls -la /var/www/adlawyer-webapp

# Проверяем конфигурацию службы
cat /etc/systemd/system/adlawyer-webapp.service
```

### Порт занят

```bash
# Проверить что использует порт
sudo lsof -i :5000

# Убить процесс
sudo kill -9 <PID>

# Или изменить порт в deploy.config.sh
```

### База данных недоступна

```bash
# Проверить подключение к PostgreSQL
psql -h 79.174.89.150 -p 19474 -U vkord_user -d vk_user

# В логах приложения искать ошибки подключения
sudo journalctl -u adlawyer-webapp | grep -i "database\|connection"
```

### .NET не найден

```bash
# Проверить установку
dotnet --version

# Если не установлен, установить вручную
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
export PATH="$PATH:$HOME/.dotnet"
```

## Что дальше?

После успешного развертывания:

1. 📖 Прочитайте [полное руководство](DEPLOYMENT-GUIDE.md) для детального понимания
2. ⚙️ Настройте [различные окружения](DEPLOYMENT-CONFIG.md) если нужно
3. 🔐 Настройте SSL сертификаты для HTTPS
4. 📊 Настройте мониторинг и алерты
5. 🔒 Настройте firewall и security

---

**Время развертывания**: ~10-15 минут  
**Требования**: Root доступ, интернет  
**Поддержка**: Свяжитесь с командой разработки

